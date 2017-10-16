using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dona.Forms.Exceptions
{
    public class CannotSendSmsException : Exception
    {
        public CannotSendSmsException(string message) : base(message)
        {
        }
    }
}
