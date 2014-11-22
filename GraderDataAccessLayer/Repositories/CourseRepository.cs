
using System.Data.Entity;
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

        public IEnumerable<Course> GetAll()
        {
            //return await Task.Run(() => _db.Course.Where(c => c.Id > 0));
            return _db.Course;
        }

        public Course Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool Add(Course item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Course item)
        {
            throw new NotImplementedException();
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
