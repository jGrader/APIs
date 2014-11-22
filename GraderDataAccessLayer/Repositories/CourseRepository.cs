namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using System.Data;
    using System.Data.Entity.Core;
    using System.Data.Entity.Infrastructure;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Interfaces;

    public class CourseRepository : ICourseRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public IEnumerable<CourseModel> GetAll()
        {
            return _db.Course;
        }

        public CourseModel Get(int id)
        {
            var item = _db.Course.FirstOrDefault(c => c.Id == id);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            return item;
        }

        public async Task<bool> Add(CourseModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Course.Add(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> Remove(int id)
        {
            var item = _db.Course.FirstOrDefault(c => c.Id == id);
            if (item == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.Course.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> Update(CourseModel item)
        {
            var dbItem = _db.Course.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Course.Remove(dbItem);
                _db.Course.Add(item);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (DBConcurrencyException)
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
            _db = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
