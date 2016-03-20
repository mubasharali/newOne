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
//using System.Web.Mvc;
using System.Web;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class CommentController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Comment
        public IQueryable<Comment> GetComments()
        {
            
            
            //var ret = from comment in db.Ads.Include(x => x.Comments).ToList()
            //          orderby comment.Comments ascending
            //          select new
            //          {

            //          };
            return db.Comments;
        }

        // GET api/Comment/5
        [ResponseType(typeof(Comment))]
        public async Task<IHttpActionResult> GetComment(int id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT api/Comment/5

        public async Task<IHttpActionResult> updateComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(comment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.Id))
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

        // POST api/Comment
        [ResponseType(typeof(Comment))]
        public async Task<IHttpActionResult> PostComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!( HttpContext.Current.Request.IsAuthenticated))
            {
               // return BadRequest();
                return BadRequest("you must be logged in post comment");
            }
            comment.time = DateTime.UtcNow;
            comment.postedBy = User.Identity.GetUserId();
            db.Comments.Add(comment);
            await db.SaveChangesAsync();
            //db.Comments.Include(x => x.AspNetUser).Select(x=>x.;

            var ret = db.Comments.Where(x => x.Id == comment.Id).Select(x => new
            {
                description = x.description,
                postedById = x.postedBy,
                postedByName = x.AspNetUser.Email,
                time = x.time,
                id = x.Id,
                adId = x.adId,
                imageExtension = x.AspNetUser.dpExtension,
                islogin = x.postedBy
            }).FirstOrDefault();

            
            return Ok(ret);
           // return CreatedAtRoute("DefaultApi", new { id = comment.Id }, comment);
        }
        [ResponseType(typeof(CommentReply))]
        public async Task<IHttpActionResult> PostCommentReply(CommentReply comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(HttpContext.Current.Request.IsAuthenticated))
            {
                return BadRequest();
            }
            comment.time = DateTime.UtcNow;
            comment.postedBy = User.Identity.GetUserId();
            db.CommentReplies.Add(comment);
            await db.SaveChangesAsync();
            var ret = db.CommentReplies.Where(x => x.Id == comment.Id).Select(x => new
            {
                description = x.description,
                postedById = x.postedBy,
                postedByName = x.AspNetUser.Email,
                time = x.time,
                imageExtension = x.AspNetUser.dpExtension,
                id = x.Id,
            }).FirstOrDefault();
            return Ok(ret);
        }
        public async Task<IHttpActionResult> updateCommentReply(CommentReply comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(comment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        // DELETE api/Comment/5
      //  [ResponseType(typeof(Comment))]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteComment(int id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Comments.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCommentReply(int id)
        {
            CommentReply comment = await db.CommentReplies.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.CommentReplies.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        public async Task<IHttpActionResult> LikeComment(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null) 
            { 
            Comment comment = await db.Comments.FindAsync(id);
            var commentVoteByUser = await db.CommentVotes.FirstOrDefaultAsync(x => x.commentId == id && x.votedBy == userId);
            if (comment == null)
            {
                return NotFound();
            }
            var vote = commentVoteByUser;
            if (vote != null)
            {
                if (vote.isup && isup)
                {
                    return BadRequest("You have already voteup");
                }
                else if (vote.isup && !isup)
                {
                    vote.isup = false;
                }
                else if (!vote.isup && !isup)
                {
                    return BadRequest("You have already votedown");
                }
                else if (!vote.isup && isup)
                {
                    vote.isup = true;
                }
            }
            else
            {
                CommentVote repvote = new CommentVote();
                repvote.isup = isup;
                repvote.votedBy = userId;
                repvote.commentId = id;
                db.CommentVotes.Add(repvote);
            }
            await db.SaveChangesAsync();

            var q = (from x in comment.CommentVotes.Where(x => x.commentId == comment.Id)
                     let voteUp = comment.CommentVotes.Count(m => m.isup)
                     let voteDown = comment.CommentVotes.Count(m => m.isup == false)
                     select new { voteUpCount = voteUp, voteDownCount = voteDown });

            return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> LikeCommentReply(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            CommentReply comment = await db.CommentReplies.FindAsync(id);
            var commentVoteByUser = await db.CommentReplyVotes.FirstOrDefaultAsync(x => x.commentReplyId == id && x.votedBy == userId);
            if (comment == null)
            {
                return NotFound();
            }
            var vote = commentVoteByUser;
            if(vote != null)
            {
                if (vote.isup && isup)
                {
                    return BadRequest("You have already voteup");
                }
                else if (vote.isup && !isup)
                {
                    vote.isup = false;
                }
                else if (!vote.isup && !isup)
                {
                    return BadRequest("You have already votedown");
                }
                else if(!vote.isup && isup)
                {
                    vote.isup = true;
                }
            }
            else
            {
                CommentReplyVote repvote = new CommentReplyVote();
                repvote.isup = isup;
                repvote.votedBy = userId;
                repvote.commentReplyId = id;
                db.CommentReplyVotes.Add(repvote);
            }
            await db.SaveChangesAsync();

            var q = (from x in comment.CommentReplyVotes.Where(x=>x.commentReplyId== comment.Id)
                    let voteUp = comment.CommentReplyVotes.Count(m => m.isup)
                    let voteDown = comment.CommentReplyVotes.Count(m => m.isup == false)
                    select new { voteUpCount = voteUp, voteDownCount = voteDown });
           
            return Ok(q);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(int id)
        {
            return db.Comments.Count(e => e.Id == id) > 0;
        }
    }
}