using AirportSimulator.API.Hubs;
using AirportSimulator.API.Logic.Interfaces;
using AirportSimulator.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace AirportSimulator.API.Logic
{
    public class AirportLogic : IAirportLogic
    {
        private readonly ILogic<Station> _stationLogic;
        private readonly ILogic<Plane> _planeLogic;
        private readonly Random _random;
        private readonly IHubContext<AirportHub> _hubContext;
        private readonly SemaphoreSlim _semaphore;
        public AirportLogic(ILogic<Station> stationLogic, ILogic<Plane> planeLogic, IHubContext<AirportHub> hubContext)
        {
            _stationLogic = stationLogic;
            _planeLogic = planeLogic;
            _random = new Random();
            _hubContext = hubContext;
            _semaphore = new SemaphoreSlim(1, 1);
        }
        public async Task<Airport> Get()
        {
            var planes = await _planeLogic.GetAll();
            var stations = await _stationLogic.GetAll();
            var airport = new Airport
            {
                Planes = planes.ToList(),
                Stations = stations.ToList()
            };
            OnStart(airport);
            return airport;
        }
        public async Task<Airport> DoLogic()
        {
            await _semaphore.WaitAsync();
            try
            {
                var airport = await Get();
                if (airport.Planes.Count < 4)
                {
                    await NewEntry(airport);
                    airport = await Get();
                    await SendUpdateToClients(airport);
                }
                airport = await Move(airport);
                return airport;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Airport> NewEntry(Airport airport)
        {
            var rand = _random.Next(0, 2);
            var station = await _stationLogic.NewEntry(rand);
            if (station == null || station.IsOccupied)
                return airport;
            var plane = await _planeLogic.NewEntry(station.Id);
            station.Plane = plane;
            station.IsOccupied = true;
            return airport;
        }

        public async Task<Airport> Move(Airport airport)
        {
            if (await ReadyToMove(airport))
            {
                var planes = airport.Planes;
                var stations = airport.Stations;
                foreach (var plane in planes)
                {
                    var station = stations.FirstOrDefault(s => s.Id == plane.CurrentStation);
                    var nextStation = await _stationLogic.Move(station);
                    plane.CurrentStation = nextStation.Id;
                    await _planeLogic.Move(plane);
                    await SendUpdateToClients(airport);
                }
            }
            return airport;
        }
        private async Task<bool> ReadyToMove(Airport airport)
        {
            var planes = airport.Planes;
            var stations = airport.Stations;
            foreach (var plane in planes)
            {
                if (await _planeLogic.ReadyToMove(plane))
                {
                    var station = stations.FirstOrDefault(s => s.Id == plane.CurrentStation);
                    if (await _stationLogic.ReadyToMove(station))
                        continue;
                    return false;
                }
                return false;
            }
            return true;
        }
        private static void OnStart(Airport airport)
        {
            var stations = airport.Stations;
            var planes = airport.Planes;
            foreach (var plane in planes)
                UpdateStation(stations, plane);
        }
        private static void UpdateStation(IEnumerable<Station> stations, Plane plane)
        {
            var station = stations.FirstOrDefault(s => s.Id == plane.CurrentStation);
            station.Plane = plane;
            station.IsOccupied = true;
        }
        private async Task SendUpdateToClients(Airport airport)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveAirportUpdate", airport);
        }
    }
}
