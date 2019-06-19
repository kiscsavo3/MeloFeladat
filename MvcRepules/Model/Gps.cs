using MvcRepules.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcRepules.Model
{
    public class Gps
    {
        public Guid GpsId { get; set; }
        public virtual GlobalPoint GlobalPoint { get; set; }
        public Guid GlobalPointId { get; set; }
        public DateTime TimeMoment { get; set; }
        public Guid FlightId { get; set; }
        public virtual Flight Flight { get; set; }

    }
}
