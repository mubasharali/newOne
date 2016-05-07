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
    public class ForumController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Forum
        public IQueryable<Question> GetQuestions()
        {
            return db.Questions;
        }
        public async Task<IHttpActionResult> SearchQuestions(string title, string tags, string category)
        {
            
            if (title != null && tags != null && category != null)
            {
                var ret = from q in db.Questions
                          where q.QuestionTags.Any(x => x.Tag.name.Equals(tags))
                          where q.title.Contains(title)
                          where q.category.Equals(category)
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else if (title != null && category != null)
            {
                var ret = from q in db.Questions
                          where q.title.Contains(title)
                          where q.category.Equals(category)
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else if (tags != null && category != null)
            {
                var ret = from q in db.Questions
                          where q.QuestionTags.Any(x => x.Tag.name.Equals(tags))
                          where q.category.Equals(category)
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else if (title != null )
            {
                var ret = from q in db.Questions
                          where q.title.Contains(title)
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else if (category != null)
            {
                var ret = from q in db.Questions
                          where q.category.Equals(category)
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else if (tags != null)
            {
                var ret = from q in db.Questions
                          where q.QuestionTags.Any(x => x.Tag.name.Equals(tags))
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
            else
            {
                var ret = from q in db.Questions
                          orderby q.time descending
                          select new
                          {
                              title = q.title,
                              id = q.Id,
                              views = q.views,
                              postedById = q.postedBy,
                              postedByName = q.AspNetUser.Email,
                              voteUpCount = q.QuestionVotes.Count(x => x.isUp),
                              voteDownCount = q.QuestionVotes.Count(x => x.isUp == false),
                              time = q.time,
                              answers = q.Answers.Count,
                              tags = from tag in q.QuestionTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name,
                                     }
                          };
                return Ok(ret);
            }
        }
        // GET api/Forum/5
        [ResponseType(typeof(Question))]
        public async Task<IHttpActionResult> GetQuestion(int id)
        {
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            if (question.views == null)
            {
                question.views = 0;
            }
            question.views++;
            db.Entry(question).State = EntityState.Modified;
            await db.SaveChangesAsync();
            //await QuestionViews(id);
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            var ret = from q in db.Questions
                      where q.Id == id
                      select new
                      {
                          title = q.title,
                          description = q.description,
                          id = q.Id,
                          postedById = q.AspNetUser.Id,
                          postedByName = q.AspNetUser.Email,
                          time = q.time,
                          islogin = islogin,
                          views = q.views,
                          reportedCount = q.ReportedQuestions.Count,
                          isReported = q.ReportedQuestions.Any(x=>x.reportedBy == islogin),
                          voteUpCount = q.QuestionVotes.Where(x=>x.isUp == true).Count(),
                          voteDownCount = q.QuestionVotes.Where(x => x.isUp == false).Count(),
                          isUp = q.QuestionVotes.Any(x=>x.votedBy == islogin && x.isUp),
                          isDown = q.QuestionVotes.Any(x=>x.votedBy == islogin && x.isUp == false),
                          isFollowed = q.FollowQuestions.Any(x=>x.followedBy == islogin),
                          questionTags = from tag in q.QuestionTags.ToList()
                                         select new{
                                             id = tag.tagId,
                                             name = tag.Tag.name,
                                             //info = tag.Tag.info,
                                         },
                          questionReplies = from reply in q.QuestionReplies.ToList()
                                            select new
                                            {
                                                id = reply.Id,
                                                description = reply.description,
                                                postedById = reply.AspNetUser.Id,
                                                postedByName = reply.AspNetUser.Email,
                                                time = reply.time,
                                                voteUpCount = reply.QuestionReplyVotes.Where(x=>x.isUp).Count(),
                                                voteDownCount = reply.QuestionReplyVotes.Where(x=>x.isUp == false).Count(),
                                                isUp = reply.QuestionReplyVotes.Any(x =>x.votedBy == islogin && x.isUp),
                                                isDown = reply.QuestionReplyVotes.Any(x=>x.votedBy == islogin && x.isUp == false),
                                            },
                          answers = from ans in q.Answers.ToList()
                                    select new
                                    {
                                        id = ans.Id,
                                        description = ans.description,
                                        postedByName = ans.AspNetUser.Email,
                                        postedById = ans.AspNetUser.Id,
                                        time = ans.time,
                                        voteUpCount = ans.AnswerVotes.Where(x => x.isUp == true).Count(),
                                        voteDownCount = ans.AnswerVotes.Where(x => x.isUp == false).Count(),
                                        isUp = ans.AnswerVotes.Any(x => x.votedBy == islogin && x.isUp),
                                        isDown = ans.AnswerVotes.Any(x => x.votedBy == islogin && x.isUp == false),
                          
                                        answerReplies = from rep in ans.AnswerReplies.ToList()
                                                        select new
                                                        {
                                                            id = rep.Id,
                                                            description = rep.description,
                                                            postedByName = rep.AspNetUser.Email,
                                                            postedById = rep.AspNetUser.Id,
                                                            time = rep.time,
                                                            voteUpCount = rep.AnswerReplyVotes.Where(x => x.isUp).Count(),
                                                            voteDownCount = rep.AnswerReplyVotes.Where(x => x.isUp == false).Count(),
                                                            isUp = rep.AnswerReplyVotes.Any(x => x.votedBy == islogin && x.isUp),
                                                            isDown = rep.AnswerReplyVotes.Any(x => x.votedBy == islogin && x.isUp == false),
                                            
                                                        }
                                    }
                      };
            return Ok(ret);
        }
        
        //public async Task<IHttpActionResult> GetMobileCategoriesCount()
        //{
        //    var data =await (from q in db.Questions
        //               where q.subCategory.Equals("Mobiles")
        //               select new{
        //                   NokiaCount = q.lowCategory.Count(x=>x.Equals("Nokia")),
        //                   SamsungCount = q.lowCategory.Count(x=>x.Equals("Samsung"))
        //               }).FirstOrDefaultAsync();
        //    return Ok(data);
        //}
        //public async Task<IHttpActionResult> QuestionViews(int id)
        //{
        //    Question ad = await db.Questions.FindAsync(id);
        //    if (ad == null)
        //    {
        //        return NotFound();
        //    }
        //    var userId = User.Identity.GetUserId();
        //    if (userId != null)
        //    {
        //        var isAlreadyViewed = ad.QuestionViews.Any(x => x.viewedBy == userId);
        //        if (isAlreadyViewed)
        //        {
        //            return Ok();
        //        }
        //        QuestionView rep = new QuestionView();
        //        rep.viewedBy = userId;
        //        rep.questionId = id;
        //        db.QuestionViews.Add(rep);
        //        await db.SaveChangesAsync();

        //        return Ok();
        //    }
        //    else
        //    {
        //        string ip = GetIPAddress();
        //        var isAlreadyViewed = ad.QuestionViews.Any(x => x.viewedBy == ip);
        //        if (isAlreadyViewed)
        //        {
        //            return Ok();
        //        }
        //        QuestionView rep = new QuestionView();
        //        rep.viewedBy = ip;
        //        rep.questionId = id;
        //        //if (rep.viewedBy == "::1")
        //        //{
        //        //    rep.viewedBy = "abcdef4264";
        //        //}
        //        db.QuestionViews.Add(rep);
        //        try { 
        //        await db.SaveChangesAsync();
        //        }
        //        catch (Exception e)
        //        {
        //            string s = e.ToString();
        //        }
        //        return Ok();
        //    }

        //}
        // PUT api/Forum/5
        public async Task<IHttpActionResult> PutQuestion(int id, Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != question.Id)
            {
                return BadRequest();
            }

            db.Entry(question).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
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

        // POST api/Forum
        [ResponseType(typeof(Question))]
        public async Task<IHttpActionResult> PostQuestion(Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            question.time = DateTime.UtcNow;
            question.postedBy = User.Identity.GetUserId();
            db.Questions.Add(question);
            await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { id = question.Id }, question);
        }

        // DELETE api/Forum/5
        [HttpPost]
        public async Task<IHttpActionResult> DeleteQuestion(int id)
        {
            Question question = await db.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            db.Questions.Remove(question);
            await db.SaveChangesAsync();

            return Ok("Done");
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnswer(int id)
        {
            Answer comment = await db.Answers.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Answers.Remove(comment);
            try { 
            await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
            return Ok(comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteQuestionReply(int id)
        {
            QuestionReply comment = await db.QuestionReplies.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.QuestionReplies.Remove(comment);
            try { 
            await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
            return Ok(comment);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnswerReply(int id)
        {
            AnswerReply comment = await db.AnswerReplies.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.AnswerReplies.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        public async Task<IHttpActionResult> PostQuestionReply(QuestionReply question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.QuestionReplies.Add(question);
                await db.SaveChangesAsync();
                var ret = db.QuestionReplies.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.Email,
                    time = x.time
                }).FirstOrDefault();
                return Ok(ret);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateQuestionReply(QuestionReply comment)
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

        public async Task<IHttpActionResult> PostAnswerReply(AnswerReply question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.AnswerReplies.Add(question);
                await db.SaveChangesAsync();
                var ret =await db.AnswerReplies.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.Email,
                    time = x.time
                }).FirstOrDefaultAsync();
                return Ok(ret);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateAnswerReply(AnswerReply comment)
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
        public async Task<IHttpActionResult> PostAnswer(Answer question)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state");
                }
                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.Answers.Add(question);
                await db.SaveChangesAsync();
                var ret = db.Answers.Where(x => x.Id == question.Id).Select(x => new
                {
                    id = x.Id,
                    description = x.description,
                    postedById = x.AspNetUser.Id,
                    postedByName = x.AspNetUser.Email,
                    time = x.time
                }).FirstOrDefault();
                return Ok(ret);
            }
            else
            {
                return BadRequest("Not login");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateAnswer(Answer comment)
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
        [HttpPost]
        public async Task<IHttpActionResult> ReportQuestion(int id)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Question ad = await db.Questions.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.ReportedQuestions.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report a Question only once.If something is really wrong you can contact us");
                }
                ReportedQuestion rep = new ReportedQuestion();
                rep.reportedBy = userId;
                rep.questionId = id;
                db.ReportedQuestions.Add(rep);
                await db.SaveChangesAsync();

                var count = ad.ReportedQuestions.Count;

                return Ok(count);
            }
            else
            {
                return BadRequest("Not login");
            }
        }
        public async Task<IHttpActionResult> VoteQuestion(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Question comment = await db.Questions.FindAsync(id);
                var commentVoteByUser = await db.QuestionVotes.FirstOrDefaultAsync(x => x.questionId == id && x.votedBy == userId);
                if (comment == null)
                {
                    return NotFound();
                }
                var vote = commentVoteByUser;
                if (vote != null)
                {
                    if (vote.isUp && isup)
                    {
                        return BadRequest("You have already voteup");
                    }
                    else if (vote.isUp && !isup)
                    {
                        vote.isUp = false;
                    }
                    else if (!vote.isUp && !isup)
                    {
                        return BadRequest("You have already votedown");
                    }
                    else if (!vote.isUp && isup)
                    {
                        vote.isUp = true;
                    }
                }
                else
                {
                    QuestionVote repvote = new QuestionVote();
                    repvote.isUp = isup;
                    repvote.votedBy = userId;
                    repvote.questionId = id;
                    db.QuestionVotes.Add(repvote);
                }
                await db.SaveChangesAsync();

                var q = (from x in comment.QuestionVotes.Where(x => x.questionId == comment.Id)
                         let voteUp = comment.QuestionVotes.Count(m => m.isUp)
                         let voteDown = comment.QuestionVotes.Count(m => m.isUp == false)
                         select new { voteCount = voteUp - voteDown }).FirstOrDefault();

                return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> VoteAnswer(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Answer comment = await db.Answers.FindAsync(id);
                var commentVoteByUser = await db.AnswerVotes.FirstOrDefaultAsync(x => x.answerId == id && x.votedBy == userId);
                if (comment == null)
                {
                    return NotFound();
                }
                var vote = commentVoteByUser;
                if (vote != null)
                {
                    if (vote.isUp && isup)
                    {
                        return BadRequest("You have already voteup");
                    }
                    else if (vote.isUp && !isup)
                    {
                        vote.isUp = false;
                    }
                    else if (!vote.isUp && !isup)
                    {
                        return BadRequest("You have already votedown");
                    }
                    else if (!vote.isUp && isup)
                    {
                        vote.isUp = true;
                    }
                }
                else
                {
                    AnswerVote repvote = new  AnswerVote();
                    repvote.isUp = isup;
                    repvote.votedBy = userId;
                    repvote.answerId = id;
                    db.AnswerVotes.Add(repvote);
                }
                await db.SaveChangesAsync();

                var q = (from x in comment.AnswerVotes.Where(x => x.answerId == comment.Id)
                         let voteUp = comment.AnswerVotes.Count(m => m.isUp)
                         let voteDown = comment.AnswerVotes.Count(m => m.isUp == false)
                         select new { voteUpCount = voteUp, voteDownCount = voteDown }).FirstOrDefault();

                return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> VoteQuestionReply(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                QuestionReply comment = await db.QuestionReplies.FindAsync(id);
                var commentVoteByUser = await db.QuestionReplyVotes.FirstOrDefaultAsync(x => x.questionReplyId == id && x.votedBy == userId);
                if (comment == null)
                {
                    return NotFound();
                }
                var vote = commentVoteByUser;
                if (vote != null)
                {
                    if (vote.isUp && isup)
                    {
                        return BadRequest("You have already voteup");
                    }
                    else if (vote.isUp && !isup)
                    {
                        vote.isUp = false;
                    }
                    else if (!vote.isUp && !isup)
                    {
                        return BadRequest("You have already votedown");
                    }
                    else if (!vote.isUp && isup)
                    {
                        vote.isUp = true;
                    }
                }
                else
                {
                    QuestionReplyVote repvote = new  QuestionReplyVote();
                    repvote.isUp = isup;
                    repvote.votedBy = userId;
                    repvote.questionReplyId = id;
                    db.QuestionReplyVotes.Add(repvote);
                }
                await db.SaveChangesAsync();

                var q = (from x in comment.QuestionReplyVotes.Where(x => x.questionReplyId == comment.Id)
                         let voteUp = comment.QuestionReplyVotes.Count(m => m.isUp)
                         let voteDown = comment.QuestionReplyVotes.Count(m => m.isUp == false)
                         select new { voteUpCount = voteUp, voteDownCount = voteDown }).FirstOrDefault();

                return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> Follow(int questionId)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                string s;
                Question q = await db.Questions.FindAsync(questionId);
                var followed = q.FollowQuestions.FirstOrDefault(x => x.followedBy == userId);
                if (followed != null)
                {
                    db.FollowQuestions.Remove(followed);
                    db.SaveChanges();
                    s = "Follow";
                    return Ok(s);
                }
                FollowQuestion f = new FollowQuestion();
                f.followedBy = userId;
                f.questionId = questionId;
                db.FollowQuestions.Add(f);
                db.SaveChanges();
                s = "UnFollow";
                return Ok(s);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> VoteAnswerReply(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                AnswerReply comment = await db.AnswerReplies.FindAsync(id);
                var commentVoteByUser = await db.AnswerReplyVotes.FirstOrDefaultAsync(x => x.answerReplyId == id && x.votedBy == userId);
                if (comment == null)
                {
                    return NotFound();
                }
                var vote = commentVoteByUser;
                if (vote != null)
                {
                    if (vote.isUp && isup)
                    {
                        return BadRequest("You have already voteup");
                    }
                    else if (vote.isUp && !isup)
                    {
                        vote.isUp = false;
                    }
                    else if (!vote.isUp && !isup)
                    {
                        return BadRequest("You have already votedown");
                    }
                    else if (!vote.isUp && isup)
                    {
                        vote.isUp = true;
                    }
                }
                else
                {
                    AnswerReplyVote repvote = new  AnswerReplyVote();
                    repvote.isUp = isup;
                    repvote.votedBy = userId;
                    repvote.answerReplyId = id;
                    db.AnswerReplyVotes.Add(repvote);
                }
                await db.SaveChangesAsync();

                var q = (from x in comment.AnswerReplyVotes.Where(x => x.answerReplyId == comment.Id)
                         let voteUp = comment.AnswerReplyVotes.Count(m => m.isUp)
                         let voteDown = comment.AnswerReplyVotes.Count(m => m.isUp == false)
                         select new { voteUpCount = voteUp, voteDownCount = voteDown }).FirstOrDefault();

                return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionExists(int id)
        {
            return db.Questions.Count(e => e.Id == id) > 0;
        }
    }
}