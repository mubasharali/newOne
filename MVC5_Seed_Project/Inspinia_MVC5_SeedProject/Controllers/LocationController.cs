using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Inspinia_MVC5_SeedProject.Models;

namespace Inspinia_MVC5_SeedProject.Controllers
{
    public class LocationController : ApiController
    {
        private Entities db = new Entities();

        public async Task<IHttpActionResult> SubmitFeedback(Feedback fb)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    fb.givenBy = User.Identity.GetUserId();
                    fb.time = DateTime.UtcNow;
                    db.Feedbacks.Add(fb);
                    await db.SaveChangesAsync();
                    return Ok("Done");
                }
            }
            return BadRequest();
        }
        // GET api/Location
        public async Task<IHttpActionResult> GetCities()
        {
            var cities = (db.Cities.Select(x => x.cityName)).AsEnumerable();
            return Ok(cities);
        }
        public async Task<IHttpActionResult> GetPopularPlaces(string city)
        {
            var places = await db.popularPlaces.Where(x => x.City.cityName == city).Select(x => x.name).ToListAsync();
            return Ok(places);
        }

        // GET api/Location/5
        [ResponseType(typeof(City))]
        public async Task<IHttpActionResult> GetCity(string id)
        {
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }
        
        [HttpPost]
        public async Task<string> SaveCity([FromBody] string city)
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
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
                    return "Done";
                }
                //return Ok(citydb.Id);
            }
           // return BadRequest();
            return "Done";
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCity(City city)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                city.updatedBy = User.Identity.GetUserId();
                city.updatedOn = DateTime.UtcNow;
                db.Entry(city).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdatePopularPlace(popularPlace city)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                city.updatedBy = User.Identity.GetUserId();
                city.updatedOn = DateTime.UtcNow;
                db.Entry(city).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest("Not login");
        }

        // DELETE api/Location/5
        [HttpPost]
        public async Task<IHttpActionResult> DeleteCity(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                City city = null;
                try
                {
                     city = await db.Cities.FirstOrDefaultAsync(x=>x.Id.Equals(id));
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                if (city == null)
                {
                    return NotFound();
                }

                db.Cities.Remove(city);
                await db.SaveChangesAsync();

                return Ok(city);
            }
            return BadRequest("Not login");
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeletePopularPlace(int id)
        {
            if (User.Identity.IsAuthenticated)
            {

                popularPlace city = await db.popularPlaces.FindAsync(id);
                if (city == null)
                {
                    return NotFound();
                }

                db.popularPlaces.Remove(city);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }

                return Ok(city);
            }
            return BadRequest("Not login");
        }
        public async Task<IHttpActionResult> GetCitiesWithTime(int daysAgo)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysAgo);
            DateTime days = DateTime.UtcNow - duration;
                var ret = from mob in db.Cities
                          where mob.addedOn >= days
                          select new
                          {
                              id = mob.Id,
                              cityName = mob.cityName,
                              addedOn = mob.addedOn,
                              addedBy = mob.addedBy,
                              addedByName = mob.AspNetUser.Email,
                              updatedBy = mob.updatedBy,
                              updatedByName = mob.AspNetUser1.Email,
                              updatedOn = mob.updatedOn,
                          };
                return Ok(ret);
        }
        public async Task<IHttpActionResult> GetPPWithTime(int daysAgo)
        {
            TimeSpan duration = DateTime.UtcNow - DateTime.Today.AddDays(-daysAgo);
            DateTime days = DateTime.UtcNow - duration;
            var ret = from mob in db.popularPlaces
                      where mob.addedOn >= days
                      select new
                      {
                          id = mob.Id,
                          name = mob.name,
                          cityName = mob.City.cityName,
                          cityId = mob.cityId,
                          addedOn = mob.addedOn,
                          addedBy = mob.addedBy,
                          addedByName = mob.AspNetUser.Email,
                          updatedBy = mob.updatedBy,
                          updatedByName = mob.AspNetUser1.Email,
                          updatedOn = mob.updatedOn,
                      };
            return Ok(ret);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool CityExists(string id)
        //{
        //    return db.Cities.Count(e => e.Id == id) > 0;
        //}
    }
}