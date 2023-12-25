using AirportSimulator.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AirportSimulator.API.Models
{
    public class Plane
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(5)]
        public string FlightNumber { get; set; } = "IL123";
        public int CurrentStation { get; set; } = 0;
        public FlightStatus Status { get; set; } = FlightStatus.Landing;
        public bool ReadyForNextStation { get; set; } = false;
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public DateTime? ExitDate { get; set; }
    }
}
