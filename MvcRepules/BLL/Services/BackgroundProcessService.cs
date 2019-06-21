using Hangfire;
using MvcRepules.DAL;
using MvcRepules.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL.Services
{
    public class BackgroundProcessService
    {
        private readonly ApplicationDbContext _appDbContext;
        private List<string[]> gps;
        public BackgroundProcessService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            gps = new List<string[]>();
        }
        public void RunInBackground()
        {
            BackgroundJob.Schedule(
                () => DataMining(), TimeSpan.FromSeconds(1.0));
        }

        public void DataMining()
        {
            List<PilotLog> pilotLogs = _appDbContext.PilotLog.ToList();
            foreach (var pilotLog in pilotLogs)
            {
                string text = System.Text.Encoding.UTF8.GetString(pilotLog.File);

                List<string> file = new List<string>();
                string[] arr = text.Split('\n');
                foreach (string item in arr)
                {
                    file.Add(item);
                }
                string datumUnformed = file[1];
                string datumYear = "20" + datumUnformed.Substring(9, 2);
                string datumMonth = datumUnformed.Substring(7, 2);
                string datumDay = datumUnformed.Substring(5, 2);

                // this is the FlightDate
                DateTime flightDate = new DateTime(
                    Int32.Parse(datumYear), Int32.Parse(datumMonth), Int32.Parse(datumDay));

                //select records, which begins with B
                foreach (string record in file)
                {
                    if (record.IndexOf('B').Equals(0))
                    {
                        string time = record.Substring(1, 6);
                        string lati = record.Substring(7, 7);
                        string longi = record.Substring(15, 8);
                        string[] selector = new string[] { time, lati, longi };
                        gps.Add(selector);
                    }
                }
                
                //find departure time and place
                string[] inspectorDep = new string[]
                {
                    gps[0][0], gps[0][1], gps[0][2]
                };
                inspectorDep = FindDepTimeAndPlace(inspectorDep);
                

                // find arrive time and place
                string[] inspectorArr = new string[]
                {
                    gps[gps.Count-1][0], gps[gps.Count-1][1], gps[gps.Count-1][2]
                };
                inspectorArr = FindArrTimeAndPlace(inspectorArr);

                //calculate dep time:

                DateTime depTime = CalculateTime(inspectorDep[0], flightDate);

                //calculate arrive time
                DateTime arrTime = CalculateTime(inspectorArr[0], flightDate);
                
                // this is the FlightTime
                var flightTime = arrTime - depTime;

                // this is the dep place (GlobalPoint)
                float latitudeDep = GetLatitude(inspectorDep[1]);
                float longitudeDep = GetLongitude(inspectorDep[2]);

                // this is the arr place (GlobalPoint)
                float latitudeArr = GetLatitude(inspectorArr[1]);
                float longitudeArr = GetLongitude(inspectorArr[2]);

                GlobalPoint gpDep = new GlobalPoint
                {
                    Latitude = latitudeDep,
                    Longitude = longitudeDep
                };

                GlobalPoint gpArr = new GlobalPoint
                {
                    Latitude = latitudeArr,
                    Longitude = longitudeArr
                };

                Flight newFlightRecord = new Flight
                {
                    FlightDate = flightDate,
                    FlightTime = flightTime,
                    TakeoffPlace = gpDep,
                    LandPlace = gpArr,
                    Status = Status.Waiting_For_Accept,
                    UserId = pilotLog.UserId,
                };

                // insert the new records into database:
                _appDbContext.GlobalPoint.Add(gpDep);
                _appDbContext.GlobalPoint.Add(gpArr);
                _appDbContext.Flight.Add(newFlightRecord);
                // remove from pilotlog db table the processed record
                _appDbContext.PilotLog.Remove(pilotLog);

                //calculate all of gps records
                for (int i = 0; i < gps.Count; i++)
                {
                    GlobalPoint globalPoint = new GlobalPoint
                    {
                        Latitude = GetLatitude(gps[i][1]),
                        Longitude = GetLongitude(gps[i][2])
                    };
                    Gps Gps = new Gps
                    {
                        TimeMoment = CalculateTime(gps[i][0], flightDate),
                        GlobalPoint = globalPoint,
                        Flight = newFlightRecord
                    };
                    _appDbContext.GlobalPoint.Add(globalPoint);
                    _appDbContext.Gps.Add(Gps);
                }
                _appDbContext.SaveChanges();
            }
        }
        private DateTime CalculateTime(string time, DateTime flightDate)
        {
            int hourdep = Int32.Parse(time.Substring(0, 2));
            int mindep = Int32.Parse(time.Substring(2, 2));
            int secdep = Int32.Parse(time.Substring(4, 2));
            return new DateTime(flightDate.Year, flightDate.Month, flightDate.Day, hourdep, mindep, secdep);
        }

        private string[] FindDepTimeAndPlace(string[] inspectorDep)
        {
            for (int i = 1; i < gps.Count - 1; i++)
            {
                if (gps[i][1].Equals(inspectorDep[1]) && gps[i][2].Equals(inspectorDep[2]) &&
                    gps[i - 1][1].Equals(inspectorDep[1]) && gps[i - 1][2].Equals(inspectorDep[2]) &&
                    !gps[i + 1][1].Equals(inspectorDep[1]) && !gps[i][2].Equals(inspectorDep[2]))
                {
                    inspectorDep[0] = gps[i][0];
                    inspectorDep[1] = gps[i][1];
                    inspectorDep[2] = gps[i][2];
                    break;
                }
            }
            return inspectorDep;
        }

        private string[] FindArrTimeAndPlace(string[] inspectorArr)
        {
            for (int i = 1; i < gps.Count - 1; i++)
            {
                if (gps[i][1].Equals(inspectorArr[1]) && gps[i][2].Equals(inspectorArr[2]) &&
                    !gps[i - 1][1].Equals(inspectorArr[1]) && !gps[i - 1][2].Equals(inspectorArr[2]) &&
                    gps[i + 1][1].Equals(inspectorArr[1]) && gps[i][2].Equals(inspectorArr[2]))
                {
                    inspectorArr[0] = gps[i][0];
                    inspectorArr[1] = gps[i][1];
                    inspectorArr[2] = gps[i][2];
                    break;
                }
            }
            return inspectorArr;
        }

        private float GetLatitude(string record)
        {
            double degreeLa = Convert.ToDouble(Int32.Parse(record.Substring(0, 2)));
            double minLaInt = Convert.ToDouble(Int32.Parse(record.Substring(2, 2)));
            double minLaFloat = Convert.ToDouble(Int32.Parse(record.Substring(4, 3))) / 1000.0;
            return (float)degreeLa + (float)((minLaInt + minLaFloat) / 60.0);
        }
        private float GetLongitude(string record)
        {
            double degreeLo = Convert.ToDouble(Int32.Parse(record.Substring(0, 3)));
            double minLoInt = Convert.ToDouble(Int32.Parse(record.Substring(3, 2)));
            double minLaFloat = Convert.ToDouble(Int32.Parse(record.Substring(5, 3))) / 1000.0;
            return (float)degreeLo + (float)(( minLoInt+minLaFloat) / 60.0);
        }

    }
}
