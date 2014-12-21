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
using GraderDataAccessLayer;
using GraderDataAccessLayer.Models;

namespace GraderApi.Controllers
{
    public class GradeComponentController : ApiController
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: api/GradeComponentModels
        public IEnumerable<GradeComponentModel> GetGradeComponent()
        {
            return db.GradeComponent;
        }

        // GET: api/GradeComponentModels/5
        [ResponseType(typeof(GradeComponentModel))]
        public async Task<IHttpActionResult> GetGradeComponentModel(int id)
        {
            GradeComponentModel gradeComponentModel = await db.GradeComponent.FindAsync(id);
            if (gradeComponentModel == null)
            {
                return NotFound();
            }

            return Ok(gradeComponentModel);
        }

        // PUT: api/GradeComponentModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGradeComponentModel(int id, GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gradeComponentModel.Id)
            {
                return BadRequest();
            }

            db.Entry(gradeComponentModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GradeComponentModelExists(id))
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

        // POST: api/GradeComponentModels
        [ResponseType(typeof(GradeComponentModel))]
        public async Task<IHttpActionResult> PostGradeComponentModel(GradeComponentModel gradeComponentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GradeComponent.Add(gradeComponentModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = gradeComponentModel.Id }, gradeComponentModel);
        }

        // DELETE: api/GradeComponentModels/5
        [ResponseType(typeof(GradeComponentModel))]
        public async Task<IHttpActionResult> DeleteGradeComponentModel(int id)
        {
            GradeComponentModel gradeComponentModel = await db.GradeComponent.FindAsync(id);
            if (gradeComponentModel == null)
            {
                return NotFound();
            }

            db.GradeComponent.Remove(gradeComponentModel);
            await db.SaveChangesAsync();

            return Ok(gradeComponentModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GradeComponentModelExists(int id)
        {
            return db.GradeComponent.Count(e => e.Id == id) > 0;
        }
    }
}