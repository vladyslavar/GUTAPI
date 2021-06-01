using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GUTAPI.Database;

namespace GUTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        CustomDBContext db;

        public TelegramController(CustomDBContext db)
        {
            this.db = db;
            this.db.Database.EnsureCreated();
        }
        //add user
        [HttpPost("adduser/{id}/{username}")]
        public void AddUser(long id, string username)
        {
            var user = db.TelegramUsers.Find(id);
            if(user == null)
            {
                TelegramUser tgUser = new TelegramUser()
                {
                    id_telegram_user = id,
                    username_telegram_user = username
                };

                db.TelegramUsers.Add(tgUser);
                db.SaveChanges();
            }
            else { }
        }
    }
}
