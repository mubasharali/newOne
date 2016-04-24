using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class AdminController : Controller
    {
        private Entities db = new Entities();
        
        public bool isAdmin()
        {
            if (Request.IsAuthenticated)
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
        // GET: /Admin/
        public async Task<ActionResult> Index()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> SuperAdmin()
        {
            if(User.Identity.GetUserId() == "c1239071-cf6d-4cec-9da4-4b2871250143" || User.Identity.GetUserId() == "7234b5b0-2cb5-4d4a-bc18-98e17c460221")
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Models()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> ManageUsers()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Ads()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Feedback()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        public async Task<ActionResult> Location()
        {
            if (isAdmin())
            {
                return View();
            }
            return RedirectToAction("../not-found");
        }
        // GET: /Admin/Details/5
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

        // GET: /Admin/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId");
            return View();
        }

        // POST: /Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // GET: /Admin/Edit/5
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
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // POST: /Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            ViewBag.Id = new SelectList(db.AdsLocations, "Id", "cityId", ad.Id);
            return View(ad);
        }

        // GET: /Admin/Delete/5
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

        // POST: /Admin/Delete/5
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
