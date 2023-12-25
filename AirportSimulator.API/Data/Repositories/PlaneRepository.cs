using AirportSimulator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AirportSimulator.API.Data.Repositories
{
    public class PlaneRepository: IRepository<Plane>
    {
        private readonly AirportDbContext _context;
        public PlaneRepository(DbContextOptions<AirportDbContext> options)
        {
            _context= new AirportDbContext(options);
        }
        public async Task<IEnumerable<Plane>> GetAll()=> await _context.Planes.ToListAsync();
        public async Task<Plane> GetById(int id) => await _context.Planes.FirstOrDefaultAsync(p => p.Id == id);
        public async Task Add(Plane entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Plane entity)
        {
            var existingPlane = await _context.Planes.FindAsync(entity.Id);

            if (existingPlane == null)
            {
                throw new InvalidOperationException("Plane not found");
            }
            _context.Entry(existingPlane).CurrentValues.SetValues(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

    }
}
