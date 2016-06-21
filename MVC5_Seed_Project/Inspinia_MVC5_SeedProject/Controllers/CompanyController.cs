using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http.Formatting;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using Inspinia_MVC5_SeedProject.CodeTemplates;
using System.Web;
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class CompanyController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Company
        public IQueryable<Company> GetCompanies()
        {
            return db.Companies;
        }
        public async Task<IHttpActionResult> GetCompaniesOfLoginUser()
        {
            var loginUserId = User.Identity.GetUserId();
            if (loginUserId == null)
            {
                return BadRequest();
            }
            var companies = db.Companies.Where(x => x.createdBy.Equals(loginUserId)).Select(x => x.title);
            return Ok(companies);
        }
        public async Task<IHttpActionResult> SaveNeedAService(string title, string tags, string city, string pp , string exectLocation)
        {
            var loginUserId = User.Identity.GetUserId();
            if (loginUserId == null)
            {
                return BadRequest();
            }
            Ad ad = new Ad();
            ad.title = title;
            ElectronicsController e = new ElectronicsController();
            // e.MyAd(ad, "Save", "Services");
            ad.category = "Services";
            ad.status = "a";
            ad.type = true;
            ad.condition = "z";
            ad.description = "                                                               ";
            ad.postedBy = loginUserId;
            ad.time = DateTime.UtcNow;
            db.Ads.Add(ad);
            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ed)
            {
                string s = ed.ToString();
            }
            e.SaveTags(tags, ad);
            e.MyAdLocation(city, pp, exectLocation, ad,"Save");
            await db.SaveChangesAsync();
            return Ok("Done");
        }
        // GET api/Company/5
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> GetCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }
        public async Task<IHttpActionResult> SearchCompanies(string title, string tags,string category = null,string city = null, string famousPlace = null)
        {
            if (tags == null || tags == "undefined")
            {
                var ret = from company in db.Companies
                          where ((title == null || title == "" || title == "undefined" || company.title.Contains(title)) && (category == null || category == "" || category == "undefined" || company.category.Equals(category)) && (city == null || city == "undefined" || company.City.cityName.Equals(city)) && (famousPlace == null || famousPlace == "undefined" || company.popularPlace.name.Equals(famousPlace)))
                          select new
                          {
                              id = company.Id,
                              title = company.title,
                              shortabout = company.shortabout,
                              city = company.City.cityName,
                              exectLocation = company.exectLocation,
                              popularPlace = company.popularPlace.name,
                              logoExtension = company.logoextension,
                              //  rating = company.Reviews.Average(x=>x.rating),
                              category = company.category,
                              contactNo1 = company.contactNo1,
                              contactNo2 = company.contactNo2,
                              tags = from tag in company.CompanyTags
                                     select new
                                     {
                                         id = tag.tagId,
                                         name = tag.Tag.name
                                     },

                          };
                return Ok(ret);
            }
            string[] tagsArray = null;
            if (tags != null)
            {
                tagsArray = tags.Split(',');
            }
            var ret1 = from company in db.Companies
                       where ((title == null || title == "" || title == "undefined" || company.title.Contains(title) ) && (!tagsArray.Except(company.CompanyTags.Select(x => x.Tag.name)).Any()) && (category == null || category == "" || category == "undefined" || company.category.Equals(category)))
                      select new
                      {
                          id = company.Id,
                          title = company.title,
                          shortabout = company.shortabout,
                          city = company.City.cityName,
                          exectLocation = company.exectLocation,
                          popularPlace = company.popularPlace.name,
                          logoExtension = company.logoextension,
                          //  rating = company.Reviews.Average(x=>x.rating),
                          contactNo1 = company.contactNo1,
                          contactNo2 = company.contactNo2,
                          category = company.category,
                          tags = from tag in company.CompanyTags
                                 select new
                                 {
                                     id = tag.tagId,
                                     name = tag.Tag.name
                                 },

                      };
            return Ok(ret1);
        }
        public async Task<IHttpActionResult> AddReview(Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                review.reviewedBy = User.Identity.GetUserId();
                review.time = DateTime.UtcNow;
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                var ret =await (from rev in db.Reviews
                          where rev.Id.Equals(review.Id)
                          select new
                          {
                              id = rev.Id,
                              reviewDescription = rev.description,
                              reviewedBy = rev.reviewedBy,
                              reviewedByName = rev.AspNetUser.Email,
                              time = rev.time,
                              rating = rev.rating,
                          }).FirstOrDefaultAsync();
                return Ok(ret);
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> UpdateReview(Review review)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteReview(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var review =await db.Reviews.FirstOrDefaultAsync(x => x.Id.Equals(id));
               
                try {
                    db.Reviews.Remove(review);
                    await db.SaveChangesAsync();
                }
                catch(Exception e)
                {
                    string s = e.ToString();
                }
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> LikeReview(int id, bool isup)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                Review comment = await db.Reviews.FindAsync(id);
                var commentVoteByUser = await db.ReviewVotes.FirstOrDefaultAsync(x => x.reviewId == id && x.votedBy == userId);
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
                    ReviewVote repvote = new  ReviewVote();
                    repvote.isup = isup;
                    repvote.votedBy = userId;
                    repvote.reviewId = id;
                    db.ReviewVotes.Add(repvote);
                }
                await db.SaveChangesAsync();

                var q = (from x in comment.ReviewVotes.Where(x => x.reviewId == comment.Id)
                         let voteUp = comment.ReviewVotes.Count(m => m.isup)
                         let voteDown = comment.ReviewVotes.Count(m => m.isup == false)
                         select new { voteUpCount = voteUp, voteDownCount = voteDown }).FirstOrDefault();

                return Ok(q);
            }
            else
            {
                return BadRequest("You are not login");
            }
        }
        public async Task<IHttpActionResult> AddReviewReply(ReviewReply review)
        {
            if (User.Identity.IsAuthenticated)
            {
                review.postedBy = User.Identity.GetUserId();
                review.time = DateTime.UtcNow;
                db.ReviewReplies.Add(review);
                await db.SaveChangesAsync();
                var ret =await (from rep in db.ReviewReplies
                          where rep.Id.Equals(review.Id)
                          select new
                          {
                              id = rep.Id,
                              time = rep.time,
                              description = rep.description,
                              postedBy = rep.postedBy,
                              postedByName = rep.AspNetUser.Email,
                          }).FirstOrDefaultAsync();
                return Ok(ret);
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> UpdateReviewReply(ReviewReply review)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteReviewReply(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var ret =await db.ReviewReplies.FindAsync(id);
                db.ReviewReplies.Remove(ret);
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> GetPage(int id)
        {
            var loginUserId = User.Identity.GetUserId();
            var loginUserProfileExtension = "";
            if (loginUserId != null)
            {
                loginUserProfileExtension = db.AspNetUsers.Find(loginUserId).dpExtension; 
            }
            var comp = db.Companies.Find(id);
            if(comp.views == null)
            {
                comp.views = 0;
            }
            comp.views = comp.views + 1;
           // db.Entry(comp).State = EntityState.Modified;
            await db.SaveChangesAsync();
            var ret = from company in db.Companies
                      where company.Id.Equals(id)
                      select new
                      {
                          reviews = from review in company.Reviews
                                   //where review.reviewedBy.Equals(loginUserId)
                                   select new
                                   {
                                       id = review.Id,
                                       rating = review.rating,
                                       reviewDescription = review.description,
                                       time = review.time,
                                       reviewedBy = review.reviewedBy,
                                       reviewedByName = review.AspNetUser.Email,
                                       voteUpCount = review.ReviewVotes.Count(x=>x.isup),
                                       voteDownCount = review.ReviewVotes.Count(x=>x.isup == false),
                                       isUp = review.ReviewVotes.Any(x=>x.isup && x.votedBy.Equals(loginUserId)),
                                       isDown = review.ReviewVotes.Any(x => x.isup == false && x.votedBy.Equals(loginUserId)),
                                       reviewReplies = from reply in review.ReviewReplies
                                                       select new
                                                       {
                                                           id = reply.Id,
                                                           description = reply.description,
                                                           time = reply.time,
                                                           postedBy = reply.postedBy,
                                                           postedByName = reply.AspNetUser.Email,
                                                       },
                                   },
                                   activeAds = from ad in company.CompanyAds
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
                                                 //  brand = ad.Ad.LaptopAd.LaptopModel.LaptopBrand.brand,
                                                 //  model = ad.Ad.LaptopAd.LaptopModel.model,
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
                          loginUserId = loginUserId,
                          loginUserProfileExtension = loginUserProfileExtension,
                          id = company.Id,
                          title = company.title,
                          time = company.time,
                          since = company.since,
                          shortAbout = company.shortabout,
                          longAbout = company.longabout,
                          logoExtension = company.logoextension,
                          owner = company.owner,
                          status = company.status,
                          views = company.views,
                          category = company.category,
                          twlink = company.twlink,
                          fblink = company.fblink,
                          contactNo1 = company.contactNo1,
                          contactNo2 = company.contactNo2,
                          email = company.email,
                          websiteLink = company.websitelink,
                          createdById = company.AspNetUser.Id,
                          createdByName = company.AspNetUser.Email,
                          cityName = company.City.cityName,
                          cityId = (int?)company.City.Id,
                          popularPlaceId = (int?)company.popularPlaceId,
                          popularPlaceName = company.popularPlace.name,
                          exectLocation = company.exectLocation,
                          branches = from branch in company.CompanyOffices.ToList()
                                     select new
                                     {
                                         id = branch.Id,
                                         since = branch.since,
                                         cityId = branch.cityId,
                                         cityName = branch.City.cityName,
                                         popularPlaceId = branch.popularPlaceId,
                                         popularPlace = branch.popularPlace.name,
                                         exectLocation = branch.exectLocation,
                                         contactNo1 = branch.contactNo1,
                                         contactNo2 = branch.contactNo2
                                     },
                          tags = from tag1 in company.CompanyTags
                                   select new
                                   {
                                       id = tag1.tagId,
                                       name = tag1.Tag.name,
                                   },

                      };

            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> HeadOfficeLocation(Company branch, string city, string popularPlace, string exectLocation)
        {
           // var branch = await db.Companies.FindAsync(companyId);
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    await db.SaveChangesAsync();
                    // loc.cityId = cit.Id;
                    branch.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        db.popularPlaces.Add(pop);
                        await db.SaveChangesAsync();
                        //  loc.popularPlaceId = pop.Id;
                        branch.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    // loc.cityId = citydb.Id;
                    branch.cityId = citydb.Id;
                    if (popularPlace != null)
                    {
                        var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
                        if (ppp == null)
                        {
                            popularPlace pop = new popularPlace();
                            pop.cityId = citydb.Id;
                            pop.name = popularPlace;
                            pop.addedBy = User.Identity.GetUserId();
                            pop.addedOn = DateTime.UtcNow;
                            db.popularPlaces.Add(pop);
                            await db.SaveChangesAsync();
                            //   loc.popularPlaceId = pop.Id;
                            branch.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            //   loc.popularPlaceId = ppp.Id;
                            branch.popularPlaceId = ppp.Id;
                        }
                    }
                }
                branch.exectLocation = exectLocation;
                
                try
                {
                    db.Entry(branch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            //var ret = await (from br in db.CompanyOffices
            //                 where br.Id.Equals(branch.Id)
            //                 select new
            //                 {
            //                     id = br.Id,
            //                     cityId = br.cityId,
            //                     cityName = br.City.cityName,
            //                     popularPlace = br.popularPlace.name,
            //                     popularPlaceId = br.popularPlaceId,
            //                     contactNo1 = br.contactNo1,
            //                     contactNo2 = br.contactNo2,
            //                     since = br.since,
            //                     exectLocation = br.exectLocation,
            //                 }).FirstOrDefaultAsync();
            return Ok("Done");
        }

        public async Task<IHttpActionResult> OfficeBranch(CompanyOffice branch, string city, string popularPlace, string exectLocation,string SaveOrUpdate)
        {
            //var company = await db.Companies.FindAsync(companyId);
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedBy = User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    await db.SaveChangesAsync();
                    // loc.cityId = cit.Id;
                    branch.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        db.popularPlaces.Add(pop);
                        await db.SaveChangesAsync();
                        //  loc.popularPlaceId = pop.Id;
                        branch.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    // loc.cityId = citydb.Id;
                    branch.cityId = citydb.Id;
                    if (popularPlace != null)
                    {
                        var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
                        if (ppp == null)
                        {
                            popularPlace pop = new popularPlace();
                            pop.cityId = citydb.Id;
                            pop.name = popularPlace;
                            pop.addedBy = User.Identity.GetUserId();
                            pop.addedOn = DateTime.UtcNow;
                            db.popularPlaces.Add(pop);
                            await db.SaveChangesAsync();
                            //   loc.popularPlaceId = pop.Id;
                            branch.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            //   loc.popularPlaceId = ppp.Id;
                            branch.popularPlaceId = ppp.Id;
                        }
                    }
                }
                branch.exectLocation = exectLocation;
                if (SaveOrUpdate == "Save")
                {
                    db.CompanyOffices.Add(branch);
                }
                else if (SaveOrUpdate == "Update")
                {
                    //var bra = db.CompanyOffices.Find(branch.Id);
                    //bra.popularPlaceId = branch.popularPlaceId;
                    //bra.since = branch.since;
                    //bra.cityId = branch.cityId;
                    //bra.contactNo1 = branch.contactNo1;
                    //bra.contactNo2 = branch.contactNo2;
                    //await db.SaveChangesAsync();
                    db.Entry(branch).State = EntityState.Modified;
                }
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            var ret = await (from br in db.CompanyOffices
                      where br.Id.Equals(branch.Id)
                      select new
                      {
                          id = br.Id,
                          cityId = br.cityId,
                          cityName = br.City.cityName,
                          popularPlace = br.popularPlace.name,
                          popularPlaceId = br.popularPlaceId,
                          contactNo1 = br.contactNo1,
                          contactNo2 = br.contactNo2,
                          since = br.since,
                          exectLocation = br.exectLocation,
                      }).FirstOrDefaultAsync();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBranch(int id)
        {
            CompanyOffice company = await db.CompanyOffices.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            db.CompanyOffices.Remove(company);
            await db.SaveChangesAsync();

            return Ok("Done");
        }
        public async Task<object> UpdateTags(string s, int companyId)
        {
           var com =await db.Companies.Include("CompanyTags").FirstOrDefaultAsync(x => x.Id.Equals(companyId));
            var temp = com.CompanyTags.ToList();
            foreach (var cc in temp)
            {
                db.CompanyTags.Remove(cc);
            }
            
            await db.SaveChangesAsync();
            if(s == null || s == "" || s== "undefined")
            {
                return -1;
            }
            string[] values = s.Split(',');
            Models.Tag[] tags = new Models.Tag[values.Length];
            CompanyTag[] qt = new CompanyTag[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
                string ss = values[i];
                if (ss != "")
                {
                    var data = await db.Tags.FirstOrDefaultAsync(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                    tags[i] = new Models.Tag();
                    if (data != null)
                    {
                        tags[i].Id = data.Id;
                    }
                    else
                    {
                        tags[i].name = values[i];
                        tags[i].time = DateTime.UtcNow;
                        tags[i].createdBy = User.Identity.GetUserId();
                        db.Tags.Add(tags[i]);
                    }
                }
                else
                {
                    tags[i] = null;
                }
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string sb = e.ToString();
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (tags[i] != null)
                {
                    qt[i] = new CompanyTag();
                    qt[i].companyId = companyId;
                    qt[i].tagId = tags[i].Id;
                    db.CompanyTags.Add(qt[i]);
                }
            }
            await db.SaveChangesAsync();
            var ret = from taa in qt
                      select new
                      {
                          id = taa.tagId,
                          name = taa.Tag.name,
                      };
            return ret;
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateLongAbout(Company comment)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            db.Entry(comment).State = EntityState.Modified;
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


        [HttpPost]
        public async Task<IHttpActionResult> UpdatePage(Company comment,string tags)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            db.Entry(comment).State = EntityState.Modified;
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
           var returnTags = await UpdateTags(tags, comment.Id);
            return Ok(returnTags);
        }
        public class BranchLocation1
        {
            public string cityName;
            public string popularPlace;
        }
        private static readonly string _awsAccessKey =
            ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey =
            ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName =
            ConfigurationManager.AppSettings["Bucketname"];
        private static readonly string _folderName =
            ConfigurationManager.AppSettings["FolderName"];
        private static readonly string _userFolder =
            ConfigurationManager.AppSettings["UserFolder"];
        private static readonly string _email =
            ConfigurationManager.AppSettings["email"];
        private static readonly string _companyFoler =
            ConfigurationManager.AppSettings["CompanyFolder"];
        // DELETE api/Company/5
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            IAmazonS3 client;

            var logoExtension = company.logoextension;
            if (logoExtension != null) {
                logoExtension = logoExtension.Trim();
            }

            DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest();
            multiObjectDeleteRequest.BucketName = _bucketName;
            multiObjectDeleteRequest.AddKey(_companyFoler + "/" + company.Id + "/logo" +logoExtension, null);
              
            var userId = User.Identity.GetUserId();
            var email = company.AspNetUser.UserName;
            var status = db.AspNetUsers.Find(userId).status;
            if (status == "admin")
            {
                if (userId != company.createdBy)
                {
                    string Body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("/Views/Admin/Email/DeletePageAlert.html"));
                    Body = Body.Replace("#CompanyTitle#", company.title);
                    ElectronicsController.sendEmail(email, "Your Busniess page is deleted by admin!", Body);
                }
            }

            try
            {
                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = "https://s3.amazonaws.com/";
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(
                     _awsAccessKey, _awsSecretKey, config))
                {
                    client.DeleteObjects(multiObjectDeleteRequest);
                }
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
            db.Companies.Remove(company);
            await db.SaveChangesAsync();

            return Ok(company);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(int id)
        {
            return db.Companies.Count(e => e.Id == id) > 0;
        }
    }
}