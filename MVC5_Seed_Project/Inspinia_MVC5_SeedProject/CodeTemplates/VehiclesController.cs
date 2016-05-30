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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data.Entity.Validation;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class VehiclesController : Controller
    {
        private Entities db = new Entities();
        HttpClient httpClient = new HttpClient();
        string url = "http://localhost:59322/api";
        ElectronicsController electronicController = new ElectronicsController();
        // GET: /Vehicles/


        //[Route("Vehicle/{category?}")]
        //public ActionResult Index(string category)
        //{
        //    ViewBag.subcategory = category;
        //    ViewBag.category = "Vehicles";
        //    return View("Index");
        //}
        public ActionResult Index()
        {
            ViewBag.category = "Vehicles";
            return View("../Education/Categories");
        }
        [Route("Cars")]
        public ActionResult Cars()
        {
            return View();
        }
        [Route("Bikes")]
        public ActionResult Bikes()
        {
            return View();
        }
        [Route("Commerical-Vehicles")]
        public ActionResult CommericalVehicles()
        {
            ViewBag.category = "Vehicles";
            ViewBag.subcategory = "commerical-vehicles";
            return View("Index");
        }
        [Route("Vehicles-for-rent")]
        public ActionResult VehiclesForRent()
        {
            ViewBag.category = "Vehicles";
            ViewBag.subcategory = "vehicles-for-rent";
            return View("Index");
        }
        [Route("Other-Vehicles")]
        public ActionResult OtherVehicles()
        {
            ViewBag.category = "Vehicles";
            ViewBag.subcategory = "other-vehicles";
            return View("Index");
        }
        [Route("Vehicles-spare-parts")]
        public ActionResult SpareParts()
        {
            ViewBag.category = "Vehicles";
            ViewBag.subcategory = "spare-parts";
            return View("Index");
        }
        [Route("Pets-Animals")]
        public ActionResult Animals()
        {
            ViewBag.category = "Animals";
            ViewBag.subcategory = "Animals";
            return View("Index");
        }
        //public ActionResult VehiclesForRent()
        //{
        //    ViewBag.category = "Commerical-Vehicles";
        //    return View("Index");
        //}

        // GET: /Vehicles/Details/5
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
            ViewBag.adId = id;
            return View();
        }
        //public ActionResult Cars()
        //{
        //    return View();
        //}
        //public ActionResult Bikes()
        //{
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {

            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    //string tempId = Request["tempId"];
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = electronicController.MyAd(ad, "Save", "Vehicles", "Cars");
                   
                    electronicController.PostAdByCompanyPage(ad.Id);

                    await saveCarAd(ad);

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
                    electronicController.SaveTags(Request["tags"],  ad);

                    electronicController.ReplaceAdImages( ad, fileNames);
                    //location
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }
        public ActionResult CreateCarAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public IdStatus SaveCarsBrandModel( Ad ad)
        {
           IdStatus adStatus = new IdStatus();
            adStatus.status = "a";
            var company = System.Web.HttpContext.Current.Request["brand"];
            var model = System.Web.HttpContext.Current.Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {
                bool isOldBrand = db.CarBrands.Any(x => x.brand.Equals(company));
                
                if (!isOldBrand)
                {
                    CarBrand mob = new  CarBrand();
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
                    db.CarBrands.Add(mob);
                    db.SaveChanges();

                    CarModel mod = new CarModel();
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
                    db.CarModels.Add(mod);
                    db.SaveChanges();
                    //ad.status = "p";
                    adStatus.status = "p";
                }
                else
                {
                    bool isOldModel = db.CarModels.Any(x => x.model.Equals(model));
                    if (!isOldModel)
                    {
                        adStatus.status = "p";
                     //   ad.status = "p";
                        var brandId = db.CarBrands.FirstOrDefault(x => x.brand.Equals(company));
                        CarModel mod = new  CarModel();
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
                        db.CarModels.Add(mod);
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
                var mobileModel = db.CarModels.FirstOrDefault(x => x.CarBrand.brand == company && x.model == model);
                adStatus.id = mobileModel.Id;
                return adStatus;
            }
        }
        public IdStatus SaveBikesBrandModel(Ad ad)
        {
            IdStatus idStatus = new IdStatus();
            idStatus.status = "a";
            var company = System.Web.HttpContext.Current.Request["brand"];
            var model = System.Web.HttpContext.Current.Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {

                bool isOldBrand = db.BikeBrands.Any(x => x.brand.Equals(company));
                if (!isOldBrand)
                {
                    BikeBrand mob = new  BikeBrand();
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
                    db.BikeBrands.Add(mob);
                    db.SaveChanges();

                    BikeModel mod = new  BikeModel();
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
                    db.BikeModels.Add(mod);
                    db.SaveChanges();
                    //ad.status = "p";
                    idStatus.status = "p";
                }
                else
                {
                    bool isOldModel = db.BikeModels.Any(x => x.model.Equals(model));
                    if (!isOldModel)
                    {
                        idStatus.status = "p";
                        //ad.status = "p";
                        var brandId = db.BikeBrands.FirstOrDefault(x => x.brand.Equals(company));
                        BikeModel mod = new BikeModel();
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
                        db.BikeModels.Add(mod);
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
                var mobileModel = db.BikeModels.FirstOrDefault(x => x.BikeBrand.brand == company && x.model == model);
                idStatus.id = mobileModel.Id;
                return idStatus;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCarAd([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {

            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    //string tempId = Request["tempId"];
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = electronicController.MyAd(ad, "Save", "Vehicles", "Cars");
                    
                    electronicController.PostAdByCompanyPage(ad.Id);

                    await saveCarAd(ad);

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
                    electronicController.SaveTags(Request["tags"],  ad);

                    electronicController.ReplaceAdImages( ad, fileNames);
                    //location
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly( ad.title) });
                }
                return RedirectToAction("Cars", "Vehicles");
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }
        public async Task<string> saveCarAd(Ad ad, bool update = false)
        {
          //  ad.category = "Vehicles";
          //  ad.subcategory = "Cars";
            CarAd mobileAd = new CarAd();
            mobileAd.color = Request["color"];
            if (Request["year"] != "" && Request["year"] != null)
            {
                mobileAd.year = short.Parse(Request["year"]);
            }
            mobileAd.fuelType = Request["fuelType"];
            mobileAd.assembly = Request["assembly"];
            if (Request["kmDriven"] != null && Request["kmDriven"] != "")
            {
                mobileAd.kmDriven = int.Parse(Request["kmDriven"]);
            }
            if (Request["engineCapacity"] != null && Request["engineCapacity"] != "")
            {
                mobileAd.engineCapacity =int.Parse( Request["engineCapacity"]);
            }
            mobileAd.adId = ad.Id;
            if (Request["noOfOwners"] != null && Request["noOfOwners"] != "")
            {
                mobileAd.noOfOwners =  short.Parse(Request["noOfOwners"]);
            }
            if (Request["registeredCity"] != null && Request["registeredCity"] != "")
            {
                var city = Request["registeredCity"];
                 mobileAd.registeredCity =await SaveCity(city);
            }
            mobileAd.transmission = Request["transmission"];
            IdStatus idstatus = SaveCarsBrandModel( ad);
            mobileAd.carModel = idstatus.id;
            ad.status = idstatus.status;
            if (update)
            {
                db.Entry(mobileAd).State = EntityState.Modified;
            }
            else
            {
                db.CarAds.Add(mobileAd);
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
            return ad.status;
        }
        public async Task<string> saveBikeAd(Ad ad, bool update = false)
        {
            //  ad.category = "Vehicles";
            //  ad.subcategory = "Cars";
            BikeAd mobileAd = new  BikeAd();
            if (Request["year"] != "" && Request["year"] != null)
            {
                mobileAd.year = short.Parse(Request["year"]);
            }
            if (Request["kmDriven"] != null && Request["kmDriven"] != "")
            {
                mobileAd.kmDriven = int.Parse(Request["kmDriven"]);
            }
            mobileAd.adId = ad.Id;
            if (Request["noOfOwners"] != null && Request["noOfOwners"] != "")
            {
                mobileAd.noOfOwners = short.Parse(Request["noOfOwners"]);
            }
            if (Request["registeredCity"] != null && Request["registeredCity"] != "")
            {
                var city = Request["registeredCity"];

                mobileAd.registeredCity = await SaveCity(city);
            }
            IdStatus idstatus =SaveBikesBrandModel( ad);
            mobileAd.bikeModel = idstatus.id;

            if (update)
            {
                db.Entry(mobileAd).State = EntityState.Modified;
            }
            else
            {
                db.BikeAds.Add(mobileAd);
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
            return idstatus.status;
        }
        public async Task<int?> SaveCity(string city)
        {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    db.Cities.Add(cit);
                    await db.SaveChangesAsync();
                    return cit.Id ;
                }
                return citydb.Id;
        }
        // GET: /Vehicles/Edit/5
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
            return View(ad);
        }
        public async Task<ActionResult> EditCarAd(int? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCarAd([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
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
                        ad = electronicController.MyAd(ad, "Update", "Vehicles", "Cars");
                       

                        electronicController.PostAdByCompanyPage(ad.Id,true);

                        db.SaveChanges();

                        

                        await saveCarAd(ad,true);

                        

                        //tags
                        electronicController.SaveTags(Request["tags"],  ad, "update");
                        

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location

                        electronicController.ReplaceAdImages( ad, fileNames);
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");

                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title =ElectronicsController.URLFriendly( ad.title) });
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Edit", ad);
        }




        public ActionResult CreateBikeAd()
        {
            Ad ad = new Ad();
            return View(ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBikeAd([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
        {

            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = electronicController.MyAd(ad, "Save", "Vehicles", "Bikes");
                    
                    electronicController.PostAdByCompanyPage(ad.Id);

                    await saveBikeAd(ad);

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
                    electronicController.SaveTags(Request["tags"],  ad);

                    electronicController.ReplaceAdImages( ad, fileNames);
                    //location
                    electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Bikes", "Vehicles");
            }
            TempData["error"] = "Only enter those information about which you are asked";
            return View("Create", ad);
        }
        public async Task<ActionResult> EditBikeAd(int? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBikeAd([Bind(Include = "Id,category,postedBy,title,description,time,price,isnegotiable")] Ad ad)
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
                        ad = electronicController.MyAd(ad, "Update", "Vehicles", "Bikes");


                        electronicController.PostAdByCompanyPage(ad.Id, true);

                        db.SaveChanges();
                        await saveBikeAd(ad, true);
                        electronicController.SaveTags(Request["tags"],  ad, "update");


                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            string sss = e.ToString();
                        }
                        //location

                        electronicController.ReplaceAdImages( ad, fileNames);
                        electronicController.MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");

                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Edit", ad);
        }



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
            return View(ad);
        }

        // GET: /Vehicles/Delete/5
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

        // POST: /Vehicles/Delete/5
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
