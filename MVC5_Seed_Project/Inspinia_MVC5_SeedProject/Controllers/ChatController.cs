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
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class ChatController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Chat
        public IQueryable<Chat> GetChats()
        {
            return db.Chats;
        }
        public async Task<IHttpActionResult> GetAllChatUsers()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = User.Identity.GetUserId();
                var users = from user in db.Chats
                          where user.sentTo.Equals(id) || user.sentFrom.Equals(id)
                          select new
                          {
                              id = user.sentTo.Equals(id)? user.AspNetUser.Id: user.AspNetUser1.Id,
                              name = user.sentTo.Equals(id) ? user.AspNetUser.Email: user.AspNetUser1.Email,
                              dpExtension = user.sentTo.Equals(id) ? user.AspNetUser.dpExtension: user.AspNetUser1.dpExtension,
                              lastMessage = user.message,
                              time = user.time
                          };
               var users1 = users.GroupBy(x => x.id).AsEnumerable().Select(x => x.LastOrDefault());
                return Ok(users1);
            }
            return BadRequest();
        }
        // GET api/Chat/5
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> GetChat(string with)
        {
            if(User.Identity.IsAuthenticated){
                var userId = User.Identity.GetUserId();
                var ret = from chat in db.Chats
                          where chat.sentTo.Equals(with) && chat.sentFrom.Equals(userId) || chat.sentTo.Equals(userId) && chat.sentFrom.Equals(with)
                          select new
                          {
                              id = chat.Id,
                              sentFrom = chat.sentFrom,
                              sentTo = chat.sentTo,
                              sentFromName = chat.AspNetUser1.Email,
                              sentToName = chat.AspNetUser.Email,
                              message = chat.message,
                              time = chat.time,
                              loginUserId = userId,
                              dpExtension = chat.AspNetUser.dpExtension
                          };
            return Ok(ret);
            }
            return BadRequest("Not login");
            
        }
        public async Task<IHttpActionResult> GetLoginUserId()
        {
            return Ok( User.Identity.GetUserId());
        }
        public async Task<IHttpActionResult> SearchUser(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            return Ok(chat);
        }
        // PUT api/Chat/5
        public async Task<IHttpActionResult> PutChat(int id, Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chat.Id)
            {
                return BadRequest();
            }

            db.Entry(chat).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Chat
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> PostChat(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (User.Identity.IsAuthenticated)
            {
                chat.sentFrom = User.Identity.GetUserId();
                if (chat.sentFrom == chat.sentTo)
                {
                    return BadRequest("You cannot send message to Yourself!");
                }
            }
            else
            {
                return BadRequest();
            }
            chat.time = DateTime.UtcNow;
            db.Chats.Add(chat);
            await db.SaveChangesAsync();
            var ret = from msg in db.Chats
                      where msg.Id.Equals(chat.Id)
                      select new
                      {
                          time = msg.time,
                          sentTo = msg.sentTo,
                          sentFrom = msg.sentFrom,
                          messgae = msg.message,
                      };
            return Ok(ret);
        }

        // DELETE api/Chat/5
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> DeleteChat(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            db.Chats.Remove(chat);
            await db.SaveChangesAsync();

            return Ok(chat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChatExists(int id)
        {
            return db.Chats.Count(e => e.Id == id) > 0;
        }
    }
}