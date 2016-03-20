using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

using Microsoft.AspNet.Identity;

using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class User
    {
        public string id;
        public string name;
        public string dpExtension;
    }
    public class Map
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public List<string> Connections { get; set; }
    }
    public class OnlineUsers : Hub
    {
        public static readonly List<Map> maps = new List<Map>();
        private Entities db = new Entities();
        public override System.Threading.Tasks.Task OnConnected()
        {
            User u = new User();
            u.id = "visitor";
            u.name = "Visitor";
            u.dpExtension = "";
            if (Context.User.Identity.IsAuthenticated)
            {
                u.id = Context.User.Identity.GetUserId();
                var da = db.AspNetUsers.Find(u.id);
                u.name = da.Email;
                u.dpExtension = da.dpExtension;
            }
            Map data = maps.FirstOrDefault(t => t.UserId == u.id);
            if (data == null)
            {
                maps.Add(new Map()
                {
                    UserId = u.id,
                    User = u,
                    Connections = new List<string>() { Context.ConnectionId }
                });
            }
            else
            {
                data.Connections.Add(Context.ConnectionId);
            }

            Clients.All.showConnected(maps.Select(m => m.User));
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var map = maps.FirstOrDefault(t => t.Connections.Contains(Context.ConnectionId));
            if (map != null)
            {
                map.Connections.Remove(Context.ConnectionId);
                if (map.Connections.Count <= 0)
                {
                    maps.Remove(map);
                }
            }

            Clients.All.showConnected(maps.Select(m => m.User));
            return base.OnDisconnected(stopCalled);
        }
    }
}