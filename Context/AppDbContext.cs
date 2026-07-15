using Microsoft.EntityFrameworkCore;
using WeatherAPI.Model;

namespace WeatherAPI.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Weather> HistoricoTemp => Set<Weather>();
    }
}
