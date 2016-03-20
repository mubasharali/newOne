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
using Microsoft.AspNet.Identity;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class AdminController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Admin
        public IQueryable<Mobile> GetMobiles()
        {
            return db.Mobiles;
        }
        public async Task<bool> isAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<IHttpActionResult> GetFeedbacks()
        {
            var ret = from feed in db.Feedbacks
                      orderby feed.time descending
                      select new
                      {
                          id = feed.Id,
                          type = feed.type,
                          description = feed.description,
                          time = feed.time,
                          givenById = feed.givenBy,
                          givenByName = feed.AspNetUser.Email,
                      };
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteFeedback(int id)
        {
            var feedback = await db.Feedbacks.FindAsync(id);
            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();
            return Ok("Done");
        }
        public async Task<IHttpActionResult> GetAds(int limit)
        {
           // await AdViews(id);
            string islogin = "";
            string loginUserProfileExtension = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
                var ide = await db.AspNetUsers.FindAsync(islogin);
                loginUserProfileExtension = ide.dpExtension;
            }
            var ret = ((from ad in db.Ads
                       where ad.status != "a" || ad.Reporteds.Count > 0
                       orderby ad.time descending
                       select new
                       {
                           title = ad.title,
                           postedById = ad.AspNetUser.Id,
                           postedByName = ad.AspNetUser.Email,
                           description = ad.description,
                           id = ad.Id,
                           time = ad.time,
                           status = ad.status,
                           islogin = islogin,
                           loginUserProfileExtension = loginUserProfileExtension,
                           isNegotiable = ad.isnegotiable,
                           price = ad.price,
                           reportedCount = ad.Reporteds.Count,
                           isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                           //views = ad.AdViews.Count,
                           views = ad.views,
                           condition = ad.condition,
                           type = ad.type,
                           isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                           savedCount = ad.SaveAds.Count,
                           mobilead = new
                           {
                               color = ad.MobileAd.color,
                               sims = ad.MobileAd.sims,
                               brand = ad.MobileAd.MobileModel.Mobile.brand,
                               model = ad.MobileAd.MobileModel.model
                           },
                           laptopad = new
                           {
                               color = ad.LaptopAd.color,
                               brand = ad.LaptopAd.LaptopModel.LaptopBrand.brand,
                               model = ad.LaptopAd.LaptopModel.model,
                           },
                           location = new
                           {
                               cityName = ad.AdsLocation.City.cityName,
                               cityId = ad.AdsLocation.cityId,
                               popularPlaceId = ad.AdsLocation.popularPlaceId,
                               popularPlace = ad.AdsLocation.popularPlace.name,
                               exectLocation = ad.AdsLocation.exectLocation,
                           },
                           adTags = from tag in ad.AdTags.ToList()
                                    select new
                                    {
                                        id = tag.tagId,
                                        name = tag.Tag.name,
                                        followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                        //info = tag.Tag.info,
                                    },
                           bid = from biding in ad.Bids.ToList()
                                 select new
                                 {
                                     postedByName = biding.AspNetUser.Email,
                                     postedById = biding.AspNetUser.Id,
                                     price = biding.price,
                                     time = biding.time,
                                     id = biding.Id,
                                 },
                           comment = from comment in ad.Comments.ToList()
                                     orderby comment.time
                                     select new
                                     {
                                         description = comment.description,
                                         postedById = comment.postedBy,
                                         postedByName = comment.AspNetUser.Email,
                                         imageExtension = comment.AspNetUser.dpExtension,
                                         time = comment.time,
                                         id = comment.Id,
                                         adId = comment.adId,
                                         islogin = islogin,
                                         loginUserProfileExtension = loginUserProfileExtension,
                                         voteUpCount = comment.CommentVotes.Where(x => x.isup == true).Count(),
                                         voteDownCount = comment.CommentVotes.Where(x => x.isup == false).Count(),
                                         isUp = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup),
                                         isDown = comment.CommentVotes.Any(x => x.votedBy == islogin && x.isup == false),
                                         commentReply = from commentreply in comment.CommentReplies.ToList()
                                                        orderby commentreply.time
                                                        select new
                                                        {
                                                            id = commentreply.Id,
                                                            description = commentreply.description,
                                                            postedById = commentreply.postedBy,
                                                            postedByName = commentreply.AspNetUser.Email,
                                                            imageExtension = commentreply.AspNetUser.dpExtension,
                                                            loginUserProfileExtension = loginUserProfileExtension,
                                                            time = commentreply.time,
                                                            voteUpCount = commentreply.CommentReplyVotes.Where(x => x.isup == true).Count(),
                                                            voteDownCount = commentreply.CommentReplyVotes.Where(x => x.isup == false).Count(),
                                                            isUp = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup),
                                                            isDown = commentreply.CommentReplyVotes.Any(x => x.votedBy == islogin && x.isup == false)
                                                        }
                                     }
                       }).Take(limit)).AsEnumerable();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveAd(int id)
        {
            Ad mobile = await db.Ads.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveMobileBrand(int id)
        {
            Mobile mobile = await db.Mobiles.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveMobileModel(int id)
        {
            MobileModel mobile = await db.MobileModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveLaptopBrand(int id)
        {
            LaptopBrand mobile = await db.LaptopBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveCarBrand(int id)
        {
            CarBrand mobile = await db.CarBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveLaptopModel(int id)
        {
            LaptopModel mobile = await db.LaptopModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveCarModel(int id)
        {
            CarModel mobile = await db.CarModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveBikeBrand(int id)
        {
            BikeBrand mobile = await db.BikeBrands.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }
        [HttpPost]
        public async Task<IHttpActionResult> ApproveBikeModel(int id)
        {
            BikeModel mobile = await db.BikeModels.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }
            if (mobile.status == "a")
            {
                return BadRequest("Already approved");
            }
            mobile.status = "a";
            await db.SaveChangesAsync();
            return Ok("approved");
        }

        // PUT api/Admin/5
        public async Task<IHttpActionResult> PutMobile(int id, Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mobile.Id)
            {
                return BadRequest();
            }

            db.Entry(mobile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobileExists(id))
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

        // POST api/Admin
        [ResponseType(typeof(Mobile))]
        public async Task<IHttpActionResult> PostMobile(Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Mobiles.Add(mobile);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = mobile.Id }, mobile);
        }

        // DELETE api/Admin/5
        [ResponseType(typeof(Mobile))]
        public async Task<IHttpActionResult> DeleteMobile(int id)
        {
            Mobile mobile = await db.Mobiles.FindAsync(id);
            if (mobile == null)
            {
                return NotFound();
            }

            db.Mobiles.Remove(mobile);
            await db.SaveChangesAsync();

            return Ok(mobile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MobileExists(int id)
        {
            return db.Mobiles.Count(e => e.Id == id) > 0;
        }
    }
}