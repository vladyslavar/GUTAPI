using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class RedditToken
    {
        public long id_key { get; set; }
        public long id_telegram_user { get; set; }
        public string app_id { get; set; }
        public string app_secret { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string time_existing { get; set; }
    }
}
