using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class TwitterContact
    {
        public long id { get; set; }
        public long id_telegram_user { get; set; }
        public string id_twitter_contact { get; set; }
        public string username_twitter_contact { get; set; }
    }
}
