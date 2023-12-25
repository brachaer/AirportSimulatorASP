using AirportSimulator.Services.ModelsDTO;

namespace AirportSimulator.Services
{
    public class AirportService : DataService
    {
        readonly ControllerType controller = ControllerType.Airport;
        public async Task<AirportDTO> GetAirport() => await GetData<AirportDTO>(controller);
        public async Task GetHubAirport() => await GetHub(controller);
    }
}