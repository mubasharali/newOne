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

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class ServicesController : Controller
    {
        private Entities db = new Entities();

        // GET: /Services/
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.AdsLocation).Include(a => a.LaptopAd).Include(a => a.MobileAd).Include(a => a.AspNetUser).Include(a => a.CompanyAd);
            return View(await ads.ToListAsync());
        }

        // GET: /Services/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: /Services/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation");
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color");
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color");
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id = new SelectList(db.CompanyAds, "adId", "rating");
            return View();
        }

        // POST: /Services/Create
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
            return View(ad);
        }

        // GET: /Services/Edit/5
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
            return View(ad);
        }

        // POST: /Services/Edit/5
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
            return View(ad);
        }

        // GET: /Services/Delete/5
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

        // POST: /Services/Delete/5
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
