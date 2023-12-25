$(document).ready(function () {
    let isSimulatorRunning = false;
    let isUpdating = false;
    let intervalId;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7129/airporthub")
        .build();

    connection.start().then(() => {
        console.log("Connected to SignalR Hub!");
    }).catch((error) => {
        console.error("Error connecting:", error);
    });

    connection.on("ReceiveAirportUpdate", (airport) => {
        const airportDTO = {
            Planes: airport.planes.map(plane => ({
                FlightNumber: plane.flightNumber,
                CurrentStation: plane.currentStation,
                Status: mapFlightStatus(plane.status)
            })),
            Stations: airport.stations.map(station => ({
                Id: station.id,
                IsOccupied: station.isOccupied,
                Plane: station.plane
            }))
        };
        updateViewWithAirportData(airportDTO);
    });

    function updateViewWithAirportData(airportDTO) {
    
            $('#airport-planes').empty();
        $('#airport-planes').append(`<h3>Flights in Airport:</h3>`)
            airportDTO.Planes.forEach(plane => {
                if (plane.CurrentStation != 0)
                    $('#airport-planes').append(`<h4 class="${plane.Status.toLowerCase()}">${plane.FlightNumber} : Station ${plane.CurrentStation} : ${plane.Status}</h4>`);
            });
        updateStationsView(airportDTO.Stations);
    }
    function updateStationsView(stations) {
        $.ajax({
            url: '/Home/AirportMapPartial',
            type: 'POST',
            data: { stations: stations },
            success: function (result) {
                $('#airport-map-section').html(result);
            },
            error: function (error) {
                console.error('Error rendering stations:', error);
            }
        });
    }
    function mapFlightStatus(statusNumber) {
        switch (statusNumber) {
            case 0:
                return "Landing";
            case 2:
                return "TakingOff";
            default:
                return "Unknown";
        }
    }
    function updateData() {
        if (isUpdating || connection.state !== 'Connected') {
            return; 
        }

        isUpdating = true;

        if (isSimulatorRunning) {
            console.log('Simulator is running...');

            $.ajax({
                url: '/Home/UpdateData',
                type: 'GET',
                success: function (result) {
                    console.log('Update data success:', result);
                },
                error: function (error) {
                    console.error('Update data error:', error);
                },
                complete: function () {
                    isUpdating = false; 
                }
            });
        } else {
            console.log('Simulator is stopped.');
            isUpdating = false;
        }
    }
    $('#updateButton').click(function () {
        console.log('Button clicked. isSimulatorRunning:', isSimulatorRunning);

        isSimulatorRunning = !isSimulatorRunning;

        if (isSimulatorRunning) {
            console.log('Starting simulator...');
            updateData();
           intervalId= setInterval(updateData, 2000);
            $(this).text('Stop Simulator');
        } else {
            clearInterval(intervalId);
            $(this).text('Start Simulator');
            console.log('Stopping simulator...');
        }
    });
});
