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
    public class arventuresController : ApiController
    {
        private abp__politecnics_com_dam01Entities db = new abp__politecnics_com_dam01Entities();

        // GET: api/arventures
        public IQueryable<arventure> Getarventure()
        {
            //Evita que cargue los datos relacionados (y así cargar sólo las areventures per se)
            db.Configuration.LazyLoadingEnabled = false;
            return db.arventure;
        }

        // GET: api/arventures/5
        [ResponseType(typeof(arventure))]
        public async Task<IHttpActionResult> Getarventure(int id)
        {
            IHttpActionResult result;
            //Evita que cargue los datos relacionados (y así cargar sólo las arventures per se)
            db.Configuration.LazyLoadingEnabled = false;

            //cicles cicles = await db.cicles.FindAsync(id);

            //Queremos que nos devuelva ciclos con curso
            arventure _arventure = await db.arventure
                .Include("route")
                .Include("story")
                .Where(c => c.id == id)
                .FirstOrDefaultAsync();

            if (_arventure == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_arventure);
            }

            return result;
        }

        // PUT: api/arventures/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putarventure(int id, arventure _arventure)
        {
            IHttpActionResult result;
            String msg = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _arventure.id)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_arventure).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!arventureExists(id))
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


        // POST: api/arventures
        [ResponseType(typeof(arventure))]
        public async Task<IHttpActionResult> Postarventure(arventure _arventure)
        {

            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.arventure.Add(_arventure);
                String msg = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _arventure.id }, _arventure);

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

        // DELETE: api/arventures/5
        [ResponseType(typeof(arventure))]
        public async Task<IHttpActionResult> Deletearventure(int id)
        {
            IHttpActionResult result;

            arventure _arventure = await db.arventure.FindAsync(id);
            if (_arventure == null)
            {
                result = NotFound();

            }
            else
            {
                String msg = "";
                try
                {
                    db.arventure.Remove(_arventure);
                    await db.SaveChangesAsync();
                    result = Ok(_arventure);

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

        private bool arventureExists(int id)
        {
            return db.arventure.Count(e => e.id == id) > 0;
        }
    }
}