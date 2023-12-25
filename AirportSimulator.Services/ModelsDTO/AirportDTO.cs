using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportSimulator.Services.ModelsDTO
{
    public class AirportDTO
    {
       public ICollection<PlaneDTO> Planes {  get; set; }
       public ICollection<StationDTO> Stations { get; set; }
    }
}
