using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class LaptopsComputersController : Controller
    {
        private Entities db = new Entities();

        // GET: /LaptopsComputers/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /LaptopsComputers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.adId = id;
            return View("../Electronics/Details");
        }

        // GET: /LaptopsComputers/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation");
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color");
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color");
            return View();
        }

        // POST: /LaptopsComputers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // GET: /LaptopsComputers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // POST: /LaptopsComputers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable,subcategory,type,condition")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "exectLocation", ad.Id);
            ViewBag.Id = new SelectList(db.LaptopAds, "Id", "color", ad.Id);
            ViewBag.Id = new SelectList(db.MobileAds, "Id", "color", ad.Id);
            return View(ad);
        }

        // GET: /LaptopsComputers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: /LaptopsComputers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ad ad = db.Ads.Find(id);
            db.Ads.Remove(ad);
            db.SaveChanges();
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
