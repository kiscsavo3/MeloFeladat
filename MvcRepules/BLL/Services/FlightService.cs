using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using MvcRepules.BLL.DTO;
using MvcRepules.DAL;
using MvcRepules.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcRepules.BLL.Services
{
    public class FlightService
    {
        private readonly ApplicationDbContext _appDbContext;
        public FlightService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        private async Task<List<FlightsDto>> GetFlightsAsync(Guid? UserId = null, int? status = null)
        {
            List<FlightsDto> Flights = new List<FlightsDto>();
            var queryFlights = from f in _appDbContext.Flight select f;
            List<Flight> flights = new List<Flight>();
            if (UserId != null)
            {
                flights = await queryFlights.Where(f => f.UserId == UserId)
                    .Include(e => e.TakeoffPlace)
                    .Include(e => e.LandPlace)
                    .ToListAsync();
            }
            if (status != null)
            {
                switch (status)
                {
                    case 0:
                        flights = await queryFlights.Where(f => f.Status == Status.Accepted)
                            .Include(e => e.User)
                            .Include(e => e.TakeoffPlace)
                            .Include(e => e.LandPlace)
                            .ToListAsync();
                        break;
                    case 1:
                        flights = await queryFlights.Where(f => f.Status == Status.Denied)
                            .Include(e => e.User)
                            .Include(e => e.TakeoffPlace)
                            .Include(e => e.LandPlace)
                            .ToListAsync();
                        break;
                    case 2:
                        flights = await queryFlights.Where(f => f.Status == Status.Waiting_For_Accept)
                             .Include(e => e.User)
                             .Include(e => e.TakeoffPlace)
                             .Include(e => e.LandPlace)
                             .ToListAsync();
                        break;
                    default:
                        flights = await queryFlights.Where(f => f.Status == Status.Waiting_For_Accept)
                            .Include(e => e.User)
                            .Include(e => e.TakeoffPlace)
                            .Include(e => e.LandPlace)
                            .ToListAsync();
                        break;
                }
            }
            List<Airport> airports = await _appDbContext.Airport
                .Include(e => e.GlobalPoint)
                .ToListAsync();
            string DepPlace = "";
            string ArrPlace = "";

            foreach (var flight in flights)
            {
                DepPlace = airports.Find(m => CalculateDistance(m.GlobalPoint, flight.TakeoffPlace) <= 3000.0).AirportName;
                if (airports.Find(m => CalculateDistance(m.GlobalPoint, flight.LandPlace) <= 3000.0) == null) { ArrPlace = "Terep"; }
                else { ArrPlace = airports.Find(m => CalculateDistance(m.GlobalPoint, flight.LandPlace) <= 3000.0).AirportName; }

                FlightsDto flightDto;
                if (status != null)
                {
                    flightDto = new AllFlightsDto();
                    (flightDto as AllFlightsDto).PilotName = flight.User.UserName;
                    (flightDto as AllFlightsDto).Email = flight.User.Email;
                }
                else
                {
                    flightDto = new FlightsDto();
                }

                flightDto.FlightId = flight.FlightId;
                flightDto.FlightDate = flight.FlightDate;
                flightDto.TakeoffPlace = DepPlace;
                flightDto.LandPlace = ArrPlace;
                flightDto.FlightTime = flight.FlightTime;
                flightDto.Status = flight.Status;
                Flights.Add(flightDto);
            }
            return Flights; 
        }

        public async Task<List<FlightsDto>> GetMyFlightsAsync(Guid UserId)
        {
            return await GetFlightsAsync(UserId);
        }

        public double CalculateDistance(GlobalPoint start, GlobalPoint end)
        {
            GeoCoordinate pin1 = new GeoCoordinate(start.Latitude, start.Longitude);
            GeoCoordinate pin2 = new GeoCoordinate(end.Latitude, end.Longitude);

            double distanceBetween = pin1.GetDistanceTo(pin2);

            return distanceBetween;
        }

        public async Task<List<AllFlightsDto>> GetAllFlightsAsync(int status)
        {
            return (await GetFlightsAsync(null, status))
                .Select(e => e as AllFlightsDto).ToList();
        }

        public async Task<FlightsDto> GetSelectedFlightPilotAsync(Guid FlightId)
        {          
            var selectedFlight = await  _appDbContext.Flight
                .Include(e => e.TakeoffPlace)
                .Include(e => e.LandPlace)
                .FirstOrDefaultAsync(f => f.FlightId == FlightId);

            var airports = await _appDbContext.Airport
                .Include(e => e.GlobalPoint)
                .ToListAsync();

            string ArrPlace = "";
            string DepPlace = airports.Find(m => CalculateDistance(m.GlobalPoint, selectedFlight.TakeoffPlace) <= 3000.0).AirportName;
            if (airports.Find(m => CalculateDistance(m.GlobalPoint, selectedFlight.LandPlace) <= 3000.0) == null) { ArrPlace = "Terep"; }
            else { ArrPlace = airports.Find(m => CalculateDistance(m.GlobalPoint, selectedFlight.LandPlace) <= 3000.0).AirportName; }
            return new FlightsDto()
            {
                FlightId = selectedFlight.FlightId,
                FlightDate = selectedFlight.FlightDate,
                FlightTime = selectedFlight.FlightTime,
                Status = selectedFlight.Status,
                TakeoffPlace = DepPlace,
                LandPlace = ArrPlace
            };
        }

        public async Task<AllFlightsDto> GetSelectedFlightAdminAsync(Guid FlightId)
        {
            FlightsDto flightsDto = await GetSelectedFlightPilotAsync(FlightId);
            var selectedFlight = await _appDbContext.Flight
                .Include(e => e.User)
                .FirstOrDefaultAsync(f => f.FlightId == FlightId);
            return new AllFlightsDto()
            {
                FlightId = flightsDto.FlightId,
                FlightDate = flightsDto.FlightDate,
                FlightTime = flightsDto.FlightTime,
                Status = flightsDto.Status,
                TakeoffPlace = flightsDto.TakeoffPlace,
                LandPlace = flightsDto.LandPlace,
                PilotName = selectedFlight.User.UserName,
                Email = selectedFlight.User.Email
            };
        }

        public async Task<bool> EditStatusToAccept(Guid FlightId)
        {
            var flight = await _appDbContext.Flight.FirstOrDefaultAsync(f => f.FlightId == FlightId);
            //var newFlight = flight;
            //newFlight.Status = Status.Accepted;
            //_appDbContext.Flight.Remove(flight);
            //_appDbContext.Flight.Add(newFlight);
            flight.Status = Status.Accepted;
            _appDbContext.Flight.Update(flight);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditStatusToDeny(Guid FlightId)
        {
            var flight = await _appDbContext.Flight.FirstOrDefaultAsync(f => f.FlightId == FlightId);
            var newFlight = flight;
            newFlight.Status = Status.Denied;
            _appDbContext.Flight.Remove(flight);
            _appDbContext.Flight.Add(newFlight);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

    }
}
