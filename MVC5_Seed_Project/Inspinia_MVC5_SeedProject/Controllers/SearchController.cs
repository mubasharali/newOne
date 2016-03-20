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
    public class SearchController : ApiController
    {
        private Entities db = new Entities();

        public async Task<IHttpActionResult> SearchAds(string tags, string title, int minPrice, int maxPrice, string city, string pp,string category, string subcategory)
        {
            if (subcategory.Equals("Books "))
            {
                subcategory = "Books & Magazines";
            }
            else if (subcategory == "Gym ")
            {
                subcategory = "Gym & Fitness";
            }
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.Ads
                            where (ad.category.Equals(category) && (subcategory == null || subcategory == "undefined" || ad.subcategory.Equals(subcategory) )  && ad.status.Equals("a")  && (minPrice == 0 || ad.price > minPrice) && (maxPrice == 50000 || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                            orderby ad.time descending
                            select new
                            {
                                title = ad.title,
                                postedById = ad.AspNetUser.Id,
                                postedByName = ad.AspNetUser.Email,
                                description = ad.description,
                                id = ad.Id,
                                time = ad.time,
                                islogin = islogin,
                                isNegotiable = ad.isnegotiable,
                                price = ad.price,
                                reportedCount = ad.Reporteds.Count,
                                isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                                // views = ad.Ad.AdViews.Count,
                                views = ad.views,
                                condition = ad.condition,
                                savedCount = ad.SaveAds.Count,
                                adTags = from tag1 in ad.AdTags.ToList()
                                         select new
                                         {
                                             id = tag1.tagId,
                                             name = tag1.Tag.name,
                                             //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                             //info = tag.Tag.info,
                                         },
                                bid = from biding in ad.Bids.ToList()
                                      select new
                                      {
                                          price = biding.price,
                                      },
                                adImages = from image in ad.AdImages.ToList()
                                           select new
                                           {
                                               imageExtension = image.imageExtension,
                                           },
                                location = new
                                {
                                    cityName = ad.AdsLocation.City.cityName,
                                    cityId = ad.AdsLocation.cityId,
                                    popularPlaceId = ad.AdsLocation.popularPlaceId,
                                    popularPlace = ad.AdsLocation.popularPlace.name,
                                    exectLocation = ad.AdsLocation.exectLocation,
                                },

                            };
                return Ok(temp1);
            }
            string[] tagsArray = null;
            if (tags != null)
            {
                tagsArray = tags.Split(',');
            }



            var temp = from ad in db.Ads
                       where (ad.category.Equals(category) && (subcategory == null || subcategory == "undefined" || ad.subcategory.Equals(subcategory)) && (!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any()) && ad.status.Equals("a") && (minPrice == 0 || ad.price > minPrice) && (maxPrice == 50000 || ad.price < maxPrice) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                        
                       orderby ad.time descending
                        select new
                        {
                            title = ad.title,
                            postedById = ad.AspNetUser.Id,
                            postedByName = ad.AspNetUser.Email,
                            description = ad.description,
                            id = ad.Id,
                            time = ad.time,
                            islogin = islogin,
                            isNegotiable = ad.isnegotiable,
                            price = ad.price,
                            reportedCount = ad.Reporteds.Count,
                            isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                            // views = ad.Ad.AdViews.Count,
                            views = ad.views,
                            condition = ad.condition,
                            savedCount = ad.SaveAds.Count,
                            adTags = from tag1 in ad.AdTags.ToList()
                                     select new
                                     {
                                         id = tag1.tagId,
                                         name = tag1.Tag.name,
                                         //followers = tag.Tag.FollowTags.Count(x => x.tagId.Equals(tag.Id)),
                                         //info = tag.Tag.info,
                                     },
                            bid = from biding in ad.Bids.ToList()
                                  select new
                                  {
                                      price = biding.price,
                                  },
                            adImages = from image in ad.AdImages.ToList()
                                       select new
                                       {
                                           imageExtension = image.imageExtension,
                                       },
                            location = new
                            {
                                cityName = ad.AdsLocation.City.cityName,
                                cityId = ad.AdsLocation.cityId,
                                popularPlaceId = ad.AdsLocation.popularPlaceId,
                                popularPlace = ad.AdsLocation.popularPlace.name,
                                exectLocation = ad.AdsLocation.exectLocation,
                            },

                        };
            return Ok(temp);
        }

        // GET api/Search/5
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

        // PUT api/Search/5
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

        // POST api/Search
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

        // DELETE api/Search/5
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