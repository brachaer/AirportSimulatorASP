using AirportSimulator.API.Data.Repositories;
using AirportSimulator.API.Logic.Interfaces;
using AirportSimulator.API.Models;
using AirportSimulator.API.Models.Enums;
using System.Data;

namespace AirportSimulator.API.Logic
{
    public class StationLogic : ILogic<Station>
    {
        private readonly IRepository<Station> _stationRepository;
        private readonly ITimeLogic _timeLogic;
        public StationLogic(IRepository<Station> stationRepository, ITimeLogic timeLogic)
        {
            _stationRepository = stationRepository;
            _timeLogic = timeLogic;
        }
        public async Task<IEnumerable<Station>> GetAll() => await _stationRepository.GetAll();
        public async Task<Station> GetById(int id) => await _stationRepository.GetById(id);
        public async Task<Station> NewEntry(int type)
        {
            var stationType = type == 0 ? StationType.Start | StationType.Landing : StationType.Start | StationType.FinalStation;
            var stations = await GetAll();
            var station = stations.FirstOrDefault(s => s.StationType.HasFlag(stationType));
            if ((station.StationType & StationType.Landing) != 0)
                return await NewLanding(station, stations);
            if ((station.StationType & StationType.FinalStation) != 0)
                return await NewTakeOff(station, stations);
            return null;
        }
        public async Task<bool> ReadyToMove(Station entity)
        {
            await _timeLogic.DelayAsync(GetTimeDelay(entity.Id));
            if (entity.IsOccupied)
                return true;
            return false;
        }
        public async Task<Station> Move(Station entity)
        {
            var ns = await CheckStationType(entity);

            if (ns == null)
                return entity;
            if (!ns.IsOccupied && ns.Plane == null && ns.Id != 0)
                return await UpdateStation(entity, ns);
            else if (ns.StationType == StationType.Invisible && ns.Id == 0)
                return await UpdateExit(entity, ns);
            else
                return entity;
        }
        private async Task<Station> NewLanding(Station station, IEnumerable<Station> stations)
        {
            if (CheckAirportStations(station, stations) && CheckLandingAvailable(stations))
            {
                await _timeLogic.DelayAsync(GetTimeDelay(station.Id));
                return station;
            }
            return null;
        }
        private async Task<Station> NewTakeOff(Station station, IEnumerable<Station> stations)
        {
            await _timeLogic.DelayAsync(GetTimeDelay(station.Id));

            if (CheckAirportStations(station, stations))
                return station;
            else
            {
                var takeOffStation = stations.FirstOrDefault(s => s.StationType == station.StationType && s.Id != station.Id);
                if (CheckAirportStations(takeOffStation, stations))
                    return takeOffStation;
                return null;
            }
        }
        private async Task<Station> CheckStationType(Station station)
        {
            var stations = await GetAll();
            var stationType = station.StationType;
            var flightStatus = station.Plane.Status;
            switch (stationType)
            {
                case StationType.Landing:
                case StationType.Landing | StationType.Start:
                    return await GetById(station.Id + 1);
                
                case StationType.Landing | StationType.Junction:
                    {
                        var ns = await GetById(station.Id + 1);
                        if (ns.IsOccupied)
                        {
                            var other = await GetById(ns.Id + 1);
                            if (other.StationType == ns.StationType)
                                return other;
                        }
                        else
                            return ns;
                        break;
                    }
                
                case StationType.Junction:
                    {
                        if (flightStatus == FlightStatus.TakingOff)
                            return GetStationByType(stations, StationType.FinalStation);
                        else if (flightStatus == FlightStatus.Landing)
                            return await GetById(station.Id + 1);
                        break;
                    }

                case StationType.TakingOff:
                    return GetStationByType(stations, StationType.Junction);

                case StationType.FinalStation:
                    return GetStationByType(stations, StationType.Invisible);

                case StationType.FinalStation | StationType.Start:
                    {
                        if (flightStatus == FlightStatus.Landing)
                            return GetStationByType(stations, StationType.Invisible);
                        else if (flightStatus == FlightStatus.TakingOff)
                            return GetStationByType(stations, StationType.TakingOff);
                        break;
                    }
                
                default:
                    break;
            }
            return null;
        }
        private async Task<Station> UpdateExit(Station station, Station nextStation)
        {
            station.IsOccupied = false;
            station.Plane = null;
            await _timeLogic.DelayAsync(GetTimeDelay(station.Id));
            return nextStation;
        }
        private async Task<Station> UpdateStation(Station station, Station nextStation)
        {
            nextStation.IsOccupied = true;
            nextStation.Plane = station.Plane;
            station.IsOccupied = false;
            station.Plane = null;
            await _timeLogic.DelayAsync(GetTimeDelay(station.Id));
            return nextStation;
        }
        private static Station GetStationByType(IEnumerable<Station> stations, StationType type)
        {
            if (type == StationType.FinalStation)
                return stations.FirstOrDefault(s =>
                                       s.StationType.HasFlag(type) &&
                                       Enum.Equals(s.StationType, type));
            else
                return stations.FirstOrDefault(s => s.StationType.HasFlag(type));
        }
        private static bool CheckAirportStations(Station station, IEnumerable<Station> stations)
        {
            if (station.IsOccupied)
                return false;
            else
            {
                stations = stations.Where(s => s.IsOccupied);
                if (stations.Count() > 4)
                    return false;
            }
            return true;
        }
        private static bool CheckLandingAvailable(IEnumerable<Station> stations)
        {
            stations = stations.Where(s => s.Id <= 3).ToList();
            var result = stations.FirstOrDefault(s => s.IsOccupied);
            if (result != null)
                return false;
            return true;
        }
        private static int GetTimeDelay(int stationId) => stationId * 100;
    }
}
