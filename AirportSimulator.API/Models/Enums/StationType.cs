namespace AirportSimulator.API.Models.Enums
{
    [Flags]
    public enum StationType
    {
        Start=1,
        Landing=2,
        Junction=4,
        TakingOff = 8,
        FinalStation = 16,
        Invisible=32
    }
}
