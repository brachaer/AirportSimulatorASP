using AirportSimulator.ClientMVC.Models;
using AirportSimulator.Services;
using AirportSimulator.Services.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AirportSimulator.ClientMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AirportService _airportService;
        public HomeController(AirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var airportDTO = await _airportService.GetAirport();
            return View(airportDTO);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateData()
        {
            await _airportService.GetHubAirport();

            return Ok();
        }

        [HttpPost]
        public IActionResult AirportMapPartial(IEnumerable<StationDTO> stations)
        {
            return PartialView("_AirportMapPartial", stations);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}