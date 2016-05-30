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
using Inspinia_MVC5_SeedProject.Models;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
//using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class TagController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Tag
        public async Task<IHttpActionResult> GetTag(int id= 0,string name = null)
        {
            if (name != null)
            {
                id =await getTagIdByName(name);
            }
            string loginUserId = "";
            if (User.Identity.IsAuthenticated)
            {
                loginUserId = User.Identity.GetUserId();
            }
            var ret = await (from tag in db.Tags
                             where tag.Id.Equals(id)
                             select new
                             {
                                 id = tag.Id,
                                 createdById = tag.createdBy,
                                 createdByName = tag.AspNetUser.Email,
                                 updatedById = tag.updatedBy,
                                 updatedByName = tag.AspNetUser1.Email,
                                 updatedTime = tag.updatedTime,
                                 updatedInfo = tag.updatedInfo,
                                 info = tag.info,
                                 time = tag.time,
                                 followers = tag.FollowTags.Count,
                                 isFollowed = tag.FollowTags.Any(x => x.followedBy == loginUserId),
                                 loginUserId = loginUserId,
                                 name = tag.name,
                                 isReported = tag.ReportedTags.Any(x => x.reportedBy.Equals(loginUserId)),
                                 reportedCount = tag.ReportedTags.Count,
                                 ads = from ad in tag.AdTags
                                       where ad.tagId.Equals(id)
                                       orderby ad.Ad.time descending
                                       select new
                                       {
                                           title = ad.Ad.title,
                                           postedById = ad.Ad.AspNetUser.Id,
                                           postedByName = ad.Ad.AspNetUser.Email,
                                           description = ad.Ad.description,
                                           id = ad.Ad.Id,
                                           time = ad.Ad.time,
                                           islogin = loginUserId,
                                           isNegotiable = ad.Ad.isnegotiable,
                                           price = ad.Ad.price,
                                           reportedCount = ad.Ad.Reporteds.Count,
                                           isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == loginUserId),
                                           views = ad.Ad.views,
                                           condition = ad.Ad.condition,
                                           category = ad.Ad.category,
                                           color = ad.Ad.LaptopAd.color,
                                           brand = ad.Ad.LaptopAd.LaptopModel.LaptopBrand.brand,
                                           model = ad.Ad.LaptopAd.LaptopModel.model,
                                           adTags = from tag1 in ad.Ad.AdTags.ToList()
                                                    select new
                                                    {
                                                        id = tag1.tagId,
                                                        name = tag1.Tag.name,
                                                    },
                                           bid = from biding in ad.Ad.Bids.ToList()
                                                 select new
                                                 {
                                                     price = biding.price,
                                                 },
                                           adImages = from image in ad.Ad.AdImages.ToList()
                                                      select new
                                                      {
                                                          imageExtension = image.imageExtension,
                                                      },
                                           location = new
                                           {
                                               cityName = ad.Ad.AdsLocation.City.cityName,
                                               cityId = ad.Ad.AdsLocation.cityId,
                                               popularPlaceId = ad.Ad.AdsLocation.popularPlaceId,
                                               popularPlace = ad.Ad.AdsLocation.popularPlace.name,
                                           },
                                       },
                                 questions = from question in tag.QuestionTags
                                             where question.tagId.Equals(id)
                                             orderby question.Question.time descending
                                             select new
                                             {
                                                 title = question.Question.title,
                                                 id = question.Question.Id,
                                                 views = question.Question.views,
                                                 answers = question.Question.Answers.Count(),
                                                 voteUpCount = question.Question.QuestionVotes.Count,
                                                 voteDownCount = question.Question.QuestionVotes.Count(x => x.isUp == false),
                                                 time = question.Question.time,
                                                 postedByName = question.Question.AspNetUser.Email,
                                                 postedById = question.Question.postedBy,
                                                 tags = from qtag in question.Question.QuestionTags
                                                        select new
                                                        {
                                                            id = qtag.tagId,
                                                            name = qtag.Tag.name,
                                                        }
                                             }

                             }).FirstOrDefaultAsync();
            return Ok(ret);
        }
        //[HttpPost]
        //public async Task<IHttpActionResult> UpdateTag(Tag id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //   // db.Entry(tag).State = EntityState.Modified;
        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        throw;
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        [HttpPost]
        public async Task<IHttpActionResult> DeleteTag(int id)
        {
            Tag comment = await db.Tags.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Tags.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }
        public async Task<IHttpActionResult> RecentAddedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      where tag.time >= days
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                          updatedInfo = tag.updatedInfo,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> RecentUpdatedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      where tag.updatedTime >= days
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                          updatedInfo = tag.updatedInfo,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> ReportedTags(int daysago)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysago);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from tag in db.Tags
                      //where tag.time >= days
                      where tag.ReportedTags.Count > 0 || !tag.updatedInfo.Equals(null)
                      orderby tag.ReportedTags.Count
                      select new
                      {
                          createdBy = tag.createdBy,
                          createdByName = tag.AspNetUser.UserName,
                          name = tag.name,
                          info = tag.info,
                          time = tag.time,
                          id = tag.Id,
                          updatedTime = tag.updatedTime,
                          updatedBy = tag.updatedBy,
                          updatedInfo = tag.updatedInfo,
                      };
            return Ok(ret);
        }
        public async Task<int> getTagIdByName(string s)
        {
            var ret = await db.Tags.FirstOrDefaultAsync(x => x.name.Equals(s));
            if (ret != null)
            {
                return ret.Id;
            }
            return -1;
        }
        [HttpGet]
        public async Task<IHttpActionResult> SearchTags(string s)
        {
            //var ret = db.Tags.Where(x => x.name.Contains(s));
            var ret = from tag in db.Tags
                      where tag.name.Contains(s)
                      select new
                      {
                          id = tag.Id,
                          info = tag.info,
                          name = tag.name,
                          followers = tag.FollowTags.Count(),
                          questions = tag.AdTags.Count + tag.QuestionTags.Count ,
                          url = "/Tag/" + tag.Id + "/" + tag.name,
                      };
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> FollowTags(string tags,bool isUnfollowAllowed)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                string[] values = tags.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    string ss = values[i];
                    if (ss != "")
                    {
                        var tagExit = db.Tags.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                        if (tagExit != null)
                        {
                            var alreadyFollowed =await db.FollowTags.FirstOrDefaultAsync(x => x.tagId.Equals(tagExit.Id) && x.followedBy.Equals(userId));
                            if (alreadyFollowed == null)
                            {
                                FollowTag follow = new FollowTag();
                                follow.followedBy = userId;
                                follow.tagId = tagExit.Id;
                                db.FollowTags.Add(follow);
                                await db.SaveChangesAsync();
                               // return Ok("Successfully Followed");
                            }
                            else
                            {
                                if (isUnfollowAllowed)
                                {
                                    db.FollowTags.Remove(alreadyFollowed);
                                    await db.SaveChangesAsync();
                                   // return Ok("Successfully Unfollowed");
                                }
                            }
                        }
                        //return BadRequest("Tag does not exit");
                    }
                }
                return Ok("Done");
            }
            return BadRequest("Not login");
        }
        public async Task<IHttpActionResult> Follow(int tagId)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                string s = "";
                int c = 0;
                var data = new { status = s, count = c };
                Tag q = await db.Tags.FindAsync(tagId);
                var followed = q.FollowTags.FirstOrDefault(x => x.followedBy == userId);
                if (followed != null)
                {
                    db.FollowTags.Remove(followed);
                    db.SaveChanges();
                    s = "Follow";
                    c = await db.FollowTags.CountAsync(x=>x.tagId.Equals(tagId));
                    data = new { status = s, count = c };
                    return Ok(data);
                }
                FollowTag f = new  FollowTag();
                f.followedBy = userId;
                f.tagId = tagId;
                db.FollowTags.Add(f);
                db.SaveChanges();
                s = "UnFollow";
                c = await db.FollowTags.CountAsync(x=>x.tagId.Equals(tagId));
                data = new { status = s, count = c };
                return Ok(data);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ReportTag(int id)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Tag ad = await db.Tags.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.ReportedTags.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report a Tag only once.If something is really wrong you can contact us");
                }
                ReportedTag rep = new  ReportedTag();
                rep.reportedBy = userId;
                rep.tagId = id;
                db.ReportedTags.Add(rep);
                await db.SaveChangesAsync();

                var count = ad.ReportedTags.Count;

                return Ok(count);
            }
            else
            {
                return BadRequest("Not login");
            }

        }
        [HttpPost]
        public async Task<IHttpActionResult> updateTag(Tag comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var data1 =await db.Tags.FindAsync(comment.Id);
                if (data1.updatedInfo != null)
                {
                    return BadRequest("Info is pending Approval");
                }
                int len = comment.info.Length;
                data1.updatedInfo = comment.info;
                data1.updatedTime = DateTime.UtcNow;
                data1.updatedBy = User.Identity.GetUserId();
                //if (isAdmin)
                //{
                //    db.Entry(comment).State = EntityState.Modified;
                //    await db.SaveChangesAsync();
                //    return Ok("Done");
                //}
                try
                {
                    db.Entry(data1).State = EntityState.Modified;
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    string s = e.ToString();
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in e.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                }
                var data = new
                {
                    updatedById = data1.updatedBy,
                    updatedByName = db.AspNetUsers.Find(data1.updatedBy).Email,
                    updatedTime = data1.updatedTime,
                    updatedInfo = data1.updatedInfo,
                };
                return Ok(data);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> updateTagAdmin(int id,string name,string info, string createdBy,string updatedBy,string updatedInfo)
        {
            if (User.Identity.IsAuthenticated)
            {
            //    if (!ModelState.IsValid)
            //    {
            //        return BadRequest();
            //    }
                var data1 = await db.Tags.FindAsync(id);
                data1.name = name;
                data1.info = info;
                data1.updatedInfo = null;
                data1.updatedBy = updatedBy;
                data1.updatedTime = DateTime.UtcNow;
                if (updatedBy == "null")
                {
                    data1.updatedBy = User.Identity.GetUserId();
                }
               // db.Entry(comment).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    string s = e.ToString();
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in e.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> PutTag(int id, Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.Id)
            {
                return BadRequest();
            }

            db.Entry(tag).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        public async Task<IHttpActionResult> ApproveTag(int id)
        {
            var data = await db.Tags.FindAsync(id);
            data.info = data.updatedInfo;
            data.updatedInfo = null;
            await db.SaveChangesAsync();
            return Ok("Done");
        }
        public async Task<IHttpActionResult> RejectTag(int id)
        {
            var data = await db.Tags.FindAsync(id);
           
            data.updatedInfo = null;
            await db.SaveChangesAsync();
            return Ok("Done");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(int id)
        {
            return db.Tags.Count(e => e.Id == id) > 0;
        }
    }
}