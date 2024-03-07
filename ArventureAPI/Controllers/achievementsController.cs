using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ArventureAPI.Classes;
using ArventureAPI.Models;

namespace ArventureAPI.Controllers
{
    public class achievementsController : ApiController
    {
        private abp__politecnics_com_dam01Entities db = new abp__politecnics_com_dam01Entities();

        // GET: api/achievements
        public IQueryable<achievement> Getachievement()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.achievement;
        }

        // GET: api/achievements/5
        [ResponseType(typeof(achievement))]
        public async Task<IHttpActionResult> Getachievement(int id)
        {

            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            achievement _achievement = await db.achievement.FindAsync(id);


            if (_achievement == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_achievement);
            }

            return result;
        }

        // PUT: api/achievements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putachievement(int id, achievement _achievement)
        {
            IHttpActionResult result;
            String msg = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _achievement.id)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_achievement).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!achievementExists(id))
                        {
                            result = NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                        msg = MyUtils.ErrorMessage(sqlException);
                        result = BadRequest(msg);
                    }
                }
            }

            return result;
        }

        // POST: api/achievements
        [ResponseType(typeof(achievement))]
        public async Task<IHttpActionResult> Postachievement(achievement _achievement)
        {
            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.achievement.Add(_achievement);
                String msg = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _achievement.id }, _achievement);

                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    msg = MyUtils.ErrorMessage(sqlException);
                    result = BadRequest(msg);
                }
            }
            return result;
        }

        // DELETE: api/achievements/5
        [ResponseType(typeof(achievement))]
        public async Task<IHttpActionResult> Deleteachievement(int id)
        {
            IHttpActionResult result;

            achievement _achievement = await db.achievement.FindAsync(id);
            if (_achievement == null)
            {
                result = NotFound();

            }
            else
            {
                String msg = "";
                try
                {
                    db.achievement.Remove(_achievement);
                    await db.SaveChangesAsync();
                    result = Ok(_achievement);

                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    msg = MyUtils.ErrorMessage(sqlException);
                    result = BadRequest(msg);
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool achievementExists(int id)
        {
            return db.achievement.Count(e => e.id == id) > 0;
        }
    }
}