using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inspinia_MVC5_SeedProject.Models;
namespace Inspinia_MVC5_SeedProject.CodeTemplates
{
    public class TagController : Controller
    {
        public Entities db = new Entities();
        //
        // GET: /Tag/
       [Route("Tag/{id}/{name?}")]
        public ActionResult Index(int  id,string name = null)
        {
            var data = db.Tags.FirstOrDefault(x=>x.Id.Equals(id));
            if (data != null)
            {
                ViewBag.tagId = data.Id;
                return View(data);
            }
            return RedirectToAction("../not-found");
        }

        //
        // GET: /Tag/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Tag/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tag/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Tag/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Tag/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Tag/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Tag/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
