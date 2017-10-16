using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dona.Forms.Events
{
    public class CreditInformationReceivedEvent
    {
        public string Number { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
