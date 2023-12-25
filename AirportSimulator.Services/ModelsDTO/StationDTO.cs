namespace AirportSimulator.Services.ModelsDTO
{
    public class StationDTO
    {
        public int Id { get; set; }
        public bool IsOccupied { get; set; }
        public PlaneDTO Plane { get; set; }
    }
}
