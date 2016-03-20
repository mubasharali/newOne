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
    public class RealEstateController : ApiController
    {
        private Entities db = new Entities();

        public async Task<IHttpActionResult> SearchRealEstateAds(string tags, string title, int minPrice, int maxPrice, string city, string pp, int minArea, int maxArea, string floor, string bedroom, string bathroom,string subcategory)
        {
            if (subcategory == "plot ")
            {
                subcategory = "plot & land";
            }
            else if (subcategory == "PG ")
            {
                subcategory = "PG & Flatmates";
            }

            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.Houses
                            where (ad.Ad.status.Equals("a") && (subcategory == null || subcategory == "undefined" || ad.Ad.subcategory == subcategory) && (floor == null || floor == "undefined" || floor == ad.floor) && (bathroom == null || bathroom == "undefined" || bathroom == ad.bathroom) && (bedroom == null || bedroom == "undefined" || bedroom == ad.bedroom) && (minArea == 0 || ad.area > minArea) && (maxArea == 5000 || ad.area < maxArea) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 5000000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                floor = ad.floor,
                                area = ad.area,
                                bedroom = ad.bedroom,
                                bathroom = ad.bathroom,
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
            if (tags != null)
            {
                tagsArray = tags.Split(',');
            }


            var temp = from ad in db.Houses
                       where (ad.Ad.status.Equals("a") && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && (subcategory == null || subcategory == "undefined" || ad.Ad.subcategory == subcategory) && (floor == null || floor == "undefined" || floor == ad.floor) && (bathroom == null || bathroom == "undefined" || bathroom == ad.bathroom) && (bedroom == null || bedroom == "undefined" || bedroom == ad.bedroom) && (minArea == 0 || ad.area > minArea) && (maxArea == 5000 || ad.area < maxArea) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 5000000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                           floor = ad.floor,
                           area = ad.area,
                           bedroom = ad.bedroom,
                           bathroom = ad.bathroom,
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

        // POST api/RealEstate
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

        // DELETE api/RealEstate/5
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