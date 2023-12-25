namespace AirportSimulator.Services.ModelsDTO
{
    public class PlaneDTO
    {
        public string FlightNumber { get; set; }
        public int CurrentStation { get; set; }
        public FlightStatus Status { get; set; }
    }
}
