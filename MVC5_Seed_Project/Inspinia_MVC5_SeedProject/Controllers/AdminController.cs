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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Inspinia_MVC5_SeedProject.Controllers;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class AdminController : ApiController
    {
        private Entities db = new Entities();
        public AdminController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

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
        [HttpPost]
        public async Task<IHttpActionResult> MakeAdmin(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var makeAdmin =await db.AspNetUsers.FirstOrDefaultAsync(x => x.UserName.Equals(email));
                    if(makeAdmin != null)
                    {
                        if (makeAdmin.status != "admin") {
                            makeAdmin.status = "admin";
                        }
                        else
                        {
                            makeAdmin.status = "active";
                        }
                        await db.SaveChangesAsync();
                        return Ok("Done");
                    }
                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetAllAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var ret = from admin in db.AspNetUsers
                              where admin.status.Equals("admin")
                              select new
                              {
                                  id = admin.Id,
                                  name = admin.Email
                              };
                    return Ok(ret);

                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> BlockUser(string email) //block on the basis of id
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var status = db.AspNetUsers.Find(userId).status;
                if (status == "admin")
                {
                    var makeAdmin = await db.AspNetUsers.FirstOrDefaultAsync(x => x.Id.Equals(email));
                    if(makeAdmin != null)
                    {
                        makeAdmin.status = "blocked";
                        await db.SaveChangesAsync();
                        await UserManager.UpdateSecurityStampAsync(makeAdmin.Id);
                        await UserManager.SetLockoutEnabledAsync(makeAdmin.Id, true);
                        await UserManager.SetLockoutEndDateAsync(makeAdmin.Id,DateTime.Today.AddYears(10));
                        return Ok("Done");
                        
                    }
                   
                    return NotFound();
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<bool> addNewBrandModel(string brand, string model, string category)
        {
            if (brand != null)
            {
                brand.Trim();
            }
            if (model != null)
            {
                model.Trim();
            }
            if (brand == "" || brand == "undefined" || brand == null)
            {
                return true;
            }
            if (category == "Mobiles")
            {
                var isNew = db.Mobiles.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    Mobile mob = new Mobile();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.Mobiles.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        MobileModel mod = new MobileModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.MobileModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.MobileModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            var brandId = db.Mobiles.First(x => x.brand.Equals(brand));
                            MobileModel mod = new MobileModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = brandId.Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.MobileModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Laptops")
            {
                var isNew = db.LaptopBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    LaptopBrand mob = new  LaptopBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.LaptopBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        LaptopModel mod = new  LaptopModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.LaptopModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.LaptopModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            LaptopModel mod = new  LaptopModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.Mobiles.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.LaptopModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Bikes")
            {
                var isNew = db.BikeBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    BikeBrand mob = new  BikeBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.BikeBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        BikeModel mod = new  BikeModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.BikeModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.BikeModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            BikeModel mod = new  BikeModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.Mobiles.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.BikeModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else if (category == "Cars")
            {
                var isNew = db.CarBrands.Any(x => x.brand.Equals(brand));
                if (!isNew)
                {
                    CarBrand mob = new  CarBrand();
                    mob.brand = brand;
                    mob.addedBy = User.Identity.GetUserId();
                    mob.status = "a";
                    mob.time = DateTime.UtcNow;
                    db.CarBrands.Add(mob);
                    await db.SaveChangesAsync();
                    if (model != null && model != "" && model != "undefined")
                    {
                        CarModel mod = new  CarModel();
                        mod.addedBy = User.Identity.GetUserId();
                        mod.brandId = mob.Id;
                        mod.status = "a";
                        mod.time = DateTime.UtcNow;
                        mod.model = model;
                        db.CarModels.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var isNewModel = db.CarModels.Any(x => x.model.Equals(model));
                    if (!isNewModel)
                    {
                        if (model != null && model != "" && model != "undefined")
                        {
                            CarModel mod = new  CarModel();
                            mod.addedBy = User.Identity.GetUserId();
                            mod.brandId = db.Mobiles.First(x => x.brand.Equals(brand)).Id;
                            mod.status = "a";
                            mod.time = DateTime.UtcNow;
                            mod.model = model;
                            db.CarModels.Add(mod);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            return true;
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