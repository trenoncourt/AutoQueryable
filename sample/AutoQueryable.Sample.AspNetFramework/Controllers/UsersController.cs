using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoQueryable.AspNet.Filter.FilterAttributes;
using AutoQueryable.Sample.AspNetFramework.Models;

namespace AutoQueryable.Sample.AspNetFramework.Controllers
{
    public class UsersController : ApiController
    {
        private readonly AutoQueryableSampleAspNetFrameworkContext _db = new AutoQueryableSampleAspNetFrameworkContext();

        public UsersController()
        {
            _db.Database.CreateIfNotExists();
        }

        // GET: api/Users
        [AutoQueryable]
        public IQueryable<User> GetUsers() => _db.Users;

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _db.Entry(user).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return _db.Users.Count(e => e.Id == id) > 0;
        }
    }
}