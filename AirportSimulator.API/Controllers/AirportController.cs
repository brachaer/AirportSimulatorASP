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
        public async Task<Airport> GetAirport()
        {
            var airport = await _airportLogic.Get();
            return airport;
        }

        [HttpGet("hub")]
        public async Task<ActionResult> GetHub()
        {
            await _airportLogic.DoLogic();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> EnterAirport()
        {
            var airport = await _airportLogic.Get();
            await _airportLogic.NewEntry(airport);
            return Ok(airport);
        }

        [HttpPut]
        public async Task<Airport> DoLogic()
        {
            var airport = await _airportLogic.DoLogic();

            return airport;
        }
    }
}
