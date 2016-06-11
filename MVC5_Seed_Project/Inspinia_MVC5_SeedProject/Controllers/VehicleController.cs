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
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class VehicleController : ApiController
    {
        private Entities db = new Entities();

        
       
        [HttpPost]
        public async Task<IHttpActionResult> GetCarTree()
        {
            var mobiles = (from mobile in db.CarBrands
                           where mobile.brand != "" && mobile.status != "p"
                           select new
                           {
                               companyName = mobile.brand,
                               models = from model in mobile.CarModels
                                        where model.model != "" && model.status != "p"
                                        select new
                                        {
                                            model = model.model
                                        }
                           }).AsEnumerable();
            return Ok(mobiles);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetBikeTree()
        {
            var mobiles = (from mobile in db.BikeBrands
                           where mobile.brand != "" && mobile.status != "p"
                           select new
                           {
                               companyName = mobile.brand,
                               models = from model in mobile.BikeModels
                                        where model.model != "" && model.status != "p"
                                        select new
                                        {
                                            model = model.model
                                        }
                           }).AsEnumerable();
            return Ok(mobiles);
        }
        public async Task<IHttpActionResult> GetBrands()
        {
            var brands = db.CarBrands.Where(x => x.brand != "" && x.status != "p").Select(x => x.brand);
            return Ok(brands);
        }
        public async Task<IHttpActionResult> GetModels(string brand)
        {
            var models = db.CarModels.Where(x => x.CarBrand.brand.Equals(brand) && x.status != "p").Select(x => x.model);
            return Ok(models);
        }
        public async Task<IHttpActionResult> GetBikeBrands()
        {
            var brands = db.BikeBrands.Where(x => x.brand != "" && x.status != "p").Select(x => x.brand);
            return Ok(brands);
        }
        public async Task<IHttpActionResult> GetBikeModels(string brand)
        {
            var models = db.BikeModels.Where(x => x.BikeBrand.brand.Equals(brand) && x.status != "p").Select(x => x.model);
            return Ok(models);
        }
        public async Task<IHttpActionResult> SearchCarAds(string brand, string model, string tags, string title, int minPrice, int maxPrice, string city, string pp)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.CarAds
                            where ( ad.Ad.status.Equals("a") && (model == null || ad.CarModel1.model.Equals(model)) && (brand == null || ad.CarModel1.CarBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                category = ad.Ad.category,
                                subcategory = ad.Ad.subcategory,
                                views = ad.Ad.views,
                                condition = ad.Ad.condition,
                                savedCount = ad.Ad.SaveAds.Count,
                                color = ad.color,
                                brand = ad.CarModel1.CarBrand.brand,
                                model = ad.CarModel1.model,
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


            var temp = from ad in db.CarAds
                       where (ad.Ad.status.Equals("a") && (model == null || ad.CarModel1.model.Equals(model)) && (brand == null || ad.CarModel1.CarBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                           category = ad.Ad.category,
                           subcategory = ad.Ad.subcategory,
                           views = ad.Ad.views,
                           condition = ad.Ad.condition,
                           savedCount = ad.Ad.SaveAds.Count,
                           color = ad.color,
                           brand = ad.CarModel1.CarBrand.brand,
                           model = ad.CarModel1.model,
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

        public async Task<IHttpActionResult> SearchBikeAds(string brand, string model, string tags, string title, int minPrice, int maxPrice, string city, string pp)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.BikeAds
                            where (ad.Ad.status.Equals("a") && (model == null || ad.BikeModel1.model.Equals(model)) && (brand == null || ad.BikeModel1.BikeBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                category = ad.Ad.category,
                                subcategory = ad.Ad.subcategory,
                                views = ad.Ad.views,
                                condition = ad.Ad.condition,
                                savedCount = ad.Ad.SaveAds.Count,
                                brand = ad.BikeModel1.BikeBrand.brand,
                                model = ad.BikeModel1.model,
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


            var temp = from ad in db.BikeAds
                       where (ad.Ad.status.Equals("a") && (model == null || ad.BikeModel1.model.Equals(model)) && (brand == null || ad.BikeModel1.BikeBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                           category = ad.Ad.category,
                           subcategory = ad.Ad.subcategory,
                           views = ad.Ad.views,
                           condition = ad.Ad.condition,
                           savedCount = ad.Ad.SaveAds.Count,
                           brand = ad.BikeModel1.BikeBrand.brand,
                           model = ad.BikeModel1.model,
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

        // GET api/Vehicle/5
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

        // PUT api/Vehicle/5
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

        // POST api/Vehicle
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

        // DELETE api/Vehicle/5
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
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCarBrand(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                CarBrand bid = await db.CarBrands.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.CarBrands.Remove(bid);
                var ads = db.Ads.Where(x => x.CarAd.CarModel1.brandId.Equals(id));
                foreach (var ad in ads)
                {
                    db.Ads.Remove(ad);
                }
                await db.SaveChangesAsync();
                return Ok(bid);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCarModel(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                CarModel bid = await db.CarModels.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.CarModels.Remove(bid);
                var ads = db.Ads.Where(x => x.CarAd.CarModel1.Id.Equals(id));
                foreach (var ad in ads)
                {
                    db.Ads.Remove(ad);
                }
                await db.SaveChangesAsync();

                return Ok(bid);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateCarBrand(CarBrand mob)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                mob.addedBy = User.Identity.GetUserId();
                mob.time = DateTime.UtcNow;
                mob.status = "a";
                db.Entry(mob).State = EntityState.Modified;

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
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCareModel(CarModel mob)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                mob.addedBy = User.Identity.GetUserId();
                mob.time = DateTime.UtcNow;
                mob.status = "a";
                try
                {
                    db.Entry(mob).State = EntityState.Modified;
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
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }


        [HttpPost]
        public async Task<IHttpActionResult> UpdateBikeBrand(BikeBrand mob)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                mob.addedBy = User.Identity.GetUserId();
                mob.time = DateTime.UtcNow;
                mob.status = "a";
                db.Entry(mob).State = EntityState.Modified;

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
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBikeModel(BikeModel mob)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                mob.addedBy = User.Identity.GetUserId();
                mob.time = DateTime.UtcNow;
                mob.status = "a";
                try
                {
                    db.Entry(mob).State = EntityState.Modified;
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
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBikeBrand(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                BikeBrand bid = await db.BikeBrands.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.BikeBrands.Remove(bid);
                var ads = db.Ads.Where(x => x.BikeAd.BikeModel1.brandId.Equals(id));
                foreach (var ad in ads)
                {
                    db.Ads.Remove(ad);
                }
                await db.SaveChangesAsync();
                return Ok(bid);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBikeModel(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                BikeModel bid = await db.BikeModels.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.BikeModels.Remove(bid);
                var ads = db.Ads.Where(x => x.BikeAd.BikeModel1.Id.Equals(id));
                foreach (var ad in ads)
                {
                    db.Ads.Remove(ad);
                }
                await db.SaveChangesAsync();

                return Ok(bid);
            }
            return BadRequest();
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