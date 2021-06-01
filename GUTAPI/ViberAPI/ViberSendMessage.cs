using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.ViberAPI
{
    public class ViberSendMessage
    {
        public string Sender_username { get; set; } 
        public string Receiver_id { get; set; } 
        public string Message { get; set; }
    }
}
