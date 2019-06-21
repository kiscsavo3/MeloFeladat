using MvcRepules.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcRepules.BLL.DTO
{
    public class AllFlightsDto : FlightsDto
    {

        [Display(Name ="Pilot Name")]
        public string PilotName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
