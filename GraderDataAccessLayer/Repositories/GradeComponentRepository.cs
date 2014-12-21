using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Repositories
{
    public class GradeComponentRepository : IGradeComponentRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public IEnumerable<GradeComponentModel> GetAll(int courseId)
        {
            var searchResult = _db.GradeComponent.Where(c => c.CourseId == courseId);
            return searchResult;
        }

        public async Task<GradeComponentModel> Get(int id)
        {
            var searchResult = await _db.GradeComponent.FirstOrDefaultAsync(c => c.Id == id);
            return searchResult;
        }

        public async Task<bool> Add(GradeComponentModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.GradeComponent.Add(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (RetryLimitExceededException)
            {
                return false;
            }
        }

        public async Task<bool> Update(GradeComponentModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (RetryLimitExceededException)
            {
                return false;
            }
        }

        public async Task<bool> Remove(int id)
        {
            var item = _db.GradeComponent.FirstOrDefault(c => c.Id == id);
            if (item == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.GradeComponent.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (RetryLimitExceededException)
            {
                return false;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_db == null)
            {
                return;
            }

            _db.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
