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
    public class JobController : ApiController
    {
        private Entities db = new Entities();

        public async Task<IHttpActionResult> GetAllQualifications()
        {
            var ret = db.JobAds.Select(x => x.qualification);
            return Ok(ret);
        }
        public async Task<IHttpActionResult> SearchQualifications(string s)
        {
            var ret = from qual in db.JobAds
                      where qual.qualification.Contains(s)
                      select new
                      {
                          name = qual.qualification,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> SearchSkills(string s)
        {
            var ret = from tag in db.Skills
                      where tag.name.Contains(s)
                      select new
                      {
                          id = tag.Id,
                          name = tag.name,
                      };
            return Ok(ret);
        }
        public async Task<IHttpActionResult> SearchJobAds(string gender, string skills, string tags, string title, int minPrice, int maxPrice, string city, string pp, string salaryType, string category, string qualification, string exprience , string careerLevel, string jobType, DateTime? lastDateToApply, int minSeats, int maxSeats, string shift)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null && skills == null)
            {
                var temp1 = from ad in db.JobAds
                            where ((gender == null || gender == "undefined" || ad.Ad.isnegotiable == gender) && ad.Ad.status.Equals("a") && (salaryType == null || salaryType == "undefined" || ad.salaryType == salaryType) && (category == null || category == "undefined" || ad.category1 == category) && (title == null || title == "undefined" || ad.Ad.title == title) && (qualification == null || qualification == "undefined" || ad.qualification == qualification)
                            && (exprience == null || exprience == "undefined" || ad.exprience == exprience) && (careerLevel == null || careerLevel == "undefined" || ad.careerLevel.Equals(careerLevel)) && (jobType == null || jobType == "undefined" || ad.Ad.subcategory.Equals(jobType))  && (shift == null || shift == "undefined" || ad.Ad.condition.Equals(shift))
                            && ( lastDateToApply == null  || ad.lastDateToApply == lastDateToApply) 
                            && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 500000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) ) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))
                             && (minSeats == 0 || ad.seats > minSeats) && (maxSeats == 1000 || ad.seats < maxSeats) )
                            orderby ad.Ad.time descending
                            select new
                            {
                                title = ad.Ad.title,
                                postedById = ad.Ad.AspNetUser.Id,
                                postedByName = ad.Ad.AspNetUser.Email,
                                description = ad.Ad.description,
                                id = ad.Ad.Id,
                                time = ad.Ad.time,
                                islogin = islogin,
                                isNegotiable = ad.Ad.isnegotiable,
                                price = ad.Ad.price,
                                reportedCount = ad.Ad.Reporteds.Count,
                                isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == islogin),
                                // views = ad.Ad.AdViews.Count,
                                views = ad.Ad.views,
                                condition = ad.Ad.condition,
                                savedCount = ad.Ad.SaveAds.Count,
                                adTags = from tag1 in ad.Ad.AdTags.ToList()
                                         select new
                                         {
                                             id = tag1.tagId,
                                             name = tag1.Tag.name,
                                             //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                             //info = tag.Tag.info,
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
                                    exectLocation = ad.Ad.AdsLocation.exectLocation,
                                },

                            };
                return Ok(temp1);
            }
            string[] tagsArray = null;
            string[] skillsArray= null;
            if (skills != null && tags != null)
            {
                tagsArray = tags.Split(',');
                skillsArray = skills.Split(',');
                var temp = from ad in db.JobAds
                           where ((gender == null || gender == "undefined" || ad.Ad.isnegotiable == gender) && ad.Ad.status.Equals("a") && (salaryType == null || salaryType == "undefined" || ad.salaryType == salaryType) && (category == null || category == "undefined" || ad.category1 == category) && (title == null || title == "undefined" || ad.Ad.title == title) && (qualification == null || qualification == "undefined" || ad.qualification == qualification)
                                                       && (exprience == null || exprience == "undefined" || ad.exprience == exprience) && (careerLevel == null || careerLevel == "undefined" || ad.careerLevel.Equals(careerLevel)) && (jobType == null || jobType == "undefined" || ad.Ad.subcategory.Equals(jobType)) && (shift == null || shift == "undefined" || ad.Ad.condition.Equals(shift))
                                                       && (lastDateToApply == null || ad.lastDateToApply == lastDateToApply)
                                                       && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 500000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city)) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))
                                                        && (minSeats == 0 || ad.seats > minSeats) && (maxSeats == 1000 || ad.seats < maxSeats)
                                && (!skillsArray.Except(ad.Ad.JobSkills.Select(x => x.Skill.name)).Any()) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()))
                           orderby ad.Ad.time descending
                           select new
                           {
                               title = ad.Ad.title,
                               postedById = ad.Ad.AspNetUser.Id,
                               postedByName = ad.Ad.AspNetUser.Email,
                               description = ad.Ad.description,
                               id = ad.Ad.Id,
                               time = ad.Ad.time,
                               islogin = islogin,
                               isNegotiable = ad.Ad.isnegotiable,
                               price = ad.Ad.price,
                               reportedCount = ad.Ad.Reporteds.Count,
                               isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == islogin),
                               // views = ad.Ad.AdViews.Count,
                               views = ad.Ad.views,
                               condition = ad.Ad.condition,
                               savedCount = ad.Ad.SaveAds.Count,
                               adTags = from tag1 in ad.Ad.AdTags.ToList()
                                        select new
                                        {
                                            id = tag1.tagId,
                                            name = tag1.Tag.name,
                                            //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                            //info = tag.Tag.info,
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
                                   exectLocation = ad.Ad.AdsLocation.exectLocation,
                               },

                           };
                return Ok(temp);
            }

            if (skills != null )
            {
                skillsArray = skills.Split(',');
                var temp = from ad in db.JobAds
                           where ((gender == null || gender == "undefined" || ad.Ad.isnegotiable == gender) && ad.Ad.status.Equals("a") && (salaryType == null || salaryType == "undefined" || ad.salaryType == salaryType) && (category == null || category == "undefined" || ad.category1 == category) && (title == null || title == "undefined" || ad.Ad.title == title) && (qualification == null || qualification == "undefined" || ad.qualification == qualification)
                                                       && (exprience == null || exprience == "undefined" || ad.exprience == exprience) && (careerLevel == null || careerLevel == "undefined" || ad.careerLevel.Equals(careerLevel)) && (jobType == null || jobType == "undefined" || ad.Ad.subcategory.Equals(jobType)) && (shift == null || shift == "undefined" || ad.Ad.condition.Equals(shift))
                                                       && (lastDateToApply == null || ad.lastDateToApply == lastDateToApply)
                                                       && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 500000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city)) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))
                                                        && (minSeats == 0 || ad.seats > minSeats) && (maxSeats == 1000 || ad.seats < maxSeats)
                                && (!skillsArray.Except(ad.Ad.JobSkills.Select(x => x.Skill.name)).Any()))
                           orderby ad.Ad.time descending
                           select new
                           {
                               title = ad.Ad.title,
                               postedById = ad.Ad.AspNetUser.Id,
                               postedByName = ad.Ad.AspNetUser.Email,
                               description = ad.Ad.description,
                               id = ad.Ad.Id,
                               time = ad.Ad.time,
                               islogin = islogin,
                               isNegotiable = ad.Ad.isnegotiable,
                               price = ad.Ad.price,
                               reportedCount = ad.Ad.Reporteds.Count,
                               isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == islogin),
                               // views = ad.Ad.AdViews.Count,
                               views = ad.Ad.views,
                               condition = ad.Ad.condition,
                               savedCount = ad.Ad.SaveAds.Count,
                               adTags = from tag1 in ad.Ad.AdTags.ToList()
                                        select new
                                        {
                                            id = tag1.tagId,
                                            name = tag1.Tag.name,
                                            //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                            //info = tag.Tag.info,
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
                                   exectLocation = ad.Ad.AdsLocation.exectLocation,
                               },

                           };
                return Ok(temp);

            }
            if (tags != null)
            {
                tagsArray = tags.Split(',');
                var temp = from ad in db.JobAds
                           where ((gender == null || gender == "undefined" || ad.Ad.isnegotiable == gender) && ad.Ad.status.Equals("a") && (salaryType == null || salaryType == "undefined" || ad.salaryType == salaryType) && (category == null || category == "undefined" || ad.category1 == category) && (title == null || title == "undefined" || ad.Ad.title == title) && (qualification == null || qualification == "undefined" || ad.qualification == qualification)
                                                       && (exprience == null || exprience == "undefined" || ad.exprience == exprience) && (careerLevel == null || careerLevel == "undefined" || ad.careerLevel.Equals(careerLevel)) && (jobType == null || jobType == "undefined" || ad.Ad.subcategory.Equals(jobType)) && (shift == null || shift == "undefined" || ad.Ad.condition.Equals(shift))
                                                       && (lastDateToApply == null || ad.lastDateToApply == lastDateToApply)
                                                       && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 500000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city)) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))
                                                        && (minSeats == 0 || ad.seats > minSeats) && (maxSeats == 1000 || ad.seats < maxSeats)
                                && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()))
                           orderby ad.Ad.time descending
                           select new
                           {
                               title = ad.Ad.title,
                               postedById = ad.Ad.AspNetUser.Id,
                               postedByName = ad.Ad.AspNetUser.Email,
                               description = ad.Ad.description,
                               id = ad.Ad.Id,
                               time = ad.Ad.time,
                               islogin = islogin,
                               isNegotiable = ad.Ad.isnegotiable,
                               price = ad.Ad.price,
                               reportedCount = ad.Ad.Reporteds.Count,
                               isReported = ad.Ad.Reporteds.Any(x => x.reportedBy == islogin),
                               // views = ad.Ad.AdViews.Count,
                               views = ad.Ad.views,
                               condition = ad.Ad.condition,
                               savedCount = ad.Ad.SaveAds.Count,
                               adTags = from tag1 in ad.Ad.AdTags.ToList()
                                        select new
                                        {
                                            id = tag1.tagId,
                                            name = tag1.Tag.name,
                                            //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                            //info = tag.Tag.info,
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
                                   exectLocation = ad.Ad.AdsLocation.exectLocation,
                               },

                           };
                return Ok(temp);
            }

            return Ok("Done");
        }




        // GET: api/Job
        public IQueryable<Ad> GetAds()
        {
            return db.Ads;
        }

        // GET: api/Job/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> GetAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        // PUT: api/Job/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAd(int id, Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ad.Id)
            {
                return BadRequest();
            }

            db.Entry(ad).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
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

        // POST: api/Job
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> PostAd(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ads.Add(ad);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ad.Id }, ad);
        }

        // DELETE: api/Job/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> DeleteAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            db.Ads.Remove(ad);
            await db.SaveChangesAsync();

            return Ok(ad);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdExists(int id)
        {
            return db.Ads.Count(e => e.Id == id) > 0;
        }
    }
}