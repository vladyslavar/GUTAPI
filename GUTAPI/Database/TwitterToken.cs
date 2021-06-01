using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class TwitterToken
    {
        public long id { get; set; }
        public long id_telegram_user { get; set; }
        public string consumer_key { get; set; }
        public string consumer_secret { get; set; }
        public string access_token { get; set; }
        public string access_token_secret { get; set; }
        public string twitter_user_id { get; set; }

    }
}
