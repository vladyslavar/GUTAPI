using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUTAPI.AuthorizeR.EventArgs
{
    public class AuthSuccessEventArgs
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
