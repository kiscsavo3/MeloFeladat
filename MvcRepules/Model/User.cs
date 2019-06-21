using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvcRepules.Model
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            PilotLogs = new HashSet<PilotLog>();
            Flights = new HashSet<Flight>();
        }

        // Nyugodtan lehet minden tábla kulcsa sima Id, úgy is felismeri az EF
        public Guid UserId { get; set; }

        [DefaultValue(false)]
        /// Inkább legyen sima bool 
        public Boolean IsAdmin { get; set; }

        [DefaultValue(false)]
        public Boolean IsPilot { get; set; }
        public virtual ICollection<PilotLog> PilotLogs { get; set; }
        public virtual ICollection<Flight> Flights { get; set; }

    }
}
