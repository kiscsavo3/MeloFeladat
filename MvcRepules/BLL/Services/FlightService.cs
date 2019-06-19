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

        public async Task<IEnumerable> GetMyFlightsAsync(Guid UserId)
        {
            var queryFlights = from f in _appDbContext.Flight select f;
            var flights = await queryFlights.Where(f => f.UserId == UserId).ToListAsync();
            var queryGps = from g in _appDbContext.GlobalPoint select g;
            var globalPoints = await queryGps.ToListAsync();
            var queryAirports = from a in _appDbContext.Airport select a;
            List<Airport> airports = await queryAirports.ToListAsync();
            string DepPlace = "";
            string ArrPlace = "";
            List<MyFlightsDto> myFlights = new List<MyFlightsDto>();
            /*
            for (int i = 0; i < flights.Count; i++)
            {
                GlobalPoint gbDep = globalPoints.SingleOrDefault(g => g.GlobalPointId == flights[i].GlobalPointTakeoffPlaceId);
                GlobalPoint gbArr = globalPoints.SingleOrDefault(g => g.GlobalPointId == flights[i].GlobalPointLandPlaceId);
                if(gbDep != null && gbArr != null)
                {
                    foreach (var item in airports)
                    {
                        if (CalculateDistance(item.GlobalPoint, gbDep) <= 3000.0)
                        {
                            DepPlace = item.AirportName;
                        }
                        if (CalculateDistance(item.GlobalPoint, gbDep) <= 3000.0)
                        {
                            ArrPlace = item.AirportName;
                        }
                        else
                        {
                            ArrPlace = "Terep";
                        }
                    }
                    myFlights.Add(new MyFlightsDto
                    {
                        FlightDate = flights[i].FlightDate,
                        TakeoffPlace = DepPlace,
                        LandPlace = ArrPlace,
                        FlightTime = flights[i].FlightTime,
                        Status = flights[i].Status
                    });
                }
            }
            */
            return myFlights;
        }
        public double CalculateDistance(GlobalPoint start, GlobalPoint end)
        {
            double rlat1 = Math.PI * start.Latitude / 180;
            double rlat2 = Math.PI * end.Latitude / 180;
            double theta = start.Longitude - end.Longitude;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.344; // Méterben adja vissza a távolságot
        }

        public async Task<IEnumerable> GetAllFlightsAsync(int status)
        {
            IQueryable<Flight> flights = from m in _appDbContext.Flight select m;
            IList<Flight> result = new List<Flight>();
            switch (status)
            {
                case 0:
                    result = await flights.Where(m => m.Status == Status.Accepted).ToListAsync();
                    break;
                case 1:
                    result = await flights.Where(m => m.Status == Status.Denied).ToListAsync();
                    break;
                case 2:
                    result = await flights.Where(m => m.Status == Status.WaitingForAccept).ToListAsync();
                    break;
                default:
                    result = await flights.Where(m => m.Status == Status.WaitingForAccept).ToListAsync();
                    break;
            }
            return result;
        }

    }
}
