//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.Identity;
//using Inspinia_MVC5_SeedProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class ChatHub : Hub
    {
        Entities db = new Entities();

        //start
        public override Task OnConnected()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                string name = Context.User.Identity.GetUserId();
                Groups.Add(Context.ConnectionId, name);
            }
            return base.OnConnected();
        }
        public async Task<bool> SeenAt(int id)
        {
            var msg =await db.Chats.FindAsync(id);
            msg.SeenAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            Clients.Group(msg.sentTo).MessageSeen(id,msg.SeenAt);
          //  Clients.Group(msg.sentFrom).MessageSeen(id, msg.SeenAt);
            return true;
        }
        //end
        public void AddMessage(Chat msg)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var userId = Context.User.Identity.GetUserId();
                msg.time = DateTime.UtcNow;
                msg.sentFrom = Context.User.Identity.GetUserId();
                db.Chats.Add(msg);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                var ret = (from chat in db.Chats
                          where chat.Id.Equals(msg.Id)
                          select new
                          {
                              id = chat.Id,
                              sentFrom = chat.sentFrom,
                              sentTo = chat.sentTo,
                              sentFromName = chat.AspNetUser1.Email,
                              sentToName = chat.AspNetUser.Email,
                              message = chat.message,
                              time = chat.time,
                              dpExtension = chat.AspNetUser.dpExtension
                          }).FirstOrDefault();
               // var ret = db.Chats.FirstOrDefault(x => x.Id == msg.Id);
                Clients.Group(msg.sentFrom).loadNewMessage(ret);
                Clients.Group(msg.sentTo).loadNewMessage(ret);
            }
           // Clients.Caller.loadNewMessage(ret);
          //  Clients.All.loadNewMessage(ret);
        }
    }
}