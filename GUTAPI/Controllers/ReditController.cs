using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GUTAPI.Redit;
using System.Net;
using System.IO;
using System.Net.Http;
using Reddit.Things;
using RestSharp;
using GUTAPI.Database;
using Newtonsoft.Json.Linq;

namespace GUTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReditController : ControllerBase
    {
        RedditAuthorize redditAuthorize = new RedditAuthorize();
        CustomDBContext db;
        private const string appId = "[tokens]";
        private const string appSecret = "[tokens]";
        public ReditController(CustomDBContext db)
        {
            this.db = db;
            this.db.Database.EnsureCreated();
        }
        //cheked
        [HttpGet("register")]
        public string Register(string state, string code)
        {
            string result;
            string _state = state;
            string _code = code;
            string acc_token = "ud";
            string secr_token = "ud";
            
            result = redditAuthorize.GetTokens(_code);

            JObject json = JObject.Parse(result);
            foreach (var e in json)
            {
                if (e.Key == "access_token")
                {
                    acc_token = e.Value.ToString();
                }
                if (e.Key == "refresh_token")
                {
                    secr_token = e.Value.ToString();
                    //var time = DateTime.Now.AddHours(1.0);
                    //token.time_existing = time.ToString();
                }
                else continue;
            }

                db.RedditTokens.Add(new RedditToken
                {
                    app_id = appId,
                    app_secret = appSecret,
                    access_token = acc_token,
                    refresh_token = secr_token,
                    time_existing = DateTime.Now.AddHours(1.0).ToString()
                }) ;
                db.SaveChanges();
            
            return acc_token;
        }
        //cheked
        [HttpGet("authorizeanoth")]
        public IActionResult AuthorizeAnoth()
        {
            try
            {
                string url = redditAuthorize.Authorize(appId, appSecret);
                return Redirect(url);
                
            }
            catch (Exception e)
            {
                var message = e.InnerException.Message;
                return StatusCode(500, $"An error has ocurred. Ex: {message}");
            }
        }
        //cheked
        [HttpPost("addtokens/{id_tg}/{accessToken}")]
        public void AddTokens(long id_tg, string accessToken)
        {
            bool change = false;
            
            var addUserToDB = db.RedditTokens.AsQueryable().Where(p => p.access_token == accessToken);
            foreach(var user in addUserToDB)
            {
                if(change == false)
                {
                    user.id_telegram_user = id_tg;
                    db.SaveChanges();
                    change = true;
                }
                else { break; }

            }
        }
        //cheked
        //check if user is in db and tokken has not expired
        [HttpGet("getmessage/{id_tg}")]
        public IEnumerable<RedditMes> GetMessage(long id_tg)
        {
            RedditToken redditData = db.RedditTokens.AsQueryable().Where(p=> p.id_telegram_user == id_tg).FirstOrDefault();
            if (redditData != null)
            {
                var res = DateTime.Compare(Convert.ToDateTime(redditData.time_existing), DateTime.Now);
                if (res > 0)
                {
                    ReditActions reditActions = new ReditActions();

                    reditActions.Run(redditData.app_id, redditData.refresh_token, redditData.access_token);
                    reditActions.GetMess(redditData.app_id, redditData.refresh_token, redditData.access_token);

                    if (reditActions.msg.Any())
                    {
                        foreach(var mes in reditActions.msg)
                        {
                            var reddit_users_in_db = db.RedditContacts.AsQueryable().Where(p => p.id_telegram_user == id_tg);
                            bool found = false;
                            
                            foreach(var user in reddit_users_in_db)
                            {
                                if (user.reddit_contact == mes.Author) found = true;
                            }
                            if(!found)
                            {
                                RedditContact RedditContact = new RedditContact()
                                {
                                    id_telegram_user = id_tg,
                                    reddit_contact = mes.Author
                                };
                                db.RedditContacts.Add(RedditContact);
                                db.SaveChanges();
                            }
                            
                        }

                        return reditActions.msg;
                    }
                    else return reditActions.msg;
                }
                else
                {
                    db.RedditTokens.Remove(redditData);
                    List<RedditMes> list = new List<RedditMes>();
                    RedditMes mes = new RedditMes()
                    {
                        Body = "TOKEN_EXPIRED",
                        Author = "",
                        Context = ""
                    };
                    list.Add(mes);
                    return list;
                }
            }
            else
            {
                List<RedditMes> list = new List<RedditMes>();
                RedditMes mes = new RedditMes()
                {
                    Body = "AUTHORIZE_FIRST",
                    Author = "",
                    Context = ""
                };
                list.Add(mes);
                return list;
            }
        }

        

        [HttpPost("sendmessage/{id_tg}/{subject}/{receiver}/{text}")]
        public void SendMessage(long id_tg, string subjest, string receiver, string text)
        {
            RedditToken redditData = db.RedditTokens.AsQueryable().Where(p => p.id_telegram_user == id_tg).FirstOrDefault();
            if (redditData != null)
            {
                var res = DateTime.Compare(Convert.ToDateTime(redditData.time_existing), DateTime.Now);
                if (res > 0)
                {
                    ReditActions reditActions = new ReditActions();

                    reditActions.SendMessage(redditData.app_id, redditData.refresh_token, redditData.access_token, subjest, receiver, text);
                }
            }
                    
        }

        //cheked
        [HttpGet("getallusers/{tg_id}")]
        public List<string> GetAllUsers(long tg_id)
        {
            List<string> redits = new List<string>();

            var users = db.RedditContacts.AsQueryable().Where(p => p.id_telegram_user == tg_id);
            foreach (var user in users)
            {
                redits.Add($"{user.reddit_contact}");
            }
            return redits;
        }
        
    }

}
