using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class TwitterMessage
    {
        public long id_telegram_user { get; set; }
        public string last_message { get; set; }
    }
}
