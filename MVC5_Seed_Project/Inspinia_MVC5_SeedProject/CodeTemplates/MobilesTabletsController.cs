using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class MobilesTabletsController : Controller
    {
        private Entities db = new Entities();
        public ElectronicsController electronicController = new ElectronicsController();
        // GET: /MobilesTablets/
        public async Task<ActionResult> Index()
        {
            return View();
        }
        //public ActionResult Details(int? id)
        //{
        //    //if (id == null)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}
        //    //Ad ad = db.Ads.Find(id);
        //    //if (ad == null)
        //    //{
        //    //    return HttpNotFound();
        //    //}
        //    ViewBag.title = db.Ads.Find(id).title;
        //    ViewBag.adId = id;
        //    return View("../Electronics/Details");
        //}
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult CreateMobileAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditMobileAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if(ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
                
            }
            return RedirectToAction("Register", "Account");
        }
        public IdStatus SaveMobileBrandModel()
        {
            string adStatus = "a";
            var company = System.Web.HttpContext.Current.Request["brand"];
            var model = System.Web.HttpContext.Current.Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {

                var allBrands = (db.Mobiles.Select(x => x.brand)).AsEnumerable(); //getBrands
                bool isNewBrand = true;
                foreach (var brand in allBrands)
                {
                    if (brand == company)
                    {
                        isNewBrand = false;
                    }
                }
                if (isNewBrand)
                {
                    Mobile mob = new Mobile();
                    mob.brand = company;
                    mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    mob.time = DateTime.UtcNow;
                    if (company == null || company == "")
                    {
                        mob.status = "a";
                    }
                    else
                    {
                        mob.status = "p";
                    }
                    db.Mobiles.Add(mob);
                    db.SaveChanges();

                    MobileModel mod = new MobileModel();
                    mod.model = model;
                    mod.brandId = mob.Id;
                    mod.time = DateTime.UtcNow;
                    if (model == null || model == "")
                    {
                        mod.status = "a";
                    }
                    else
                    {
                        mod.status = "p";
                    }
                    mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    db.MobileModels.Add(mod);
                    db.SaveChanges();
                    adStatus = "p";
                }
                else
                {
                    var allModels = db.MobileModels.Where(x => x.Mobile.brand == company).Select(x => x.model);
                    bool isNewModel = true;
                    foreach (var myModel in allModels)
                    {
                        if (myModel == model)
                        {
                            isNewModel = false;
                        }
                    }
                    if (isNewModel)
                    {
                        adStatus = "p";
                        var brandId = db.Mobiles.FirstOrDefault(x => x.brand.Equals(company));
                        MobileModel mod = new MobileModel();
                        mod.brandId = brandId.Id;
                        mod.model = model;
                        if (model == null || model == "")
                        {
                            mod.status = "a";
                        }
                        else
                        {
                            mod.status = "p";
                        }
                        mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mod.time = DateTime.UtcNow;
                        db.MobileModels.Add(mod);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string s = e.ToString();
                        }
                    }
                }
                var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand == company && x.model == model);
                IdStatus idstatus = new IdStatus();
                idstatus.id = mobileModel.Id;
                idstatus.status = adStatus;
                return idstatus; 
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    //string tempId = Request["tempId"];
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    MobileAd mobileAd = new MobileAd();
                    mobileAd.sims = Request["sims"];
                    mobileAd.color = Request["color"];
                    IdStatus idstatus = SaveMobileBrandModel();
                    mobileAd.mobileId = idstatus.id;
                    ad.status = idstatus.status;
                    ad = electronicController.MyAd(ad, "Save", "Mobiles");
                    
                    electronicController.PostAdByCompanyPage(ad.Id);

                    
                    //images
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string sbs = e.ToString();
                    }
                    //tags
                    electronicController.SaveTags(Request["tags"], ad);
                    // FileUploadHandler(ad);
                    mobileAd.Id = ad.Id;
                    db.MobileAds.Add(mobileAd);
                    //ad.MobileAd.a(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string sbs = e.ToString();
                    }
                    // ReplaceAdImages(ad.Id,tempId,fileNames);
                   electronicController.ReplaceAdImages( ad, fileNames);
                    //location
                   electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMobileAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    MobileAd mobileAd = new MobileAd();
                    mobileAd.color = Request["color"];
                    IdStatus idstatus = SaveMobileBrandModel();
                    mobileAd.mobileId = idstatus.id;
                    ad.status = idstatus.status;
                    ad = electronicController.MyAd(ad, "Save", "MobileAccessories");
                    
                    
                    electronicController.PostAdByCompanyPage(ad.Id);
                    //tags
                    electronicController.SaveTags(Request["tags"], ad);

                    mobileAd.Id = ad.Id;
                    db.MobileAds.Add(mobileAd);
                   electronicController.ReplaceAdImages( ad, fileNames);
                    //location
                   electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        public ActionResult Update([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
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
                        MobileAd mobileAd = new MobileAd();

                        mobileAd.sims = Request["sims"];
                        mobileAd.color = Request["color"];
                        IdStatus idstatus = SaveMobileBrandModel();
                        mobileAd.mobileId = idstatus.id;
                        ad.status = idstatus.status;
                        ad = electronicController.MyAd(ad, "Update");

                        

                        //tags
                        electronicController.SaveTags(Request["tags"],  ad, "update");
                        string brand = Request["brand"];
                        string model = Request["model"];
                        var mobileModel = db.MobileModels.FirstOrDefault(x => x.Mobile.brand == brand  && x.model == model );
                        mobileAd.mobileId = mobileModel.Id;
                       
                        electronicController.PostAdByCompanyPage(ad.Id,true);
                        //db.Ads.Add(ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;
                        //ad.MobileAds.Add(mobileAd);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        electronicController.ReplaceAdImages( ad, fileNames);
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Edit", ad);
        }
        public ActionResult EditAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if(ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMobileAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        MobileAd mobileAd = new MobileAd();

                        mobileAd.sims = Request["sims"];
                        mobileAd.color = Request["color"];
                        IdStatus idstatus = SaveMobileBrandModel();
                        mobileAd.mobileId = idstatus.id;
                        ad.status = idstatus.status;
                        ad = electronicController.MyAd(ad, "Update");


                        //tags
                        electronicController.SaveTags(Request["tags"],  ad, "update");
                        //location
                        electronicController.PostAdByCompanyPage(ad.Id, true);
                       electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        electronicController.ReplaceAdImages( ad, fileNames);

                        
                        
                        //db.Ads.Add(ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;

                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditAd", ad);
        }
        public ActionResult Edit(int id)
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
