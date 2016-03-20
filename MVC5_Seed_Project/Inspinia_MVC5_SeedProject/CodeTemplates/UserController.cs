using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using Inspinia_MVC5_SeedProject.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class UserController : Controller
    {
        private Entities db = new Entities();

        //[Route("User/{s?}")]
        public ActionResult Index(string id)
        {
            //return View(db.AspNetUsers.ToList());
            var data = db.AspNetUsers.Find(id);
            if (data == null)
            {
                return RedirectToAction("../not-found");
            }
            ViewBag.id = id;
            return View(data);
        }
        public ActionResult Profile(string id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == User.Identity.GetUserId())
                {
                    ViewBag.userId = id;
                    return View();
                }
                return RedirectToAction("../User/Profile", new { id = User.Identity.GetUserId() });
            }
            return RedirectToAction("../Home/Index");
        }
        private static readonly string _awsAccessKey =
            ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey =
            ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName =
            ConfigurationManager.AppSettings["Bucketname"];
        private static readonly string _folderName =
            ConfigurationManager.AppSettings["UserFolder"];
        public ActionResult saveProfilePic()
        {
            if (Request.IsAuthenticated)
            {
                string s = Request["userId"];
                if (s == User.Identity.GetUserId())
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string id = User.Identity.GetUserId();
                    var user = db.AspNetUsers.Find(id);

                    string extension = System.IO.Path.GetExtension(file.FileName);
                    string newFileName = "p" + User.Identity.GetUserId() + extension;
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
                    user.dpExtension = extension;
                    db.SaveChanges();
                }


                


                //for (int i = 0; i < Request.Files.Count; i++)
                //{
                //    string id = User.Identity.GetUserId();
                //    var user = db.AspNetUsers.Find(id);
                //    System.IO.File.Delete(Server.MapPath(@"~\Images\Users\p" + id + user.dpExtension));
                //    HttpPostedFileBase file = Request.Files[i];
                //    string extension = System.IO.Path.GetExtension(file.FileName);
                //    file.SaveAs(Server.MapPath(@"~\Images\Users\p" + User.Identity.GetUserId() + extension));
                //    user.dpExtension = extension;
                //    db.SaveChanges();
                //}
            }
            return RedirectToAction("../User/Profile", new { id = User.Identity.GetUserId() });
        }
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspnetuser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspnetuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspnetuser);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspnetuser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspnetuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspnetuser);
        }

        // GET: /User/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            if (aspnetuser == null)
            {
                return HttpNotFound();
            }
            return View(aspnetuser);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspnetuser);
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
