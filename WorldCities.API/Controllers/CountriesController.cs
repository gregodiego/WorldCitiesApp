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
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            return await _context.Countries.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id){
            var country = await _context.Countries.FindAsync(id);
            if (country == null){
                return NoContent();
            }
            return country;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutCountry(int id, Country country){
            if(id != country.Id){
                return BadRequest();
            }
            
            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!CountryExists(id)){
                    return NoContent();
                }else{
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country){
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new{
                id = country.Id
            }, country);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Country>> DeleteCountry(int id){
            var country = await _context.Countries.FindAsync(id);

            if(country == null){
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(x => x.Id == id);
        }
    }
}