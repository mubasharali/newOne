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
using System.Web;
using System.Web.Security;
using System.Data.Entity.Validation;
using Inspinia_MVC5_SeedProject.Models;
using System.IO;
using Amazon.S3.Model;
using Amazon.S3;
using System.Configuration;
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class Image
    {
        public string extension;
        public long size;
    }
    public class ElectronicController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Electronic
        public IQueryable<Ad> GetAds()
        {
            return db.Ads;
        }
        public async Task<IHttpActionResult> getAdImages(int id)
        {
            var ad = await db.Ads.FindAsync(id);
            if (ad != null)
            {

                var ret = ad.AdImages.Select(x => x.imageExtension);
                Image [] img  = new Image[ret.Count()];
                for (int ij = 0; ij < ret.Count(); ij++)
                {
                    img[ij] = new Image();
                }
                int i = 0;
                try
                {
                    foreach (var ext in ret)
                    {
                        img[i++].extension = ext;
                      //  img[i++].size = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Images\Ads\" + id + "_" + i + ext)).Length;
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                return Ok(img);
            }
            //File f = "";
            return NotFound();
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetMobileTree()
        {
            var mobiles = (from mobile in db.Mobiles
                           where mobile.brand != "" && mobile.status != "p"
                          select new
                          {
                              id = mobile.Id,
                              companyName = mobile.brand,
                              models = from model in mobile.MobileModels
                                       where model.model != "" && model.status != "p"
                                       select new
                                       {
                                           id = model.Id,
                                           model = model.model
                                       }
                          }).AsEnumerable();
            return Ok(mobiles);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetLaptopTree()
        {
            var mobiles = (from mobile in db.LaptopBrands.ToList()
                           where mobile.status != "p" && mobile.brand != ""
                           orderby mobile.Id
                           select new
                           {
                               id = mobile.Id,
                               companyName = mobile.brand,
                               models = from model in mobile.LaptopModels
                                        where model.model != "" && model.status != "p"
                                        select new
                                        {
                                            id = model.Id,
                                            model = model.model
                                        }
                           }).AsEnumerable();
            return Ok(mobiles);
        }
        
        public async Task<IHttpActionResult> GetBrandsWithTime(int daysAgo,string MobileOrLaptop)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysAgo);
            DateTime days = DateTime.UtcNow - duration;
            if (MobileOrLaptop == "Mobiles")
            {
                var ret = from mob in db.Mobiles
                          where mob.time >= days
                          select new
                          {
                              id = mob.Id,
                              brand = mob.brand,
                              time = mob.time,
                              addedById = mob.addedBy,
                              status = mob.status,
                              addedByName = mob.AspNetUser.Email
                          };
                return Ok(ret);
            }
            if (MobileOrLaptop == "Laptops")
            {
                var retu = from b in db.LaptopBrands
                           where b.time >= days
                           select new
                           {
                               id = b.Id,
                               brand = b.brand,
                               time = b.time,
                               status = b.status,
                               addedById = b.AspNetUser.Id,
                               addedByName = b.AspNetUser.Email
                           };
                return Ok(retu);
            }
            if (MobileOrLaptop == "Bikes")
            {
                var retu = from b in db.BikeBrands
                           where b.time >= days
                           select new
                           {
                               id = b.Id,
                               brand = b.brand,
                               time = b.time,
                               status = b.status,
                               addedById = b.AspNetUser.Id,
                               addedByName = b.AspNetUser.Email
                           };
                return Ok(retu);
            }
            if (MobileOrLaptop == "Cars")
            {
                var retua = from b in db.CarBrands
                            where b.time >= days
                            select new
                            {
                                id = b.Id,
                                brand = b.brand,
                                time = b.time,
                                status = b.status,
                                addedById = b.AspNetUser.Id,
                                addedByName = b.AspNetUser.Email
                            };
                return Ok(retua);
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> GetModelsWithTime(int daysAgo,string MobileOrLaptop)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysAgo);
            DateTime days = DateTime.UtcNow - duration;
            if (MobileOrLaptop == "Mobiles")
            {
                var ret = from mob in db.MobileModels
                          where mob.time >= days
                          select new
                          {
                              id = mob.Id,
                              brand = mob.Mobile.brand,
                              brandId = mob.brandId,
                              model = mob.model,
                              status = mob.status,
                              time = mob.time,
                              addedById = mob.AspNetUser.Id,
                              addedByName = mob.AspNetUser.Email,
                          };
                return Ok(ret);
            }
            if (MobileOrLaptop == "Laptops")
            {
                var retu = from mob in db.LaptopModels
                           where mob.time >= days
                           select new
                           {
                               id = mob.Id,
                               brand = mob.LaptopBrand.brand,
                               brandId = mob.brandId,
                               model = mob.model,
                               time = mob.time,
                               status = mob.status,
                               addedById = mob.AspNetUser.Id,
                               addedByName = mob.AspNetUser.Email,
                           };
                return Ok(retu);
            }
            if (MobileOrLaptop == "Bikes")
            {
                var retu = from mob in db.BikeModels
                           where mob.time >= days
                           select new
                           {
                               id = mob.Id,
                               brand = mob.BikeBrand.brand,
                               brandId = mob.brandId,
                               model = mob.model,
                               time = mob.time,
                               status = mob.status,
                               addedById = mob.AspNetUser.Id,
                               addedByName = mob.AspNetUser.Email,
                           };
                return Ok(retu);
            }
            if (MobileOrLaptop == "Cars")
            {
                var retua = from mob in db.CarModels
                            where mob.time >= days
                            select new
                            {
                                id = mob.Id,
                                brand = mob.CarBrand.brand,
                                brandId = mob.brandId,
                                model = mob.model,
                                time = mob.time,
                                status = mob.status,
                                addedById = mob.AspNetUser.Id,
                                addedByName = mob.AspNetUser.Email,
                            };
                return Ok(retua);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateMobileBrand(Mobile mob)
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
        public async Task<IHttpActionResult> UpdateLaptopBrand(LaptopBrand mob)
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
        public async Task<IHttpActionResult> UpdateMobileModel( MobileModel mob)
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
        public async Task<IHttpActionResult> UpdateLaptopModel(LaptopModel mob)
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
        public async Task<IHttpActionResult> DeleteMobileBrand(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Mobile bid = await db.Mobiles.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.Mobiles.Remove(bid);
                
                var ads = db.Ads.Where(x => x.MobileAd.MobileModel.brandId.Equals(id));
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
        public async Task<IHttpActionResult> DeleteMobileModel(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                MobileModel bid = await db.MobileModels.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.MobileModels.Remove(bid);
                var ads = db.Ads.Where(x => x.MobileAd.MobileModel.Id.Equals(id));
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
        public async Task<IHttpActionResult> DeleteLaptopBrand(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                LaptopBrand bid = await db.LaptopBrands.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.LaptopBrands.Remove(bid);
                var ads = db.Ads.Where(x => x.LaptopAd.LaptopModel.brandId.Equals(id));
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
        public async Task<IHttpActionResult> DeleteLaptopModel(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                LaptopModel bid = await db.LaptopModels.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.LaptopModels.Remove(bid);
                var ads = db.Ads.Where(x => x.LaptopAd.LaptopModel.Id.Equals(id));
                foreach (var ad in ads)
                {
                    db.Ads.Remove(ad);
                }
                await db.SaveChangesAsync();
                return Ok(bid);
            }
            return BadRequest();
        }
        public async Task<IHttpActionResult> GetLaptopBrands()
        {
            var brands = db.LaptopBrands.Where(x => x.brand != "" && x.status != "p").Select(x => x.brand);
            //var brands =  db.LaptopBrands.Select(x => x.brand);
            return Ok(brands);
        }
        public async Task<IHttpActionResult> GetLaptopModels(string brand)
        {
            var models = db.LaptopModels.Where(x => x.LaptopBrand.brand.Equals(brand) && x.status != "p").Select(x => x.model);
            //var models =  db.LaptopModels.Where(x => x.LaptopBrand.brand == brand).Select(x => x.model);
            return Ok(models);
        }
        public async Task<IHttpActionResult> GetBrands()
        {
            var brands =  db.Mobiles.Where(x=>x.brand != "" && x.status != "p").Select(x => x.brand);
            //var brands = await db.Mobiles.ToListAsync();
            return Ok(brands);
        }
        public async Task<IHttpActionResult> GetModels(string brand)
        {
            var models = db.MobileModels.Where(x=>x.Mobile.brand.Equals(brand) && x.status != "p").Select(x=>x.model);
            //var models =await db.MobileModels.Where(x => x.Mobile == brand).ToListAsync();
            return Ok(models);
        }
        public async Task<IHttpActionResult> SearchAds( string tags, string title, string city, string pp)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null || tags == "undefined")
            {
                var temp1 = from ad in db.Ads
                            where ( ad.status.Equals("a") && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                            orderby ad.time descending
                            select new
                            {
                               // companyId = ad.CompanyAd.companyId,
                              //  companyName = ad.CompanyAd.Company.title,
                               // isAdmin = isAdmin,
                                title = ad.title,
                                postedById = ad.AspNetUser.Id,
                                postedByName = ad.AspNetUser.Email,
                                description = ad.description,
                                id = ad.Id,
                                time = ad.time,
                                status = ad.status,
                                islogin = islogin,
                                isNegotiable = ad.isnegotiable,
                                price = ad.price,
                                reportedCount = ad.Reporteds.Count,
                                isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                                views = ad.views,
                                condition = ad.condition,
                                type = ad.type,
                                isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                                savedCount = ad.SaveAds.Count,
                                category = ad.category,
                                subCategory = ad.subcategory,
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
                       where (ad.status.Equals("a") && (!tagsArray.Except(ad.AdTags.Select(x => x.Tag.name)).Any()) && (city == null || city == "undefined" || ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.AdsLocation.popularPlace.name.Equals(pp))))
                       orderby ad.time descending
                       select new
                       {
                           // companyId = ad.CompanyAd.companyId,
                           //  companyName = ad.CompanyAd.Company.title,
                           // isAdmin = isAdmin,
                           title = ad.title,
                           postedById = ad.AspNetUser.Id,
                           postedByName = ad.AspNetUser.Email,
                           description = ad.description,
                           id = ad.Id,
                           time = ad.time,
                           status = ad.status,
                           islogin = islogin,
                           isNegotiable = ad.isnegotiable,
                           price = ad.price,
                           reportedCount = ad.Reporteds.Count,
                           isReported = ad.Reporteds.Any(x => x.reportedBy == islogin),
                           views = ad.views,
                           condition = ad.condition,
                           type = ad.type,
                           isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                           savedCount = ad.SaveAds.Count,
                           category = ad.category,
                           subCategory = ad.subcategory,
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

        public async Task<IHttpActionResult> SearchMobileAds(string brand, string model,string tags,string title, int minPrice, int maxPrice,string city, string pp,bool isAccessories)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if(tags == null)
            {
                var temp1 = from ad in db.MobileAds
                           where ( ( ( isAccessories && ad.Ad.category.Equals("MobileAccessories") ) || (!isAccessories && ad.Ad.category == "Mobiles"  ) ) && ad.Ad.status.Equals("a") && (model == null || ad.MobileModel.model.Equals(model)) && (brand == null || ad.MobileModel.Mobile.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp)) ) )
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
                               views = ad.Ad.views,
                               condition = ad.Ad.condition,
                               savedCount = ad.Ad.SaveAds.Count,
                               color = ad.color,
                               sims = ad.sims,
                               brand = ad.MobileModel.Mobile.brand,
                               model = ad.MobileModel.model,
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
           
           
                var temp = from ad in db.MobileAds
                           where (((isAccessories && ad.Ad.category.Equals("MobileAccessories")) || (!isAccessories && ad.Ad.category == "Mobiles" )) && ad.Ad.status.Equals("a") && (model == null || ad.MobileModel.model.Equals(model)) && (brand == null || ad.MobileModel.Mobile.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                               color = ad.color,
                               sims = ad.sims,
                               brand = ad.MobileModel.Mobile.brand,
                               model = ad.MobileModel.model,
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
       
        public async Task<IHttpActionResult> SearchLaptopAds(string brand, string model, string tags, string title, int minPrice, int maxPrice, string city, string pp, bool isAccessories)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.LaptopAds
                            where (((isAccessories && ad.Ad.category.Equals("LaptopAccessories")) || (!isAccessories && ad.Ad.category == "Laptops")) && ad.Ad.status.Equals("a") && (model == null || ad.LaptopModel.model.Equals(model)) && (brand == null || ad.LaptopModel.LaptopBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                color = ad.color,
                                brand = ad.LaptopModel.LaptopBrand.brand,
                                model = ad.LaptopModel.model,
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


            var temp = from ad in db.LaptopAds
                       where (((isAccessories && ad.Ad.category.Equals("LaptopAccessories")) || (!isAccessories && (ad.Ad.category == "Laptops"))) && ad.Ad.status.Equals("a") && (model == null || ad.LaptopModel.model.Equals(model)) && (brand == null || ad.LaptopModel.LaptopBrand.brand.Equals(brand)) && (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                           color = ad.color,
                           brand = ad.LaptopModel.LaptopBrand.brand,
                           model = ad.LaptopModel.model,
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
        public async Task<IHttpActionResult> SearchCameras(string brand, string tags, string title, int minPrice, int maxPrice, string city, string pp, bool isAccessories,string category)
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            if (tags == null)
            {
                var temp1 = from ad in db.Cameras
                            where (((isAccessories && ad.Ad.subcategory.Equals("CamerasAccessories")) || (!isAccessories && ad.Ad.subcategory == "Cameras")) && ad.Ad.status.Equals("a") && (brand == null || brand == "undefined" ||  ad.brand.Equals(brand)) && (category == null || category == "undefined" || ad.category.Equals(category)) &&
                            (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                category = ad.category, //this is category of camera not ad
                                brand = ad.brand,
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
            

             var temp = from ad in db.Cameras
                        where (((isAccessories && ad.Ad.subcategory.Equals("CamerasAccessories")) || (!isAccessories && ad.Ad.subcategory == "Cameras")) && ad.Ad.status.Equals("a") && (brand == null || brand == "undefined" || ad.brand.Equals(brand)) && (category == null || category == "undefined" || ad.category.Equals(category)) &&
                        (!tagsArray.Except(ad.Ad.AdTags.Select(x => x.Tag.name)).Any()) &&
                            (minPrice == 0 || ad.Ad.price > minPrice) && (maxPrice == 50000 || ad.Ad.price < maxPrice) && (city == null || city == "undefined" || ad.Ad.AdsLocation.City.cityName.Equals(city) && (pp == null || pp == "undefined" || ad.Ad.AdsLocation.popularPlace.name.Equals(pp))))
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
                                category = ad.category, //this is category of camera not ad
                                brand = ad.brand,
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

        public async Task<IHttpActionResult> GetMobileTrends()
        {
            string islogin = "";
            if (User.Identity.IsAuthenticated)
            {
                islogin = User.Identity.GetUserId();
            }
            var ret = (from ad in db.MobileAds
                      orderby ad.Ad.views
                      where ad.Ad.AdImages.Count > 0
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
                          color = ad.color,
                          sims = ad.sims,
                          brand = ad.MobileModel.Mobile.brand,
                          model = ad.MobileModel.model,
                          adTags = from tag in ad.Ad.AdTags.ToList()
                                   select new
                                   {
                                       id = tag.tagId,
                                       name = tag.Tag.name,
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
                      });
            return Ok(ret);
        }
        
        //[InitializeSimpleMembership]
        public async Task<IHttpActionResult> GetItem(int id)
        {
            Ad add = await db.Ads.FindAsync(id);
            if (add == null)
            {
                return NotFound();
            }
            //await AdViews(id);
            add.views++;
            db.Entry(add).State = EntityState.Modified;
            await db.SaveChangesAsync();
            string islogin = "";
            string loginUserProfileExtension = "";
            bool isAdmin = false;
            if (User.Identity.IsAuthenticated)
            {
                if (System.Web.Security.Roles.Enabled) {
                    //string adfnadf = "Ok";
                }
                islogin = User.Identity.GetUserId();
                var ide = await db.AspNetUsers.FindAsync(islogin);
                loginUserProfileExtension = ide.dpExtension;

                try
                {
                  //  var user = new ApplicationUser() { UserName = ide.Email };
                  // isAdmin = Roles.IsUserInRole("Admin");
                   // isAdmin = Roles.GetRolesForUser().Contains("Admin");
                }
                catch (Exception e)
                {
                    string ss = e.ToString();
                }
            }
            int companyId ;
            string companyName;
            try
            {
                companyId = db.Ads.Find(id).CompanyAd.companyId;
            }
            catch(Exception e)
            {
                companyId = 0;
            }
            try
            {
                companyName = db.Ads.Find(id).CompanyAd.Company.title;
            }
            catch (Exception e)
            {
                companyName = null;
            }
            var ret =await (from ad in db.Ads
                       where ad.Id == id
                       select new
                       {
                           companyId = companyId,
                           companyName = companyName,
                           isAdmin = isAdmin,
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
                          // views = ad.AdViews.Count,
                          views = ad.views,
                           condition = ad.condition,
                           type = ad.type,
                           isSaved = ad.SaveAds.Any(x => x.savedBy == islogin),
                           savedCount = ad.SaveAds.Count,
                           category = ad.category,
                           subCategory = ad.subcategory,
                           cameraad = new{
                               brand = ad.Camera.brand,
                               category = ad.Camera.category,
                           },
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
                           bikead = new
                           {
                               brand = ad.BikeAd.BikeModel1.BikeBrand.brand,
                               model = ad.BikeAd.BikeModel1.model,
                               year = ad.BikeAd.year,
                               kmDriven = ad.BikeAd.kmDriven,
                               noOfOwners = ad.BikeAd.noOfOwners,
                               registeredCity = ad.BikeAd.City.cityName,
                           },
                           carad = new
                           {
                               color = ad.CarAd.color,
                               brand = ad.CarAd.CarModel1.CarBrand.brand,
                               model = ad.CarAd.CarModel1.model,
                               year= ad.CarAd.year,
                               kmDriven = ad.CarAd.kmDriven,
                               fuelType = ad.CarAd.fuelType,
                               noOfOwners = ad.CarAd.noOfOwners,
                               registeredCity = ad.CarAd.City.cityName,
                               transmission = ad.CarAd.transmission,
                               assembly = ad.CarAd.assembly,
                               engineCapacity = ad.CarAd.engineCapacity,
                           },
                           realestatead = new{
                               bedroom = ad.House.bedroom,
                               bathroom = ad.House.bathroom,
                               area = ad.House.area,
                               floor = ad.House.floor,
                           },
                           jobAd = new
                           {
                               seats = ad.JobAd.seats,
                               qualification = ad.JobAd.qualification,
                               exprience = ad.JobAd.exprience,
                               careerLevel = ad.JobAd.careerLevel,
                               lastDateToApply = ad.JobAd.lastDateToApply,
                               salaryType = ad.JobAd.salaryType,
                               category = ad.JobAd.category1,
                               skills = from skill in ad.JobSkills
                                        select new
                                        {
                                            id = skill.Id,
                                            name = skill.Skill.name,
                                        },
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
                           adImages = from image in ad.AdImages.ToList()
                                      select new
                                      {
                                          imageExtension = image.imageExtension,
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
                           //mobilead = from mobile in ad.MobileAd
                           //           select new
                           //           {
                           //               color = mobile.color,
                           //               condition = mobile.condition,
                           //               sims = mobile.sims,
                           //               brand = mobile.MobileModel.Mobile,
                           //               model = mobile.MobileModel.model,
                           //               //whichType = "mobiles",
                           //           },

                           //carad = from car in ad.CarAds.ToList()
                           //        select new
                           //        {
                           //            color = car.color,
                           //            condition = car.condition,
                           //            brand = car.CarModel.brand,
                           //            model = car.CarModel.model,
                           //            fuelType = car.fuelType,
                           //            year = car.year,
                           //            kmDriven = car.kmDriven,
                           //        },

                           //carad = from car in ad.CarAds.ToList()
                           //        select new
                           //        {
                           //            color = car.color,
                           //            condition = car.condition,
                           //            brand = car.CarModel.brand,
                           //            model = car.CarModel.model,
                           //            fuelType = car.fuelType,
                           //            year = car.year,
                           //            kmDriven = car.kmDriven,
                           //        },
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
                       }).FirstOrDefaultAsync();
            return Ok(ret);
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> postBid(Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(HttpContext.Current.Request.IsAuthenticated))
            {
                return BadRequest("not login");
            }
            bid.postedBy = User.Identity.GetUserId();
            var BidAlreadyPlaced = await db.Bids.FirstOrDefaultAsync(x => x.adId.Equals(bid.adId) && x.postedBy.Equals(bid.postedBy));
            if (BidAlreadyPlaced != null)
            {
                BidAlreadyPlaced.price = bid.price;
                //db.Bids.Add(BidAlreadyPlaced);
            }
            else
            {
                bid.time = DateTime.UtcNow;
                db.Bids.Add(bid);
            }
            
            await db.SaveChangesAsync();
            var ret = db.Bids.Where(x => x.Id == bid.Id).Select(x => new
            {
                price = x.price,
                postedById = x.postedBy,
                postedByName = x.AspNetUser.Email,
                time = x.time,
                id = x.Id,
            }).FirstOrDefault();
            return Ok(ret);
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBid(Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            db.Entry(bid).State = EntityState.Modified;

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
        public async Task<IHttpActionResult> DeleteBid(int id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Bid bid = await db.Bids.FindAsync(id);
                if (bid == null)
                {
                    return NotFound();
                }
                db.Bids.Remove(bid);
                await db.SaveChangesAsync();
                return Ok(bid);
            }
            return BadRequest();
        }
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
        public async Task<IHttpActionResult> Refresh(int id)
        {
            var ad =await db.Ads.FindAsync(id);
            if (ad != null)
            {
                ad.time = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return Ok("Done");
            }
            return NotFound();
        }
        // POST api/Electronic
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
        // DELETE api/Electronic/5
        [HttpPost]
        public async Task<IHttpActionResult> DeleteAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            
            IAmazonS3 client;


            DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest();
            multiObjectDeleteRequest.BucketName = _bucketName;
            var adImages = ad.AdImages.ToList();
            int count = 1;
            foreach (var image in adImages)
            {
                multiObjectDeleteRequest.AddKey(_folderName + ad.Id + "_" + count + image.imageExtension.Trim(), null);
                count++;
            }
            var userId = User.Identity.GetUserId();
            var email = ad.AspNetUser.UserName;
            var status = db.AspNetUsers.Find(userId).status;
            if (status == "admin")
            {
                if(userId != ad.postedBy){
                    string Body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("/Views/Admin/Email/DeleteAdAlert.html"));
                    Body = Body.Replace("#AdTitle#", "Cake is for sale");
                    ElectronicsController.sendEmail("m.irfanwatoo@gmail.com", "Your item is deleted by admin!", Body);
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
            try
            {
                try { 
                var companyads = ad.CompanyAd;
            db.CompanyAds.Remove(companyads);
                }catch(Exception e)
                {

                }
            db.Ads.Remove(ad);
            
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
            return Ok(ad);
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
        //public async Task<IHttpActionResult> AdViews(int id)
        //{
        //    Ad ad = await db.Ads.FindAsync(id);
        //    if (ad == null)
        //    {
        //        return NotFound();
        //    }
        //    var userId = User.Identity.GetUserId();
        //    if (userId != null)
        //    {
        //        var isAlreadyViewed = ad.AdViews.Any(x => x.viewedBy == userId);
        //        if (isAlreadyViewed)
        //        {
        //            return Ok();
        //        }
        //        AdView rep = new AdView();
        //        rep.viewedBy = userId;
        //        rep.adId = id;
        //        db.AdViews.Add(rep);
        //        await db.SaveChangesAsync();

        //        return Ok();
        //    }
        //    else
        //    {
        //        string ip = GetIPAddress();
        //        var isAlreadyViewed = ad.AdViews.Any(x => x.viewedBy == ip);
        //        if (isAlreadyViewed)
        //        {
        //            return Ok();
        //        }
        //        AdView rep = new AdView();
        //        rep.viewedBy = ip;
        //        rep.adId = id;
        //        db.AdViews.Add(rep);
        //        await db.SaveChangesAsync();
        //        return Ok();
        //    }

        //}
        public async Task<IHttpActionResult> ReportAd(Reported report)
        {
            var userId = User.Identity.GetUserId();
            if(userId != null)
            {
                Ad ad = await db.Ads.FindAsync(report.adId);
                if (ad == null)
                {
                    return NotFound();
                }
                var isAlreadyReported = ad.Reporteds.Any(x => x.reportedBy == userId);
                if (isAlreadyReported)
                {
                    return BadRequest("You can report an Ad only once.If something really wrong you can contact us");
                }
                report.reportedBy = userId;
                db.Reporteds.Add(report);
                await db.SaveChangesAsync();

                var count = ad.Reporteds.Count;

                return Ok(count);
            }
            else{
                return BadRequest("Not login");
            }
            
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