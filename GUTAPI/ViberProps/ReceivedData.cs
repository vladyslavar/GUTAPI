using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.ViberProps
{
    public class ReceivedData
    {
        public string Event { get; set; }
        public long Timespan { get; set; }
        public long MessageToken { get; set; }
        public Sender sender { get; set; }
        public Message message { get; set; }
        public string TrakingData { get; set; }

    }

    public class Message
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Media { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
    }

    public class Sender
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int Api_version { get; set; }
    }
}
