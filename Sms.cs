using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailReceiver
{
    public class Sms
    {
        public string PhoneNumber { get; set; }
        public string Body { get; set; }
        public Address Address { get; set; }
        public DateTime Date { get; set; }

    }
}
