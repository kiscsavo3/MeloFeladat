using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcRepules.Model
{
    public class PilotLog
    {
        public Guid PilotLogId { get; set; }
        public byte[] File { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }


    }
}
