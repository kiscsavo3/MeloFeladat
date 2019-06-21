using MvcRepules.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcRepules.BLL.DTO
{
    public class FlightsDto
    {
        public Guid FlightId { get; set; }

        [Display(Name = "Flight Date")]
        [DataType(DataType.DateTime)]
        public DateTime FlightDate { get; set; }

        [Display(Name = "Takeoff Place")]
        public string TakeoffPlace { get; set; }

        [Display(Name = "Landing Place")]
        public string LandPlace { get; set; }

        [Display(Name = "Flight Time")]
        public TimeSpan FlightTime { get; set; }

        public Status Status { get; set; }
    }
}

