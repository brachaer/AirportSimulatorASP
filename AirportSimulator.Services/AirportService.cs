using AirportSimulator.Services.ModelsDTO;

namespace AirportSimulator.Services
{
    public class AirportService : DataService
    {
        readonly ControllerType controller = ControllerType.Airport;
        public async Task<AirportDTO> GetAirportStatus() => await GetData<AirportDTO>(controller);
        public async Task SimulateRealTimeAirport() => await SimulateRealTime(controller);
    }
}