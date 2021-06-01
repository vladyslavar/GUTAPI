using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.ViberProps
{
    public class SendMessage
    {
        public string receiver { get; set; }
        public string min_api_version { get; set; }
        public Sendr sender { get; set; }
        public string tracking_data { get; set; }
        public string type { get; set; }
        public string text { get; set; }
    }

    public class Sendr
    {
        public string name { get; set; }
        public string avatar { get;set;}
    }
}
