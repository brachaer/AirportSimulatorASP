using AirportSimulator.Services;
using AirportSimulator.Services.ModelsDTO;

AirportService service = new AirportService();
bool stop = false;
Console.WriteLine("Welcome Airport Simulator");
Console.WriteLine("Current Airport Status : ");

var airport = await service.GetAirportStatus();
if (airport != null)
{
    var planes = airport.Planes.ToList();
    var stations = airport.Stations.ToList();
    foreach (var plane in planes)
    {
        Console.WriteLine($"{plane.FlightNumber} | Station {plane.CurrentStation} | {plane.Status}");
    }
    foreach (var station in stations)
    {
        Console.WriteLine($"Station Number {station.Id}, is occupied: {station.IsOccupied}  ");
    }
}
   
while (!stop)
{
    Console.WriteLine("Enter any key to simulate airport, Enter 'stop' to stop simulator :");
    var res = Console.ReadLine();
    if (res == "stop")
    {
        stop = true;
        break;
    }
    await service.SimulateRealTimeAirport();
    airport = await service.GetData<AirportDTO>(ControllerType.Airport);
    if (airport != null)
    {
        Console.WriteLine("Current Airport State: ");
        var planes = airport.Planes.ToList();
        foreach (var plane in planes)
        {
            Console.WriteLine($"{plane.FlightNumber} : Station {plane.CurrentStation} : {plane.Status}");
        }   
    }
}
       Console.ReadLine();