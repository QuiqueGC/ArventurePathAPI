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
    public class usersController : ApiController
    {
        private abp__politecnics_com_dam01Entities db = new abp__politecnics_com_dam01Entities();

        // GET: api/users
        public IQueryable<user> Getuser()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.user;
        }

        // GET: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Getuser(int id)
        {
            IHttpActionResult result;
            //Evita que cargue los datos relacionados (y así cargar sólo las arventures per se)
            db.Configuration.LazyLoadingEnabled = false;

            user _user = await db.user
                .Include("achievement")
                .Where(c => c.id == id)
                .FirstOrDefaultAsync();

            //user _user = await db.user.FindAsync(id);

            if (_user == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_user);
            }

            return result;
        }


        /// <summary>
        /// Actualiza el usuario
        /// </summary>
        /// <param name="id">int con el id del usuario</param>
        /// <param name="_user">el objeto usuario</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/users/update/{id}")]
        public async Task<IHttpActionResult> UpdateUser (int id, user _user)
        {

            IHttpActionResult result;
            String msg = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _user.id)
                {
                    result = BadRequest();
                }
                else
                {

                    var userEntity = db.user.Include(u => u.achievement).FirstOrDefault(u => u.id == id);
                    if (userEntity != null) 
                    {
                        // Agregar nuevos logros a la lista de logros del usuario
                        foreach (var newAchievement in _user.achievement)
                        {
                            // Verifica si el logro ya existe en la lista del usuario
                            if (!userEntity.achievement.Any(a => a.id == newAchievement.id))
                            {
                                // Si no existe, agrégalo a la lista
                                userEntity.achievement.Add(newAchievement);
                            }
                        }
                    }

                    //db.Entry(_user).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!userExists(id))
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


        // POST: api/users
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Postuser(user _user)
        {
            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.user.Add(_user);
                String msg = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _user.id }, _user);

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



        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putuser(int id, user _user)
        {
            IHttpActionResult result;
            String msg = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _user.id)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_user).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!userExists(id))
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

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Deleteuser(int id)
        {
            IHttpActionResult result;

            user _user = await db.user.FindAsync(id);
            if (_user == null)
            {
                result = NotFound();

            }
            else
            {
                String msg = "";
                try
                {
                    db.user.Remove(_user);
                    await db.SaveChangesAsync();
                    result = Ok(_user);

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

        private bool userExists(int id)
        {
            return db.user.Count(e => e.id == id) > 0;
        }
    }
}