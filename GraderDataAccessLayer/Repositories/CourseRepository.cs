
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.Remoting.Messaging;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CourseRepository : ICourseRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public IEnumerable<CourseModel> GetAll()
        {
            //return await Task.Run(() => _db.Course.Where(c => c.Id > 0));
            return _db.Course;
        }

        public CourseModel Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool Add(CourseModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Course.Add(item);
                _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public bool Remove(int id)
        {
            var item = _db.Course.FirstOrDefault(c => c.Id == id);
            if (item == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.Course.Remove(item);
                _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public bool Update(CourseModel item)
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
                _db.SaveChangesAsync();

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
