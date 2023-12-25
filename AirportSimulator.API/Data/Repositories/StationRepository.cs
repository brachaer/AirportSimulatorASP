using AirportSimulator.API.Models;
using AirportSimulator.API.Models.Enums;

namespace AirportSimulator.API.Data.Repositories
{
    public class StationRepository : IRepository<Station>
    {

        private readonly List<Station> _stations;

        public StationRepository()
        {
            _stations = new List<Station>
          {
            new Station { Id = 1, StationType=StationType.Start | StationType.Landing},
            new Station { Id = 2, StationType=StationType.Landing},
            new Station { Id = 3, StationType=StationType.Landing},
            new Station { Id = 4, StationType=StationType.Junction},
            new Station { Id = 5, StationType=StationType.Landing | StationType.Junction},
            new Station { Id = 6, StationType=StationType.Start | StationType.FinalStation},
            new Station { Id = 7, StationType=StationType.Start | StationType.FinalStation},
            new Station { Id = 8, StationType=StationType.TakingOff},
            new Station { Id = 9, StationType=StationType.FinalStation},
            new Station { Id = 0, StationType=StationType.Invisible}
        };
        }
        public async Task<IEnumerable<Station>> GetAll()=> await Task.FromResult<IEnumerable<Station>>(_stations);
        public async Task<Station> GetById(int id)
        {
            var station = _stations.FirstOrDefault(s => s.Id == id);
            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }
            return await Task.FromResult(station);
        }
        public async Task Add(Station entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            _stations.Add(entity);
            await Task.CompletedTask;
        }
        public async Task Update(Station entity)
        {
            var existingStation = await GetById(entity.Id) ?? throw new InvalidOperationException("Station not found");
            existingStation.IsOccupied = entity.IsOccupied;
            existingStation.Plane = entity.Plane;
            await Task.CompletedTask;
        }
    }

}
