using AirportSimulator.API.Logic.Interfaces;

namespace AirportSimulator.API.Logic
{
    public class TimeLogic : ITimeLogic
    {
        public async Task DelayAsync(int milliseconds) => await Task.Delay(milliseconds);
    }
}
