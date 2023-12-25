namespace AirportSimulator.API.Logic
{
    public interface ITimeLogic
    {
        Task DelayAsync(int milliseconds);
    }

}
