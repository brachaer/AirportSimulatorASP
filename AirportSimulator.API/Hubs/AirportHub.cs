using AirportSimulator.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace AirportSimulator.API.Hubs
{
    public class AirportHub: Hub
    {
        public async Task LiveSimulator(Airport airport)
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("ReceiveAirportUpdate", airport);
            }
        }
    }
}
