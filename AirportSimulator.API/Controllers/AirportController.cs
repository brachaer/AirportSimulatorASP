using AirportSimulator.API.Logic.Interfaces;
using AirportSimulator.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirportSimulator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IAirportLogic _airportLogic;

        public AirportController(IAirportLogic airportLogic)
        {
            _airportLogic = airportLogic;
        }

        [HttpGet]
        public async Task<Airport> GetAirportCurrentStatus()
        {
            var airport = await _airportLogic.Get();
            return airport;
        }

        [HttpGet("hub")]
        public async Task<ActionResult> SimulateAirportLogicRealTime()
        {
            await _airportLogic.DoLogic();
            return Ok();
        }

    }
}
