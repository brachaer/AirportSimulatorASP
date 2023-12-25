namespace AirportSimulator.API.Models
{
    public class Airport
    {
        public ICollection<Plane> Planes { get; set; }
        public ICollection<Station> Stations { get; set; }
    }
}
