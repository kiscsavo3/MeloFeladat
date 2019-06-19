using MvcRepules.DAL;
using MvcRepules.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcRepules.BLL.DTO
{
    public class MyFlightsDto
    {
        public DateTime FlightDate { get; set; }
        public string TakeoffPlace { get; set; }
        public string LandPlace { get; set; }
        public TimeSpan FlightTime { get; set; }
        public Status Status { get; set; }

    }
}
