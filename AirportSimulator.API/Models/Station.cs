using AirportSimulator.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AirportSimulator.API.Models
{
    public class Station
    {
        [Key]
        public int Id { get; set; } 
        public StationType StationType { get; set; }
        public bool IsOccupied { get; set; } = false;
        public Plane Plane { get; set; }
    }
}
