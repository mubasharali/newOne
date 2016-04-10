using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class EducationController : Controller
    {
        private Entities db = new Entities();

        // GET: /Education/
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.AdsLocation).Include(a => a.LaptopAd).Include(a => a.MobileAd).Include(a => a.AspNetUser).Include(a => a.CompanyAd).Include(a => a.JobAd).Include(a => a.CarAd).Include(a => a.BikeAd).Include(a => a.House).Include(a => a.Camera);
            return View(await ads.ToListAsync());
        }
        [Route("Books-Sports-Hobbies")]
     //  [Route("Services")]
        //[Route("abcdef")]
        public ActionResult Categories()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            return View();
           // return RedirectToAction("notFound","Home");
        }
        [Route("Home-LifeStyle")]
        public ActionResult homeL()
        {
            ViewBag.category = "Home-LifeStyle";
            return View("Categories");
        }
        [Route("Books-Magazines")]
        public ActionResult abc()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Books & Magazines";
            return View("Index");
        }
        [Route("Classes")]
        public ActionResult abc6()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Classes";
            return View("Index");
        }
        [Route("Home-tuitions")]
        public ActionResult abc5()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Home tuitions";
            return View("Index");
        }
        [Route("Musical-Instruments")]
        public ActionResult abc4()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Musical Instruments";
            return View("Index");
        }
        [Route("Sports-Equipment")]
        public ActionResult abc3()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Sports Equipment";
            return View("Index");
        }
        [Route("Gym-Fitness")]
        public ActionResult abc2()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "Gym & Fitness";
            return View("Index");
        }
        [Route("other-hobbies")]
        public ActionResult abc1()
        {
            ViewBag.category = "Books-Sports-Hobbies";
            ViewBag.subcategory = "other hobbies";
            return View("Index");
        }
        [Route("Watches")]
        public ActionResult watches()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Watches";
            return View("Index");
        }
        [Route("Clothes")]
        public ActionResult clothes()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Clothes";
            return View("Index");
        }
        [Route("Footware")]
        public ActionResult footware()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Footware";
            return View("Index");
        }
        [Route("Jewellery")]
        public ActionResult jewellery()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Jewellery";
            return View("Index");
        }
        [Route("Baby-Products")]
        public ActionResult babyProducts()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Baby Products";
            return View("Index");
        }
        [Route("Health-and-beauty-products")]
        public ActionResult healthBeautyProducts()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Health and beauty products";
            return View("Index");
        }
        [Route("Furniture")]
        public ActionResult furniture()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Furniture";
            return View("Index");
        }
        [Route("HouseHold")]
        public ActionResult houseHold()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "HouseHold";
            return View("Index");
        }
        [Route("Home-Decoration")]
        public ActionResult HomeDecoration()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "Home Decoration";
            return View("Index");
        }
        [Route("others-in-home-and-lifeStyle")]
        public ActionResult othersInHome()
        {
            ViewBag.category = "Home-LifeStyle";
            ViewBag.subcategory = "others in home and lifeStyle";
            return View("Index");
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Education/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // GET: /Education/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // POST: /Education/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition,status,views")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating", ad.Id);
            ViewBag.Id = new SelectList(db.JobAds, "adId", "qualification", ad.Id);
            ViewBag.Id = new SelectList(db.CarAds, "adId", "color", ad.Id);
            ViewBag.Id = new SelectList(db.BikeAds, "adId", "adId", ad.Id);
            ViewBag.Id = new SelectList(db.Houses, "adId", "bedroom", ad.Id);
            ViewBag.Id = new SelectList(db.Cameras, "adId", "brand", ad.Id);
            return View(ad);
        }

        // GET: /Education/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: /Education/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            db.Ads.Remove(ad);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
