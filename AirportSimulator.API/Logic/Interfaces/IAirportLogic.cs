using AirportSimulator.API.Models;

namespace AirportSimulator.API.Logic.Interfaces
{
    public interface IAirportLogic
    {
        Task<Airport> Get();
        Task<Airport> DoLogic();
        Task<Airport> NewEntry(Airport airport);
        Task<Airport> Move(Airport airport);
    }
}
