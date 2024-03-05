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








            /*
            arventure arventure = await db.arventure.FindAsync(id);
            if (arventure == null)
            {
                return NotFound();
            }

            return Ok(arventure);
            */
        }

        // PUT: api/arventures/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putarventure(int id, arventure arventure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != arventure.id)
            {
                return BadRequest();
            }

            db.Entry(arventure).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!arventureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/arventures
        [ResponseType(typeof(arventure))]
        public async Task<IHttpActionResult> Postarventure(arventure arventure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.arventure.Add(arventure);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = arventure.id }, arventure);
        }

        // DELETE: api/arventures/5
        [ResponseType(typeof(arventure))]
        public async Task<IHttpActionResult> Deletearventure(int id)
        {
            arventure arventure = await db.arventure.FindAsync(id);
            if (arventure == null)
            {
                return NotFound();
            }

            db.arventure.Remove(arventure);
            await db.SaveChangesAsync();

            return Ok(arventure);
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