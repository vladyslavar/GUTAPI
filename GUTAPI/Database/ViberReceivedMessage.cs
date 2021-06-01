using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class ViberReceivedMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public long Receiver { get; set; }
        public string Sender { get; set; }
    }
}
