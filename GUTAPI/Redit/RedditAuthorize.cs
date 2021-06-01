using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using RedditSharp;
using RedditSharp.Things;
using System.Text;
using Newtonsoft.Json;
using Reddit.Things;
using System.Threading;

namespace GUTAPI.Redit
{
    public class RedditAuthorize
    {
        public string Authorize(string appId, string appSecret)
        {
            var client = new RestClient("https://www.reddit.com");



            var request = new RestRequest("/api/v1/authorize?client_id=" + appId +
                "&response_type=code&state=" + appId + ":" + appSecret +
                "&redirect_uri=" + "https://gutapi.ml/api/redit/register" + "&duration=permanent&scope=" +
                "creddits%20modcontributors%20modmail%20modconfig%20subscribe%20structuredstyles%20vote%20wikiedit%20mysubreddits%20" +
                "submit%20modlog%20modposts%20modflair%20save%20modothers%20read%20privatemessages%20report%20identity%20livemanage%20" +
                "account%20modtraffic%20wikiread%20edit%20modwiki%20modself%20history%20flair");

            string url = client.BuildUri(request).ToString();

            return url;
        }

        public string GetTokens(string code)
        {
            List<string> tokens = new List<string>();

            RestClient rc = new RestClient();
            RestRequest request = new RestRequest("https://www.reddit.com/api/v1/access_token", Method.POST);

            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("XHgTbiabwFt54A:mNtnrSsUHBSUD23PrhFO7zQwfWfhSQ")));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", "https://gutapi.ml/api/redit/register");

            var response = rc.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var rs = response.Content.ToString();
                return rs;
            }
            else
            {
                return "list";
            }
        
        }

    }

    public class RedditResult
    {
        public string accessToken { get; set; }
        public string tokenType { get; set; }
        public string expiresIn { get; set; }
        public string scope { get; set; }
        public string refreshToken { get; set; } 
    }
       
}
