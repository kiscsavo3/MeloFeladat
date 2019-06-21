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

        //Ide szerintem már be kellene rakni a fel és leszállás repteret mint (opcionális) referenciát.
        // Idézet a doksiból: "A GPS log feldolgozása során – mint azt már valószínűleg kitaláltad 
        // – azért lesz szükség a repülőterek koordinátáira, mert ebből fogod tudni megállapítani a felszállás és leszállás helyét"

        // Szóval feldolgozás után már itt lenne a reptér, nem csak a GlobalPoint, így táblázat / részletes nézetnél
        // nem kellene már annyit számolni. 
        // (Igaz, az új reptér felvétele akkor a régi repülésekre nincs hatással, de az nem is életszerű)

    }
    public enum Status { Accepted, Denied, Waiting_For_Accept}
}
