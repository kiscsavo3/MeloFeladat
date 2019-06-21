using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcRepules.Model
{
    public class Airport
    {
        public Guid AirportId { get; set; }
        public string AirportName { get; set; }
        public Guid GlobalPointId { get; set; }
        public virtual GlobalPoint GlobalPoint { get; set; }
    }
}
