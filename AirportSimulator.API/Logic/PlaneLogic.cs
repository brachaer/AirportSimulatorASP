using AirportSimulator.API.Data.Repositories;
using AirportSimulator.API.Logic.Interfaces;
using AirportSimulator.API.Models;
using AirportSimulator.API.Models.Enums;

namespace AirportSimulator.API.Logic
{
    public class PlaneLogic : ILogic<Plane>
    {
        private readonly IRepository<Plane> _planeRepository;
        private readonly ITimeLogic _timeLogic;
        private readonly Random _random;
        public PlaneLogic(IRepository<Plane> planeRepository, ITimeLogic timeLogic)
        {
            _planeRepository = planeRepository;
            _timeLogic = timeLogic;
            _random = new Random();
        }

        public async Task<IEnumerable<Plane>> GetAll()
        {
            var planes = await _planeRepository.GetAll();
            planes = planes.Where(p => p.CurrentStation != 0);
            return planes;
        }
        public async Task<Plane> GetById(int id)
        {
            var planes = await GetAll();
            var plane = planes.FirstOrDefault(p => p.Id == id);
            return plane;
        }
        public async Task<Plane> NewEntry(int stationId) => await GeneratePlane(stationId);

        public async Task<bool> ReadyToMove(Plane entity)
        {
            await _timeLogic.DelayAsync(GetTimeDelay(entity.CurrentStation));

            entity.ReadyForNextStation = true;
            await _planeRepository.Update(entity);
            return entity.ReadyForNextStation;
        }

        public async Task<Plane> Move(Plane entity)
        {
            var exitAirport = CheckIfExiting(entity);
            if (exitAirport)
                return await UpdateFlightExit(entity);
            if (entity.ReadyForNextStation)
            {
                entity.ReadyForNextStation = false;
                await _planeRepository.Update(entity);
                await _timeLogic.DelayAsync(GetTimeDelay(entity.CurrentStation));
                return entity;
            }
            else
            {
                await ReadyToMove(entity);
                return await Move(entity);
            }
        }
        private async Task<Plane> GeneratePlane(int stationId)
        {
            var plane = new Plane
            {
                FlightNumber = "IL" + _random.Next(1000),
                Status = stationId == 1 ? FlightStatus.Landing : FlightStatus.TakingOff,
                CurrentStation = stationId
            };
            await _planeRepository.Add(plane);
            await _timeLogic.DelayAsync(GetTimeDelay(plane.CurrentStation));
            await ReadyToMove(plane);
            return plane;
        }
        private static bool CheckIfExiting(Plane plane)
        {
            if (plane.CurrentStation == 0)
                return true;
            return false;
        }
        private async Task<Plane> UpdateFlightExit(Plane plane)
        {
            plane.CurrentStation = 0;
            plane.ReadyForNextStation = false;
            plane.ExitDate = DateTime.Now;

            if (plane.Status == FlightStatus.Landing)
                plane.Status = FlightStatus.Landed;
            else
                plane.Status = FlightStatus.TookOff;
            await _planeRepository.Update(plane);
            await _timeLogic.DelayAsync(GetTimeDelay(plane.CurrentStation));
            return plane;
        }
        private static int GetTimeDelay(int stationId) => stationId * 100;
    }
}
