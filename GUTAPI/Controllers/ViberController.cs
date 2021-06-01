using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viber.Bot;
using System.Net;
using GUTAPI.ViberAPI;
using Viber.Bot.NetCore.RestApi;
using Viber.Bot.NetCore;
using Viber.Bot.NetCore.Infrastructure;
using Viber.Bot.NetCore.Models;
using GUTAPI.ViberProps;
using GUTAPI.Database;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace GUTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViberController : ControllerBase
    {
        CustomDBContext db;
        public ViberController(CustomDBContext db)
        {
            this.db = db;
            this.db.Database.EnsureCreated();
        }
        //cheked
        [HttpPost("post")]
        public async Task Post([FromBody] ReceivedData data)
        {
            string action = data.Event;
            string text;
            string senderId;
            string senderName;

            ViberActions viberActions = new ViberActions();
            ViberReceivedMessage viberMsgs;

            if (action == "message")
            {
                text = data.message.Text;
                senderId = data.sender.Id;
                senderName = data.sender.Name;
                long receiverId = 0;

                string pattern = @"(^[@]\w+)";
                Regex regex = new Regex(pattern);
                string receiver = regex.Match(text).Value;

                Regex regex2 = new Regex($"(^[@])");
                receiver = regex2.Replace(receiver, "");

                if (receiver == "")
                {
                    var user_to_check = db.VTcontacts.Find(senderId);
                    if (user_to_check != null)
                    { receiverId = user_to_check.last_tg_user; }
                    else { }
                }
                else
                {
                    var receiverId2 = db.TelegramUsers.AsQueryable().Where(p => p.username_telegram_user == receiver).FirstOrDefault();

                    if (receiverId2 != null)
                    {
                        receiverId = receiverId2.id_telegram_user;

                        var users_to_delete = db.VTcontacts.AsQueryable().Where(p => p.last_tg_user == receiverId);
                        foreach (var user_to_delete in users_to_delete)
                        {
                            db.VTcontacts.Remove(user_to_delete);
                            db.SaveChanges();

                        }

                        VTcontact vcontact = new VTcontact()
                        {
                            viber_id = senderId,
                            last_tg_user = receiverId
                        };
                        db.VTcontacts.Add(vcontact);
                        db.SaveChanges();

                    }
                    text = regex.Replace(text, "");
                }

                bool found = false;
                var tg_recIds = db.TelegramUsers.AsQueryable().Where(p => p.username_telegram_user == receiver);
                foreach (var tg_recId in tg_recIds)
                {
                    var tg_ids_in_vibercontact = db.ViberContacts.AsQueryable().Where(p => p.id_teegram_user == tg_recId.id_telegram_user);
                    foreach (var tg_id_vbc in tg_ids_in_vibercontact)
                    {
                        if (tg_id_vbc.id_viber_contact == senderId)
                        {
                            found = true;
                        }
                    }

                    if (found == false)
                    {
                        Database.ViberContact viberContact = new Database.ViberContact()
                        {
                            id_teegram_user = tg_recId.id_telegram_user,
                            id_viber_contact = senderId,
                            username_viber_contact = senderName
                        };
                        db.ViberContacts.Add(viberContact);
                        db.SaveChanges();

                    }
                }

                viberMsgs = new ViberReceivedMessage()
                {
                    Text = text,
                    Sender = senderName,
                    Receiver = receiverId
                };
                db.ViberReceivedMessages.Add(viberMsgs);
                db.SaveChanges();
            }
            else { }
        }

        [HttpPost("sendmessages")]
        public async Task SendMessages([FromBody] ViberSendMessage msg)
        {
            ViberActions viberActions = new ViberActions();
            string receiverId = "";
            if(msg.Receiver_id == "last")
            {
                var senderid = db.TelegramUsers.AsQueryable().Where(p => p.username_telegram_user == msg.Sender_username);
                if(senderid.Count() != 0)
                {
                    foreach (var sendId in senderid)
                    {
                        var user_to_receive = db.TVcontacts.Find(sendId.id_telegram_user);
                        if (user_to_receive != null)
                        { receiverId = user_to_receive.last_viber_user; }
                        else { }
                    }
                }
                else{}
            }
            else
            {
                
                var senderid = db.TelegramUsers.AsQueryable().Where(p => p.username_telegram_user == msg.Sender_username);
                foreach (var sendId in senderid)
                {
                    var user_to_delete = db.TVcontacts.Find(sendId.id_telegram_user);
                    if(user_to_delete != null)
                    {
                        db.TVcontacts.Remove(user_to_delete);
                        db.SaveChanges();
                    }
                    else { }
                    TVcontact vcontact = new TVcontact()
                    {
                        tg_id = sendId.id_telegram_user,
                        last_viber_user = msg.Receiver_id
                    };
                    db.TVcontacts.Add(vcontact);
                    db.SaveChanges();
                }
                receiverId = msg.Receiver_id;
            }
            SendMessage sendMessage = new SendMessage()
            {
                //my id: "i+ORIl2w8ykvlNJbCmctmw=="
                
                receiver = receiverId,
                min_api_version = "1",
                sender = new Sendr()
                {
                    name = msg.Sender_username,
                    avatar = ""
                },
                tracking_data = "tracking data",
                type = "text",
                text = msg.Message
            };
            await viberActions.MessageToSendViber(sendMessage);
        }

        [HttpGet("getmessage")]
        public List<ViberReceivedMessage> GetMessage()
        {
            List<ViberReceivedMessage> receivedMessages = new List<ViberReceivedMessage>();

            var receivedMsgs = db.ViberReceivedMessages.ToList();
            foreach (var message in receivedMsgs)
            {
                db.ViberReceivedMessages.Remove(message);
                receivedMessages.Add(message);
            }
            db.SaveChanges();
            return receivedMessages;
        }

        [HttpGet("getallusers/{tg_id}")]
        public List<ViberUs> GetAllUsers(long tg_id)
        {
            List<ViberUs> vibers = new List<ViberUs>();
            
            var users = db.ViberContacts.AsQueryable().Where(p => p.id_teegram_user == tg_id);
            foreach(var user in users)
            {
                ViberUs viberUs = new ViberUs()
                {
                    Username = user.username_viber_contact,
                    Viber_id = user.id_viber_contact,
                };
                vibers.Add(viberUs);
            }
            return vibers;
        }

    }
}
