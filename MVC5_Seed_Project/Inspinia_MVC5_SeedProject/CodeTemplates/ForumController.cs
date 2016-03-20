using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class ForumController : Controller
    {
        private Entities db = new Entities();

        // GET: /Forum/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Mobiles()
        {
            ViewBag.type = "mobiles";
            return View("Index");
        }
        [Route("Forum/Details/{id?}/{title?}")]
        public ActionResult Details(int? id,string title = "")
        {
            if (id != null)
            {
                var q = db.Questions.Find(id).title;
                ViewBag.questionId = id;
                ViewBag.title = q;
                return View();
            }
            return View("/NotFound");
        }

        // GET: /Forum/Create
        public ActionResult Create()
        {
            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: /Forum/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include="Id,category,postedBy,time,title,description")] Question question)
        {
            if (User.Identity.IsAuthenticated) { 
            if (ModelState.IsValid)
            {

                question.time = DateTime.UtcNow;
                question.postedBy = User.Identity.GetUserId();
                db.Questions.Add(question);

                string s = Request["tags"];
                s = s.Trim();
                string[] values = s.Split(',');
                Tag []tags = new Tag[values.Length];
                QuestionTag []qt = new QuestionTag[values.Length];
                //int count = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    string ss = values[i];
                    if (ss != "")
                    {
                        var data = db.Tags.FirstOrDefault(x => x.name.Equals(ss, StringComparison.OrdinalIgnoreCase));

                        tags[i] = new Tag();
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


                db.SaveChanges();
                
                for (int i = 0; i < values.Length; i++)
                {
                    qt[i] = new QuestionTag();
                    qt[i].questionId = question.Id;
                    qt[i].tagId = tags[i].Id;
                    db.QuestionTags.Add(qt[i]);
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = question.Id, title = ElectronicsController.URLFriendly(question.title) });
            }

            ViewBag.postedBy = new SelectList(db.AspNetUsers, "Id", "Email", question.postedBy);
            return View(question);
            }
            return View(question);
        }

        // GET: /Forum/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            var tags = question.QuestionTags.Select(x => x.Tag.name).ToArray();
            ViewBag.tags = tags;
            return View(question);
        }

        // POST: /Forum/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,category,subCategory,postedBy,time,title,description")] Question question)
        {
            if (ModelState.IsValid)
            {
                SaveQuestionTags(Request["tags"], question, true);
                db.Entry(question).State = EntityState.Modified;
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
                return RedirectToAction("Details", new {id= question.Id,title = ElectronicsController.URLFriendly( question.title) });
            }
            return View(question);
        }
        public void SaveQuestionTags(string s, Question q, bool update = false)
        {
            if (update)
            {
                var adid = q.Id;
                var adtags = db.QuestionTags.Where(x => x.questionId.Equals(adid)).ToList();
                foreach (var cc in adtags)
                {
                    db.QuestionTags.Remove(cc);
                }
                db.SaveChanges();
            }
            string[] values = s.Split(',');
            Inspinia_MVC5_SeedProject.Models.Tag[] tags = new Inspinia_MVC5_SeedProject.Models.Tag[values.Length];
            QuestionTag[] qt = new  QuestionTag[values.Length];
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
                    qt[i] = new  QuestionTag();
                    qt[i].questionId = q.Id;
                    qt[i].tagId = tags[i].Id;
                    db.QuestionTags.Add(qt[i]);
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
        // GET: /Forum/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: /Forum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
