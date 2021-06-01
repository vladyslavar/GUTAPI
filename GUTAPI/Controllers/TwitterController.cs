using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using GUTAPI.Twitter;
using Tweetinvi;
using Tweetinvi.Auth;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using System.Net.Http;
using GUTAPI.Database;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace GUTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        IAuthenticationRequestStore _myAuthRequestStore = new LocalAuthenticationRequestStore();
        TwitterAuthorize twitterAuthorize = new TwitterAuthorize();
        CustomDBContext db;


        public TwitterController(CustomDBContext db)
        {
            this.db = db;
            this.db.Database.EnsureCreated();
        }

        [HttpGet("authorize")]
        public string Authorize(string oauth_token, string oauth_verifier)
        {
            IEnumerable<string> result = new List<string>();
            string _oauth_token = oauth_token;
            string _oauth_verifier = oauth_verifier;
            result = twitterAuthorize.GetTokens(
                "[token]",
                "[token]",
                oauth_token,
                "",
                oauth_verifier);
            var res = result.ToList();

            var items = db.TwitterTokens.AsQueryable().Where(p => p.access_token == res[0]);
            foreach(var item in items)
            {
                if (db.TwitterTokens.Contains(item))
                {
                    db.TwitterTokens.Remove(item);
                }
                db.SaveChanges();
            }
            TwitterToken token = new TwitterToken()
            {
                consumer_key = "[token]",
                consumer_secret = "[token]",
                access_token = res[0],
                access_token_secret = res[1],
                twitter_user_id = res[2]
            };
            db.TwitterTokens.Add(token);
            db.SaveChanges();

            //return result;
            return res[0];
        }

        [HttpGet("authorizeanoth")]
        public IActionResult AuthorizeAnoth()
        {
            string url = twitterAuthorize.Authorize();
            return Redirect(url);
        }

        [HttpPost("addtokens/{id_tg}/{access_token}")]
        public void AddTokens(long id_tg, string access_token)
        {
            TwitterToken token = new TwitterToken()
            {
                consumer_key = "[token]",
                consumer_secret = "[token]",
                access_token = "123",
                access_token_secret = "123",
                twitter_user_id = "12345"
            };
            db.TwitterTokens.Add(token);
            db.SaveChanges();

            bool change = false;
            var addUser = db.TwitterTokens.AsQueryable().Where(p => p.access_token == access_token);
            foreach (var user in addUser)
            {
                if (change == false)
                {
                    user.id_telegram_user = id_tg;
                    db.SaveChanges();
                    change = true;
                }
                else { break; }
            }
            
        }

        [HttpGet("getmessages/{id_receiver_tg}")]
        public async Task<IEnumerable<ReceivedMessages>> GetMessage(long id_receiver_tg)
        {
            

            List<ReceivedMessages> msg = new List<ReceivedMessages>();
            msg.Clear();

            var last_message_to_check = db.TwitterMessages.Find(id_receiver_tg);

            var twitterToken = db.TwitterTokens.AsQueryable().Where(p=>p.id_telegram_user==id_receiver_tg).FirstOrDefault();
            string id_user_twitter = twitterToken.twitter_user_id;


            //TwitterToken twitterToken = db.TwitterTokens.Find(id_receiver_tg);
            if (twitterToken != null)
            {
                TwitterActions twitterActions = new TwitterActions();
                IMessage[] messages = await twitterActions.MessageGet
                    (twitterToken.access_token,
                    twitterToken.access_token_secret);

                ReceivedMessages receivedMessages;
                //check for last message
                //add contacts
                foreach (var message in messages)
                {
                    string mes1;
                    if (last_message_to_check == null) mes1 = "";
                    else mes1 = last_message_to_check.last_message;
                    var mes2 = message.Text;
                    if (mes1 != mes2) { }
                    else break;

                    var tg_users_in_db = db.TwitterContacts.AsQueryable().Where(p => p.id_telegram_user == id_receiver_tg);
                    bool check = false;
                    foreach (var tg_user_in_db in tg_users_in_db)
                    {
                        if (tg_user_in_db.id_telegram_user == id_receiver_tg) { check = true; break; }
                        else { }
                    }
                    if (check == false)
                    {
                        TwitterContact twitterContact = new TwitterContact()
                        {
                            id_telegram_user = id_receiver_tg,
                            id_twitter_contact = message.SenderId.ToString(),
                            username_twitter_contact = ""
                        };
                        db.TwitterContacts.Add(twitterContact);
                        db.SaveChanges();
                    }

                    if (message.SenderId.ToString() != id_user_twitter)
                    {
                        receivedMessages = new ReceivedMessages()
                        {
                            Text = message.Text,
                            Sender_id = message.SenderId.ToString(),
                            Receiver_id = id_receiver_tg.ToString(),
                        };
                        msg.Add(receivedMessages);
                    }
                }
                if (msg.Count() != 0)
                {
                    msg.Reverse();

                    var lastMessage = msg.Last();
                    TwitterMessage twitterMessage = new TwitterMessage()
                    {
                        id_telegram_user = Convert.ToInt64(lastMessage.Receiver_id),
                        last_message = lastMessage.Text
                    };
                    var user_with_message = db.TwitterMessages.Find(id_receiver_tg);
                    if (user_with_message != null)
                        db.TwitterMessages.Remove(user_with_message);

                    db.TwitterMessages.Add(twitterMessage);
                    db.SaveChanges();;
                    return msg;
                }
                return msg;

            }
            else
            {
                ReceivedMessages messages = new ReceivedMessages()
                {
                    Text = "AUTHORIZE_FIRST",
                    Receiver_id = id_receiver_tg.ToString(),
                    Sender_id = "1"
                };
                msg.Add(messages);
                return msg;
            }
        }

        [HttpPost("sendmessages/{id_sender_tg}/{id_receiver_twitter}/{text}")]
        public async Task<ObjectResult> SendMessage(long id_sender_tg, string id_receiver_twitter, string text)
        {
            try
            {
                TwitterToken twitterToken = db.TwitterTokens.AsQueryable().Where(p => p.id_telegram_user == id_sender_tg).FirstOrDefault();
                if (twitterToken != null)
                {
                    TwitterActions twitterActions = new TwitterActions();
                    await twitterActions.MessageSend(text, id_receiver_twitter,
                        twitterToken.access_token,
                        twitterToken.access_token_secret);
                    return StatusCode(200, $"okey");
                }
                else { return StatusCode(200, $"okey"); }
            }
            catch (Exception e)
            {
                var message = e.InnerException.Message;
                return StatusCode(500, $"An error has ocurred. Ex: {message}");
            }
        }

       
        
        [HttpGet("getallusers/{tg_id}")]
        public List<TwitterContact> GetAllUsers(long tg_id)
        {
            
            List<TwitterContact> twitters = new List<TwitterContact>();

            var users = db.TwitterContacts.AsQueryable().Where(p => p.id_telegram_user == tg_id);
            
            foreach(var user in users)
            {
                twitters.Add(user);
            }
            
            return twitters;
        }
    }
}
