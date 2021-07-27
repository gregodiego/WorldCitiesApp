using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities.API.Data;
using WorldCities.API.Models;

namespace WorldCities.API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CitiesController : Controller
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ApiResult<City>> GetCities(int pageIndex = 0, int pageSize = 10)
        {
            return await ApiResult<City>.CreateAsync(_context.Cities, pageIndex, pageSize);
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id){
            var city = await _context.Cities.FindAsync(id);
            if(city == null){
                return NotFound();
            }
            return city;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city){
            if(id != city.Id){
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try{
                await _context.SaveChangesAsync();
            }catch(DbUpdateConcurrencyException){
                if(!CityExists(id)){
                    return NotFound();
                }else{
                    throw;
                }
            }
            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<City>> PostCitie(City city){
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new{
                id = city.Id
            }, city);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<City>> DeleteCity(int id){
            var city =  await _context.Cities.FindAsync(id);
            if(city == null){
                return NoContent();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }


    }
}