using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;
using System.Text;
using Newtonsoft.Json;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.IO;
using System.Xml;
using System.Data.Entity.Validation;
using System.Drawing.Drawing2D;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class FileName
    {
        public string fileName;
        public string fileId;
    }
    public class IdStatus
    {
        public int id;
        public string status;
    }
    public class ElectronicsController : Controller
    {
       
        private Entities db = new Entities();
        //public string subdomainName
        //{
        //    get
        //    {
        //        string s = Request.Url.Host;
        //        var index = s.IndexOf(".");

        //        if (index < 0)
        //        {
        //            return null;
        //        }
        //        var sub = s.Split('.')[0];
        //        if (sub == "www" || sub == "localhsot")
        //        {
        //            return null;
        //        }
        //        return sub;
        //    }
        //}
        // GET: /Electronics/
        //public ActionResult Index()
        //{
        //   /// var ads = db.Ads.Include(a => a.AspNetUser);
        //    return View();
        //}
        public static bool checkCategory(string category, string subcategory = null)
        {
            if (category == "books-sports-hobbies")
            {
                //if (subcategory == "Courses & classes" || subcategory == "Books & Magazines")
                //{
                //    return true;
                //}
                return true;
            }
            if(category == "Home-LifeStyle")
            {
                return true;
            }
            if (category == "Vehicles")
            {
                if (subcategory == "commerical-vehicles" || subcategory == "vehicles-for-rent" || subcategory == "other-vehicles" || subcategory == "spare-parts")
                {
                    return true;
                }
            }
            else if (category == "RealEstate")
            {
                string[] subcategories = { "apartment", "house", "plot & land", "Shop", "Office", "PG & Flatmates", "other commerical places" }; //reference over Realestate/create page + RealEstateSearch.js
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Electronics")
            {
                string[] subcategories = { "TV-DVD-Multimedia","Camera", "Games", "Home-Appliances", "Other-Electronics" };
                foreach (var subcat in subcategories)
                {
                    if (subcategory == subcat)
                    {
                        return true;
                    }
                }
            }
            else if (category == "Animals")
            {
                return true;
            }
            return false;
        }
        public static void sendEmail(string to,string subject, string body){
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress("dealkar.pk@gmail.com");
           // mail.From = new System.Net.Mail.MailAddress("notification@dealkar.pk");
            // The important part -- configuring the SMTP client
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success 587
             smtp.EnableSsl = true; //commented for godaddy
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
            smtp.UseDefaultCredentials = false; // [3] Changed this
            smtp.Credentials = new NetworkCredential("dealkar.pk@gmail.com", "birthdaywish");  // [4] Added this. Note, first parameter is NOT string.
                                                // smtp.Credentials = new NetworkCredential("notification@dealkar.pk", "birthdaywish");
             smtp.Host = "smtp.gmail.com";
           // smtp.Host = "relay-hosting.secureserver.net";
            //recipient address
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            //Formatted mail body
            mail.IsBodyHtml = true;
            // string st = "Test";

            mail.Body = body;
            smtp.Send(mail);
            //try
            //{
            //    smtp.Send(mail);
            //}
            //catch (Exception e)
            //{
            //    string s = e.ToString(); 
            //}
        }

        [Route("Laptops-Computers")]
        public ActionResult ComputersLaptops()
        {
           // sendEmail("irfanyusanif@gmail.com", "Hi i am irfan");
            return View("Laptops");
        }
         [Route("Cameras")]
        public ActionResult Cameras()
        {
            return View();
        }
        public void PostAdByCompanyPage(int adId,bool update = false)
        {
            var postAdUsing = System.Web.HttpContext.Current.Request["postAdUsing"];
            if (postAdUsing != null)
            {
                if (update)
                {
                    var comads = db.CompanyAds.Find(adId);
                    if(comads != null)
                    {
                        db.CompanyAds.Remove(comads);
                        db.SaveChanges();
                    }
                }
                if (postAdUsing != "on")
                {
                    var companyId = db.Companies.FirstOrDefault(x => x.title.Equals(postAdUsing)).Id;
                    CompanyAd comAd = new CompanyAd();
                    comAd.companyId = companyId;
                    comAd.adId = adId;
                    db.CompanyAds.Add(comAd);
                    db.SaveChanges();
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Laptops");
                    LaptopAd mobileAd = new LaptopAd();
                    mobileAd.color = Request["color"];

                    mobileAd.laptopId = SaveLaptopBrandModel(ad);
                    
                    //tags
                    SaveTags(Request["tags"], ad);
                    mobileAd.Id = ad.Id;
                    // ad.LaptopAd.Add(mobileAd);
                    db.LaptopAds.Add(mobileAd);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        string ss = e.ToString();
                    }
                    PostAdByCompanyPage(ad.Id);
                    ReplaceAdImages(  ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }

        public ActionResult CreateLaptopAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult CreateCameraAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult CreateCameraAccessoriesAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditCameraAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditLaptopAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        public ActionResult EditCameraAccessoriesAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }
            }
            return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptopAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "LaptopAccessories");
                    
                    PostAdByCompanyPage(ad.Id);
                    LaptopAd mobileAd = new  LaptopAd();
                    mobileAd.color = Request["color"];

                    mobileAd.laptopId = SaveLaptopBrandModel( ad);

                    //tags
                    SaveTags(Request["tags"],  ad);

                    mobileAd.Id = ad.Id;
                    db.LaptopAds.Add(mobileAd);
                    ReplaceAdImages( ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        public void SaveCameraAd(int adId, bool update = false)
        {
            Camera cam = new Camera();
            cam.category = System.Web.HttpContext.Current.Request["cameraCategory"];
            cam.brand = System.Web.HttpContext.Current.Request["brand"];
            cam.adId = adId;
            if (update)
            {
                db.Entry(cam).State = EntityState.Modified;
            }
            else
            {
                db.Cameras.Add(cam);
            }
            db.SaveChanges();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCameraAd([Bind(Include = "Id,category,subcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Electronics", "Cameras");
                    

                    SaveCameraAd(ad.Id);
                    PostAdByCompanyPage(ad.Id);
                    
                    //tags
                    SaveTags(Request["tags"],  ad);

                    ReplaceAdImages( ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCameraAd([Bind(Include = "Id,category,subcateogry,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");


                        SaveCameraAd(ad.Id, true);

                        //tags
                        SaveTags(Request["tags"],  ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditLaptopAccessoriesAd", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCameraAccessoriesAd([Bind(Include = "Id,category,subcategory,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                    ad = MyAd(ad, "Save", "Electronics", "CamerasAccessories");
                    

                    SaveCameraAd(ad.Id);
                    PostAdByCompanyPage(ad.Id);

                    //tags
                    SaveTags(Request["tags"],  ad);

                    ReplaceAdImages( ad, fileNames);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Save");
                    db.SaveChanges();
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCameraAccessoriesAd([Bind(Include = "Id,category,subcateogry,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");

                        SaveCameraAd(ad.Id, true);

                        //tags
                        SaveTags(Request["tags"],  ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditLaptopAccessoriesAd", ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLaptopAccessoriesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");

                        LaptopAd mobileAd = new LaptopAd();
                        mobileAd.color = Request["color"];

                        mobileAd.laptopId = SaveLaptopBrandModel(ad);

                       mobileAd.laptopId = SaveLaptopBrandModel( ad);
                        mobileAd.Id = ad.Id;
                        db.Entry(mobileAd).State = EntityState.Modified;

                        //tags
                        SaveTags(Request["tags"],  ad, "update");
                        //location

                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"],  ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        db.SaveChanges();
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }

                }
                return RedirectToAction("Register", "Account");
            }
            return View("EditLaptopAccessoriesAd", ad);
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
        public void ReplaceAdImages( Ad ad, FileName[] filenames)
        {
            string newFileName = "";
            int count = 1;
            var id = ad.Id;
            var imaa = db.AdImages.Where(x => x.adId.Equals(id)).Count();
            count = imaa + 1;
            for (int i = 1; i < filenames.Length; i++)
            {
                 IAmazonS3 client;
                 try
                 {
                     using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                     {
                         GetObjectRequest request = new GetObjectRequest
                         {
                             BucketName = _bucketName,
                             Key = _folderName + filenames[i].fileName
                         };
                         using (GetObjectResponse response = client.GetObject(request))
                         {
                             string filename = filenames[i].fileName;
                             if (!System.IO.File.Exists(filename))
                             {
                                 string extension = System.IO.Path.GetExtension(filenames[i].fileName);
                                 newFileName = ad.Id.ToString() + "_" + count + extension;

                                 client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

                                 CopyObjectRequest request1 = new CopyObjectRequest()
                                 {
                                     SourceBucket = _bucketName,
                                     SourceKey = _folderName + filename,
                                     DestinationBucket = _bucketName,
                                     CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                     DestinationKey = _folderName + newFileName
                                 };
                                 CopyObjectResponse response1 = client.CopyObject(request1);

                                 AdImage image = new AdImage();
                                 image.imageExtension = extension;
                                 image.adId = ad.Id;
                                 db.AdImages.Add(image);
                                 db.SaveChanges();
                                 count++;



                                 DeleteObjectRequest deleteObjectRequest =
                                 new DeleteObjectRequest
                                 {
                                     BucketName = _bucketName,
                                     Key = _folderName + filenames[i].fileName
                                 };
                                 AmazonS3Config config = new AmazonS3Config();
                                 config.ServiceURL = "https://s3.amazonaws.com/";
                                 using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(
                                      _awsAccessKey, _awsSecretKey, config))
                                 {
                                     client.DeleteObject(deleteObjectRequest);
                                 }
                             }
                         }
                     }
                 }
                 catch (Exception e)
                 {

                 }
            }
        }

        


        [HttpPost]
        public ActionResult FileUploadHandler()
        {
            string[] fileNames = null;
            bool canpass = true;
            string filename = "";
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (canpass)
                {
                    fileNames = new string[Request.Files.Count];
                    canpass = false;
                }
                try
                {
                    IAmazonS3 client;
                    //string logo = "logo2.png";
                    //using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                    //{
                    //    GetObjectRequest request = new GetObjectRequest
                    //         {
                    //             BucketName = _bucketName,
                    //             Key = logo
                    //         };
                    //    using (GetObjectResponse response = client.GetObject(request))
                    //    {
                         //   string dest =System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), logo);
                            //if (!System.IO.File.Exists(logo))
                            //{
                            //    response.WriteResponseStreamToFile(dest);
                            //}
                            Image imgg = Image.FromFile(Server.MapPath(@"\Images\others\WaterMark.png"));
                            float f = float.Parse("1"); //0.5
                            Image img = SetImageOpacity(imgg, f);

                            HttpPostedFileBase file = Request.Files[i];
                    using (Image image = Image.FromStream(file.InputStream, true, true))
                    //  using (Image watermarkImage = Image.FromFile(Server.MapPath(@"\Images\others\WaterMark.png")))
                    using (Image watermarkImage = img)
                    {
                       //Image  image = ResizeImage(image12, 600, 600);
                        using (Graphics imageGraphics = Graphics.FromImage(image))
                        using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
                        {
                            // int x = (image.Width / 2 - watermarkImage.Width / 2);
                            int x = 4;
                            int y = image.Height - watermarkImage.Height-30;
                         //   int y = (image.Height / 2 - watermarkImage.Height / 2);
                            watermarkBrush.TranslateTransform(x, y);
                            imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                            
                            //upload on aws
                            string extension = System.IO.Path.GetExtension(file.FileName);
                            filename = "temp" + DateTime.UtcNow.Ticks + extension;
                            var i2 = new Bitmap(image);
                            //  image.Save(Server.MapPath(@"~\Images\Ads\" + filename));
                            System.IO.Directory.CreateDirectory(Server.MapPath(@"~\Images\Ads\"));

                            i2.Save(Server.MapPath(@"~\Images\Ads\" + filename));
                            if (file.ContentLength > 0) // accept the file
                            {
                                AmazonS3Config config = new AmazonS3Config();
                                config.ServiceURL = "https://s3.amazonaws.com/";
                                Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

                                var request2 = new PutObjectRequest()
                                {
                                    BucketName = _bucketName,
                                    CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                                    Key = _folderName + filename,
                                    //InputStream = file.InputStream//SEND THE FILE STREAM
                                    FilePath = Server.MapPath(@"~\Images\Ads\" + filename)
                                };
                                s3Client.PutObject(request2);
                            }
                            if (System.IO.File.Exists(Server.MapPath(@"~\Images\Ads\" + filename)))
                            {
                                System.IO.File.Delete(Server.MapPath(@"~\Images\Ads\" + filename));
                            }
                        }
                    }
                    //  }
                    fileNames[i] = filename;
                    }
               // }
                catch (Exception ex)
                {
                    return Json("Error");
                }
            }
            return Json(fileNames);
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static void UploadDPToAWS(string path, string filename)
        {
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = "https://s3.amazonaws.com/";
            Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config);

            var request2 = new PutObjectRequest()
            {
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                Key = _userFolder + filename,
                //InputStream = file.InputStream//SEND THE FILE STREAM
                FilePath = path
            };
            s3Client.PutObject(request2);
        }
        public Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        } 
        public int SaveLaptopBrandModel(Ad ad)
        {
            ad.status = "a";
            var company = Request["brand"];
            var model = Request["model"];
            if (company != null && company != "")
            {
                company = company.Trim();
                model = model.Trim();
            }
            if (true) //company != null
            {
                bool isOldBrand = db.LaptopBrands.Any(x => x.brand.Equals(company));
                if (!isOldBrand)
                {
                    if(company != "" && company != null && company != "undefined")
                    {
                        ad.status = "p";
                        LaptopBrand mob = new LaptopBrand();
                        mob.brand = company;
                        mob.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        mob.time = DateTime.UtcNow;
                        mob.status = "p";
                        db.LaptopBrands.Add(mob);
                        db.SaveChanges();
                        if(model != "" && model != null && model != "undefined")
                        {
                            LaptopModel mod = new LaptopModel();
                            mod.model = model;
                            mod.brandId = mob.Id;
                            mod.time = DateTime.UtcNow;
                            mod.status = "p";
                            mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (model != "" && model != null && model != "undefined")
                    {
                        bool isOldModel = db.LaptopModels.Any(x => x.model.Equals(model));
                        if (!isOldModel)
                        {
                            ad.status = "p";
                            var brandId = db.LaptopBrands.FirstOrDefault(x => x.brand.Equals(company));
                            LaptopModel mod = new LaptopModel();
                            mod.brandId = brandId.Id;
                            mod.model = model;
                            mod.status = "p";
                            mod.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            mod.time = DateTime.UtcNow;
                            db.LaptopModels.Add(mod);
                            db.SaveChanges();
                        }
                    }
                }
                var laptopModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                return laptopModel.Id;
            }
        }
        
        
        public ActionResult Home_Appliances()
        {
            return View();
        }
        public ActionResult CreateHomeAppliancesAd()
        {
            if (Request.IsAuthenticated)
            {

                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        public Ad MyAd( Ad ad,string SaveOrUpdate,string cateogry = null,string subcategory = null)
        {
            if (ad.status == null || ad.status == "")
            {
                ad.status = "a";
            }
            var type = System.Web.HttpContext.Current.Request["type"];
            var isbiding = System.Web.HttpContext.Current.Request["bidingAllowed"];
            var condition = System.Web.HttpContext.Current.Request["condition"];
            var pp = System.Web.HttpContext.Current.Request["price"];
            string[] prices = pp.Split(',');   //exception: object reference not set to instance of object
            if (type == "sell")
            {
                ad.type = true;
            }
            else
            {
                ad.type = false;
            }
            if (isbiding == "fixedPrice")
            {
                pp = prices[0];
                var nn = System.Web.HttpContext.Current.Request["isNegotiable"];
                if (nn == "on")
                {
                    ad.isnegotiable = "y";
                }
                else
                {
                    ad.isnegotiable = "n";
                }
            }
            else if (isbiding == "allowBiding")
            {
                pp = prices[1];
                ad.isnegotiable = "b";
            }
            
            if (condition == "new")
            {
                ad.condition = "n";
            }
            else if (condition == "unboxed")
            {
                ad.condition = "b";
            }
            else
            {
                ad.condition = "u";
            }
            if (condition == null)
            {
                ad.condition = "z";
            }
            if (pp != null && pp != "")
            {
                ad.price = int.Parse(pp);
            }
            //ad.description = ad.description.Replace("'", "`");
            //  ad.description = System.Web.HttpUtility.HtmlEncode(ad.description);
            ad.postedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (SaveOrUpdate == "Save")
            {
                ad.category = cateogry;
                ad.subcategory = subcategory==null?"":subcategory;
                ad.time = DateTime.UtcNow;
                db.Ads.Add(ad);
                
            }
            else if (SaveOrUpdate == "Update")
            {
                ad.time =DateTime.Parse(System.Web.HttpContext.Current.Request["time"]);
                if (ad.category == null)
                {
                    ad.category = System.Web.HttpContext.Current.Request["category"];
                }
                if (ad.subcategory == null)
                {
                    ad.subcategory = System.Web.HttpContext.Current.Request["subcategory"];
                }
                db.Entry(ad).State = EntityState.Modified;
            }
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
            return ad;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHomeAppliancesAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                   ad =  MyAd( ad,"Save","Electronics","HomeAppliances");


                    //tags
                    SaveTags(Request["tags"], ad);
                    //location
                    MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Save");
                    return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                }
                return RedirectToAction("Register", "Account");
            }
            return View("CreateHomeAppliancesAd", ad);
            //ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", ad.postedBy);
            //return View(ad);
        }
        
        public void MyAdLocation(string city, string popularPlace, string exectLocation, Ad ad,string SaveOrUpdate)
        {
            AdsLocation loc = new AdsLocation();
            if (city != null)
            {
                var citydb = db.Cities.FirstOrDefault(x => x.cityName.Equals(city, StringComparison.OrdinalIgnoreCase));
                if (citydb == null)
                {
                    City cit = new City();
                    cit.cityName = city;
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    cit.addedOn = DateTime.UtcNow;
                    cit.status = "p";
                    db.Cities.Add(cit);
                    db.SaveChanges();
                    loc.cityId = cit.Id;
                    if (popularPlace != null)
                    {
                        popularPlace pop = new popularPlace();
                        try
                        {
                            Coordinates co = GetLongitudeAndLatitude(popularPlace,city);
                            if (co.status)
                            {
                                pop.longitude = co.longitude;
                                pop.latitude = co.latitude;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                        
                        pop.cityId = cit.Id;
                        pop.name = popularPlace;
                        pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        pop.addedOn = DateTime.UtcNow;
                        pop.status = "p";
                        db.popularPlaces.Add(pop);
                        db.SaveChanges();
                        loc.popularPlaceId = pop.Id;
                    }
                }
                else
                {
                    loc.cityId = citydb.Id;
                    if (popularPlace != null)
                    {
                        var ppp = db.popularPlaces.FirstOrDefault(x => x.City.cityName.Equals(city, StringComparison.OrdinalIgnoreCase) && x.name.Equals(popularPlace, StringComparison.OrdinalIgnoreCase));
                        if (ppp == null)
                        {
                            popularPlace pop = new popularPlace();
                            try
                            {
                                Coordinates co = GetLongitudeAndLatitude(popularPlace,city);
                                if (co.status)
                                {
                                    pop.longitude = co.longitude;
                                    pop.latitude = co.latitude;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            pop.cityId = citydb.Id;
                            pop.name = popularPlace;
                            pop.addedBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
                            pop.addedOn = DateTime.UtcNow;
                            pop.status = "p";
                            db.popularPlaces.Add(pop);
                            
                            db.SaveChanges();
                            loc.popularPlaceId = pop.Id;
                        }
                        else
                        {
                            loc.popularPlaceId = ppp.Id;
                        }
                    }
                }
                loc.exectLocation = exectLocation == "undefined"?null:exectLocation;
                loc.Id = ad.Id;
                if (SaveOrUpdate == "Save")
                {
                    ad.AdsLocation = loc;
                   // db.AdsLocations.Add(loc);
                }
                else if (SaveOrUpdate == "Update")
                {
                    db.Entry(loc).State = EntityState.Modified;
                }
                    db.SaveChanges();
                
            }
        }
        [Route("TV-DVD-Multimedia")]
        public ActionResult tv()
        {
            ViewBag.subcategory = "TV-DVD-Multimedia";
            ViewBag.category = "Electronics";
            return View("../Vehicles/Index");
        }
        [Route("Other-Electronics")]
        public ActionResult otherelectronics()
        {
            ViewBag.subcategory = "Other-Electronics";
            ViewBag.category = "Electronics";
            return View("../Vehicles/Index");
        }
        [Route("Home-Appliances")]
        public ActionResult homeappliances()
        {
            ViewBag.subcategory = "Home-Appliances";
            ViewBag.category = "Electronics";
            return View("../Vehicles/Index");
        }
        [Route("Games")]
        public ActionResult games()
        {
            ViewBag.subcategory = "Games";
            ViewBag.category = "Electronics";
            return View("../Vehicles/Index");
        }

        //[Route("Electronics/{category?}")]
        //public ActionResult Index(string category)
        //{
        //    ViewBag.subcategory = category;
        //    ViewBag.category = "Vehicles";
        //    return View("Index");
        //}
        //[HttpPost]
        
        public ActionResult EditLaptopAd(int id)
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = db.Ads.Find(id);
                if (ad.postedBy == User.Identity.GetUserId())
                {
                    return View(ad);
                }

            }
            return RedirectToAction("Register", "Account");
        }
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
        public void SaveTags(string s, Ad ad,string addOrUpdate = "add")
        {
            if(addOrUpdate == "update")
            {
                var adid = ad.Id;
                var adtags = db.AdTags.Where(x => x.adId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.AdTags.Remove(cc);
                }
                 db.SaveChanges();
            }
            string[] values = s.Split(',');
            Inspinia_MVC5_SeedProject.Models.Tag[] tags = new Inspinia_MVC5_SeedProject.Models.Tag[values.Length];
            AdTag[] qt = new AdTag[values.Length];
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
                        tags[i].createdBy = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
                    qt[i] = new AdTag();
                    qt[i].adId = ad.Id;
                    qt[i].tagId = tags[i].Id;
                    db.AdTags.Add(qt[i]);
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
        }
        public ActionResult UpdateLaptopAd([Bind(Include = "Id,category,postedBy,title,description,time")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    if (Request["postedBy"] == User.Identity.GetUserId())
                    {
                        FileName[] fileNames = JsonConvert.DeserializeObject<FileName[]>(Request["files"]);
                        ad = MyAd(ad, "Update");
                        LaptopAd mobileAd = new LaptopAd();

                        mobileAd.color = Request["color"];
                        var company = Request["brand"];
                        var model = Request["model"];

                        SaveLaptopBrandModel(ad);
                        //tags
                        SaveTags(Request["tags"], ad,"update");

                        var mobileModel = db.LaptopModels.FirstOrDefault(x => x.LaptopBrand.brand == company && x.model == model);
                        mobileAd.laptopId = mobileModel.Id;
                       
                        
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
                        MyAdLocation(Request["city"], Request["popularPlace"], Request["exectLocation"], ad, "Update");
                        ReplaceAdImages( ad, fileNames);
                        return RedirectToAction("Details", "Electronics", new { id = ad.Id, title = ElectronicsController.URLFriendly(ad.title) });
                    }
                }
                return RedirectToAction("Register", "Account");
            }
            return View("Create", ad);
        }
        [Route("Details/{id?}/{title?}")]
        public ActionResult Details(int? id,string title = null)
        {
            ViewBag.adId = id;
            Ad data = db.Ads.Find(id);
            if(data == null){
                return RedirectToAction("notFound","Home");
            }
            ViewBag.title = data.title;
            return View();
        }
        
        // GET: /Electronics/Create
        
        public ActionResult CreateLaptopAd()
        {
            if (Request.IsAuthenticated)
            {
                Ad ad = new Ad();
                return View(ad);
            }
            return RedirectToAction("Register", "Account");
        }
        
        
        // POST: /Electronics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        

        // POST: /Electronics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        

        // GET: /Electronics/Delete/5
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
        public Coordinates test(string abc,string city)
        {
            return GetLongitudeAndLatitude(abc,city);
           // return GetCoordinates(abc);
        }
        public class Coordinates
        {
            public decimal longitude;
            public decimal latitude;
            public bool status;
        }
        public static Coordinates GetLongitudeAndLatitude(string famousPlace,string city)
        {
            Coordinates co = new Coordinates();
            co.status = false;
            //string urlAddress = "https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBilH9FSqKqoahGM2ImsDB4XAMNiQASPsQ&address=" + HttpUtility.UrlEncode(famousPlace) + "&region=" + city + "&sensor=false";
            string urlAddress = "https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBilH9FSqKqoahGM2ImsDB4XAMNiQASPsQ&address=" + HttpUtility.UrlEncode(famousPlace + " " +city)  + "&sensor=false";
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(urlAddress);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/GeocodeResponse/result/geometry/location");
                if (objXmlNodeList.Count == 1)
                {
                    co.status = true;
                }
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    // GET LONGITUDE 
                    co.longitude = decimal.Parse ( objXmlNode.ChildNodes.Item(0).InnerText);
                    // GET LATITUDE 
                    co.latitude= decimal.Parse ( objXmlNode.ChildNodes.Item(1).InnerText);
                }
            }
            catch
            {
                // Process an error action here if needed  
            }
            return co;
        }
        
        
        // POST: /Electronics/Delete/5
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
