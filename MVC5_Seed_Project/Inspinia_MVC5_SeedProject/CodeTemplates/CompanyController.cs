using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class CompanyController : Controller
    {
        private Entities db = new Entities();
        
        // GET: /Company/
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.AspNetUser).Include(c => c.City).Include(c => c.popularPlace);
            return View(companies.ToList());
            
        }
        private static readonly string _awsAccessKey =
            ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey =
            ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName =
            ConfigurationManager.AppSettings["Bucketname"];
        private static readonly string _folderName =
            ConfigurationManager.AppSettings["CompanyFolder"];
        public ActionResult uploadLogo()
        {
            int id = int.Parse( Request["id"]);
            HttpPostedFileBase file = Request.Files["fileInput"];
            string extension = System.IO.Path.GetExtension(file.FileName);



            string newFileName = id + "/logo" + extension ;
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = "https://s3.amazonaws.com/";
            Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

            var request2 = new PutObjectRequest()
            {
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                Key = _folderName + newFileName,
                InputStream = file.InputStream//SEND THE FILE STREAM
            };
            s3Client.PutObject(request2);
            var data = db.Companies.Find(id);
            data.logoextension = extension;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();


            //string companyFolder = "~/Images/Company/" + id;
            //if (! System.IO.Directory.Exists(Server.MapPath(companyFolder)))
            //{
            //    System.IO.Directory.CreateDirectory(Server.MapPath(companyFolder));
            //}
            ////filename = "temp" + DateTime.UtcNow.Ticks + extension;
            //file.SaveAs(Server.MapPath(companyFolder + "/logo" + extension ));
            //var data = db.Companies.Find(id);
            //data.logoextension = extension;
            //db.Entry(data).State = EntityState.Modified;
            //db.SaveChanges();
            return RedirectToAction("Details","Company", new { id = id ,title = ElectronicsController.URLFriendly( data.title) });
        }
        public ActionResult Create()
        {
            Company company = new Company();
            return View(company);
        }
        [Route("CompanyPage/{id}/{title?}")]
        public ActionResult Details(int id , string title = null)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyId = company.Id;
            ViewBag.title = company.title;
            return View();
        }
        public ActionResult Ask()
        {
            return View();
        }
        public ActionResult Discussion(int id)
        {
            ViewBag.discussionId = id;
            return View();
        }
        // GET: /Company/Create
        
        public void SaveTags(Company ad)
        {
            string s = Request["tags"];
            string[] values = s.Split(',');
            Inspinia_MVC5_SeedProject.Models.Tag[] tags = new Inspinia_MVC5_SeedProject.Models.Tag[values.Length];
            CompanyTag[] qt = new CompanyTag[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
                string ss = values[i];
                if (ss != "")
                {
                    var data = db.Tags.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                    tags[i] = new Inspinia_MVC5_SeedProject.Models.Tag();
                    if (data != null)
                    {
                        tags[i].Id = data.Id;
                    }
                    else
                    {
                        tags[i].name = values[i];
                        tags[i].time = DateTime.UtcNow;
                        tags[i].createdBy = User.Identity.GetUserId();
                        db.Tags.Add(tags[i]);
                    }
                }
                else
                {
                    tags[i] = null;
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string sb = e.ToString();
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (tags[i] != null)
                {
                    qt[i] = new CompanyTag();
                    qt[i].companyId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.CompanyTags.Add(qt[i]);
                }
            }
        }
        // POST: /Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,title,shortabout,longabout,since,contactNo1,contactNo2,email,fblink,twlink,websitelink,owner,logoextension,category,createdBy,time,status,cityId,popularPlaceId,exectLocation")] Company company)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    company.createdBy = User.Identity.GetUserId();
                    company.time = DateTime.UtcNow;
                    company.status = "a";
                    db.Companies.Add(company);
                    SaveTags(company);
                    try
                    {
                        db.SaveChanges();
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
                    catch (Exception e)
                    {
                        string ss = e.ToString();
                    }
                    return RedirectToAction("Details","Company", new {id = company.Id , title =ElectronicsController.URLFriendly( company.title) });
                }
                
            }

            return View(company);
        }

        // GET: /Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: /Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,title,shortabout,longabout,since,contactNo1,contactNo2,email,fblink,twlink,websitelink,owner,logoextension,category,createdBy,time,status,cityId,popularPlaceId,exectLocation")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: /Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: /Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
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
