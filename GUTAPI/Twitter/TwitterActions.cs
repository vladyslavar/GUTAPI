using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Tweetinvi;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace GUTAPI.Twitter
{
    public class TwitterActions
    {
        public async Task<IMessage[]> MessageGet(string access_token, string access_token_secret)
        {
            var userCredentials = new TwitterCredentials
                ("fGPc4QONQcctUxMv5ZHzVo1HA", 
                "CduzTML5y3cKmLolA4NiVxgVQ58RB7lsissQcjT65jMtRk4I0X", 
                access_token, 
                access_token_secret);
            var userClient = new TwitterClient(userCredentials);
            var messages = await userClient.Messages.GetMessagesAsync();
            return messages;
        }

        public async Task MessageSend(string text, string receiver, string access_token, string access_token_secret)
        {
            var userCredentials = new TwitterCredentials
                ("fGPc4QONQcctUxMv5ZHzVo1HA",
                "CduzTML5y3cKmLolA4NiVxgVQ58RB7lsissQcjT65jMtRk4I0X",
                access_token,
                access_token_secret);
            var userClient = new TwitterClient(userCredentials);
            await userClient.Messages.PublishMessageAsync(text, Convert.ToInt64(receiver));
        }

    }
}
