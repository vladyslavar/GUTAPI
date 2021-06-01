using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using GUTAPI.Controllers;
using System.Net.Http;
using System.Net;
using System.Text;
using GUTAPI.ViberProps;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GUTAPI.ViberAPI
{
    public class ViberActions
    {

        public async Task MessageToSendViber(SendMessage message)
        {
            string json = await Task.Run(() => JsonConvert.SerializeObject(message));
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Viber-Auth-Token", "4d4ed377c027dd04-9bc309fa5575c70c-f626e539881e42ab");
                
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");


                HttpResponseMessage response = await client.PostAsync("https://chatapi.viber.com/pa/send_message", new StringContent(json));
                var responseJson = await response.Content.ReadAsStringAsync();

            }
            
        }
        public async Task<HttpResponseMessage> GetInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Viber-Auth-Token", "4d4ed377c027dd04-9bc309fa5575c70c-f626e539881e42ab");
                var response = await client.GetAsync("https://chatapi.viber.com/pa/get_account_info");
                return response;
            }
            
        }

    }

    public class ViberReceivedMessages
    {
        public string Text { get; set; }
        public long Receiver { get; set; }
        public string Sender { get; set; }
    }

    public class ViberUs
    {
        public string Username { get; set; }
        public string Viber_id { get; set; }
    }

}
