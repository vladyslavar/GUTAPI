using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using System.Net;
using System.Web;

namespace GUTAPI.Twitter
{
    public class TwitterAuthorize
    {
        public string Authorize()
        {
            var client = new RestClient("https://api.twitter.com");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(
                "fGPc4QONQcctUxMv5ZHzVo1HA",
                "CduzTML5y3cKmLolA4NiVxgVQ58RB7lsissQcjT65jMtRk4I0X",
                "https://gutapi.ml/api/twitter/authorize"
                );
            var request = new RestRequest("/oauth/request_token", Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);

            string oauthToken = qs["oauth_token"];
            string oauthTokenSecret = qs["oauth_token_secret"];

            request = new RestRequest("oauth/authorize?oauth_token=" + oauthToken);

            string url = client.BuildUri(request).ToString();
            return url;
        }

        public IEnumerable<string> GetTokens(string key, string secret, string oauth_token, string oauth_token_secret, string oauth_verifier)
        {
            List<string> tokens = new List<string>();
            var request = new RestRequest("/oauth/access_token", Method.POST);

            var client = new RestClient("https://api.twitter.com");
            client.Authenticator = OAuth1Authenticator.ForAccessToken(key, secret, oauth_token, oauth_token_secret, oauth_verifier);

            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var qs = HttpUtility.ParseQueryString(response.Content);

                string oauthToken = qs["oauth_token"];
                string oauthTokenSecret = qs["oauth_token_secret"];
                string userId = qs["user_id"];
                string screenName = qs["screen_name"];
                string xAuthExpires = qs["x_auth_expires"];

                tokens.Add(oauthToken);
                tokens.Add(oauthTokenSecret);
                tokens.Add(userId);

                return tokens;
            }
            else
            {
                List<string> list = new List<string>();
                list.Add("error");
                return list;
            }
        }
        
    }
}
