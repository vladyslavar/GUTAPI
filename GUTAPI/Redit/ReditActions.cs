using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reddit;
using Reddit.Controllers.EventArgs;
using Reddit.Things;
using System.IO;
using System.Reflection;
using Reddit.Controllers;
using System.Net;
using System.Text;
using System.Collections;
using Reddit.Inputs.PrivateMessages;
using System.Threading;

namespace GUTAPI.Redit
{
    public class ReditActions
    {
        public List<RedditMes> msg = new List<RedditMes>();
        //private string AppId = "XHgTbiabwFt54A";
        //private string AppSecret = "mNtnrSsUHBSUD23PrhFO7zQwfWfhSQ";
        //private string RefreshToken;


        public void Run(string Appid, string Refreshtoken, string AccessToken)
        {
            string appId = Appid;
            string refreshToken = Refreshtoken;
            string accessToken = AccessToken;
            RedditClient reddit = new RedditClient(
                appId: appId,
                refreshToken: refreshToken, 
                appSecret: "mNtnrSsUHBSUD23PrhFO7zQwfWfhSQ", 
                accessToken: accessToken);

            Reddit.Controllers.User me = reddit.Account.Me;
            string name = me.Name;
            var message = reddit.Account.Messages.GetMessagesInbox();
            
        }
        public List<ConversationMessage> NewMessages;
        public List<RedditMes> GetMess(string Appid, string Refreshtoken, string AccessToken)
        {
            msg.Clear();
            string appId = Appid;
            string refreshToken = Refreshtoken;
            string accessToken = AccessToken;
            RedditClient reddit = new RedditClient(appId: appId, refreshToken: refreshToken, accessToken: accessToken);
            var karma = reddit.Account.Karma();

            var messages = reddit.Account.Messages.GetMessagesUnread();
            var message2 = reddit.Models.PrivateMessages.GetMessages("unread", new PrivateMessagesGetMessagesInput(false, count: 10));

            foreach (var message in messages)
            {
                reddit.Models.PrivateMessages.ReadMessage(message.Name);

                RedditMes mes = new RedditMes()
                {
                    Body = message.Body,
                    Author = message.Author,
                    Context = message.Context
                };
                msg.Add(mes);
            }
            return msg;
        }

        
        public void SendMessage(string Appid, string Refreshtoken, string AccessToken, string subj, string receiver, string message)
        {
            RedditClient reddit = new RedditClient(appId: Appid, refreshToken: Refreshtoken, accessToken: AccessToken);
            Reddit.Controllers.User me = reddit.Account.Me;

            reddit.Models.PrivateMessages.Compose(
                new PrivateMessagesComposeInput(subject: subj, text: message, to: receiver, fromSr: me.Name));

        }
    }
    public class RedditMes
    {
        public string Body { get; set; }
        public string Author { get; set; }
        public string Context { get; set; }
    }
    
}
