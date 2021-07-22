using Microsoft.EntityFrameworkCore;
using WorldCities.API.Models;

namespace WorldCities.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options){}

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}