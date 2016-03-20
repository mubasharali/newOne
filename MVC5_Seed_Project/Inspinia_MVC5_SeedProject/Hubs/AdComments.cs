using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.Hubs
{
    public class AdComments : Hub
    {
        private Entities db = new Entities();
        public void AddComment()
        {
            Clients.All.hello();
        }
        public void UpdateComment()
        {
            Clients.All.hello();
        }
        public void DeleteComment()
        {
            Clients.All.hello();
        }
        public void AddCommentReply(CommentReply comment)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                comment.time = DateTime.UtcNow;
                comment.postedBy =Context.User.Identity.GetUserId();
                db.CommentReplies.Add(comment);
                db.SaveChanges();
                var ret = db.CommentReplies.Where(x => x.Id == comment.Id).Select(x => new
                {
                    description = x.description,
                    postedById = x.postedBy,
                    postedByName = x.AspNetUser.Email,
                    time = x.time,
                    imageExtension = x.AspNetUser.dpExtension,
                    id = x.Id,
                }).FirstOrDefault();
                Clients.Caller.appendCommentReplyToMe(ret);
                Clients.Others.appendCommentReply(ret);
            }
        }
        public void AddComment(Comment comment)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                comment.time = DateTime.UtcNow;
                comment.postedBy = Context.User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();
                var ret = db.Comments.Where(x => x.Id == comment.Id).Select(x => new
                {
                    description = x.description,
                    postedById = x.postedBy,
                    postedByName = x.AspNetUser.Email,
                    time = x.time,
                    imageExtension = x.AspNetUser.dpExtension,
                    id = x.Id,
                }).FirstOrDefault();
                Clients.Caller.AppendCommentToMe(ret);
                Clients.Others.appendComment(ret);
            }
        }
    }
}