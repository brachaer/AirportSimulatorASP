using AirportSimulator.API.Models;
using Microsoft.EntityFrameworkCore;


namespace AirportSimulator.API.Data
{
    public class AirportDbContext : DbContext
    {
        public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options) { }
        public virtual DbSet<Plane> Planes { get; set; }

    }
}
