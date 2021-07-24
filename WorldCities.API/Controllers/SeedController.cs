using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security;
using WorldCities.API.Data;
using WorldCities.API.Models;

namespace WorldCities.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        } 

        [HttpGet]
        public async Task<ActionResult> Import(){
            if(!_env.IsDevelopment()){
                throw new SecurityException("Not Allowed");
            }
            
            var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");

            var stream = System.IO.File.OpenRead(path); 
            var excelPackage = new ExcelPackage(stream);

            //define o primeiro worksheet
            var workSheet = excelPackage.Workbook.Worksheets[0];

            //define quantas linhas queremos processar
            var nEndRow = workSheet.Dimension.End.Row;

            //inicia os contadores 
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            //criara um dicionário de pesquisa
            //contendo todos os países já existentes
            // no banco de dados (estará vazio na primeira execução)
            var countriesByName = _context.Countries.AsNoTracking().ToDictionary( x => x.Name, StringComparer.OrdinalIgnoreCase);

            //itera por todas as linhas, pulando a primeira.
            for( int nRow = 2; nRow <= nEndRow; nRow++){
                var row = workSheet.Cells[nRow, 1, nRow, workSheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();

                //pula esse país se já existir no banco de dados
                if(countriesByName.ContainsKey(countryName)){
                    continue;
                }

                //cria a entidade do país com todos os dados xlsx
                var country = new Country{
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };
                //adiciona o país utilizando o contexto
                await _context.Countries.AddAsync(country);

                //armazene o país em nossa pesquisa para recuperar seu id mais tarde
                countriesByName.Add(countryName, country);

                //incrementa o contador de países
                numberOfCountriesAdded++;

            }
            //salva todos os países no banco
            if(numberOfCountriesAdded > 0){
                await _context.SaveChangesAsync();
            }

            // cria um dicionário de pesquisa
            // contendo todas as cidades já existentes
            // no banco de dados (estará vazio na primeira execução).
            var cities = _context.Cities.AsNoTracking().ToDictionary( x =>(
                Name: x.Name,
                Lat: x.Lat,
                Lon: x.Lon,
                CountryId: x.CountryId
                )
            );
            // itera por todas as linhas, pulando a primeira
            for(int nRow = 2; nRow <= nEndRow; nRow++){
                var row = workSheet.Cells[
                    nRow, 1, nRow, workSheet.Dimension.End.Column
                ];

                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                // recuperar ID do país por countryName
                var countryId = countriesByName[countryName].Id;

                // ignorar esta cidade se ela já existir no banco de dados
                if(cities.ContainsKey((Name: name, Lat: lat, Lon: lon, CountryId: countryId))){
                    continue;
                }

                // cria a entidade City e preenche-a com dados xlsx
                var city = new City{
                    Name = name,
                    Name_ASCII = nameAscii,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };

                //adiciona a nova cidade ao contexto do dbcontext
                _context.Cities.Add(city);

                //incrementa o contador de cidades
                numberOfCitiesAdded++;
            }

            //salva todas as cidades no banco de dados
            if(numberOfCitiesAdded > 0){
               await _context.SaveChangesAsync();
            }

            return new JsonResult( new 
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            }
            );
        }
    }
}