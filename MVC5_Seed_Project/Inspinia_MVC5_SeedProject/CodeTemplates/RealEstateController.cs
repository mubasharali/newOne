using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class RealEstateController : Controller
    {
        public Entities db = new Entities();
        ElectronicsController electronicController = new ElectronicsController();
        // GET: /RealEstate/
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.AspNetUser).Include(a => a.AdsLocation).Include(a => a.MobileAd);
            return View(await ads.ToListAsync());
        }

        // GET: /RealEstate/Details/5
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

        // GET: /RealEstate/Create
        public ActionResult Create()
        {
            Ad ad = new Ad();
            return View(ad);
        }
        public async Task<bool> SaveRealEstateAd(int adId, bool update = false)
        {
            House house = new House();
            if (System.Web.HttpContext.Current.Request["area"] != null && System.Web.HttpContext.Current.Request["area"] != "")
            {
                house.area =int.Parse( System.Web.HttpContext.Current.Request["area"]);
            }
            if (System.Web.HttpContext.Current.Request["bathroom"] != null && System.Web.HttpContext.Current.Request["bathroom"] != "")
            {
                house.bathroom = (System.Web.HttpContext.Current.Request["bathroom"]);
            }
            if (System.Web.HttpContext.Current.Request["bedroom"] != null && System.Web.HttpContext.Current.Request["bedroom"] != "")
            {
                house.bedroom = (System.Web.HttpContext.Current.Request["bedroom"]);
            }
            if (System.Web.HttpContext.Current.Request["floor"] != null && System.Web.HttpContext.Current.Request["floor"] != "")
            {
                house.floor = (System.Web.HttpContext.Current.Request["floor"]);
            }
            house.adId = adId;
            if (update)
            {
                db.Entry(house).State = EntityState.Modified;
            }
            else
            {
                db.Houses.Add(house);
            }
           await db.SaveChangesAsync();
           return true;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,category,subcategory,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {

            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    electronicController.MyAd(ref ad, "Save", ad.category, ad.subcategory);
                    db.Ads.Add(ad);
                    db.SaveChanges();


                    electronicController.PostAdByCompanyPage(ad.Id);

                      SaveRealEstateAd(ad.Id);

                    
                    //tags
                    electronicController.SaveTags(Request["tags"], ref ad);

                    electronicController.ReplaceAdImages(ref ad, fileNames);
                    //location
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }

        // GET: /RealEstate/Edit/5
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
            return View(ad);
        }

        // POST: /RealEstate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,category,subcategory,status,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var ab = Request["postedBy"];
                    var iddd = User.Identity.GetUserId();
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        electronicController.MyAd(ref ad, "Update");

                        db.Entry(ad).State = EntityState.Modified;
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

                        electronicController.PostAdByCompanyPage(ad.Id, true);

                        db.SaveChanges();
                        await SaveRealEstateAd(ad.Id, true);
                        electronicController.SaveTags(Request["tags"], ref ad, "update");


                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location

                        

                        electronicController.ReplaceAdImages(ref ad, fileNames);
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ref ad, "Update");

                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Edit", ad);
        }

        // GET: /RealEstate/Delete/5
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

        // POST: /RealEstate/Delete/5
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
