using Model;
using System;

namespace MvcRepules.Model
{
    public class Flight
    {
        public Guid FlightId { get; set; }
        public DateTime FlightDate { get; set; }
        public Guid TakeoffPlaceId { get; set; }
        public virtual GlobalPoint TakeoffPlace { get; set; }
        public Guid LandPlaceId { get; set; }
        public virtual GlobalPoint LandPlace { get; set; }
        public TimeSpan FlightTime { get; set; }
        public Status Status { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

    }
    
    public enum Status { Accepted, Denied, Waiting_For_Accept}
}
