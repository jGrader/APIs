using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Repositories
{
    public class CourseUserRepository : ICourseUserRepository
    {
        private readonly DatabaseContext _context = new DatabaseContext();

        public IEnumerable<CourseUserModel> GetAll()
        {
            var result = _context.CourseUser.Where(c => c.Id > 0);
            return result;
        }

        public CourseUserModel Get(int id)
        {
            var result = _context.CourseUser.FirstOrDefault(cu => cu.Id == id);
            if (result == null)
            {
                throw new NotImplementedException();
            }
            return result;
        }

        public IEnumerable<CourseUserModel> GetByCourseId(int courseId)
        {
            var result = _context.CourseUser.Where(cu => cu.CourseId == courseId);
            return result;
        }

        public IEnumerable<CourseUserModel> GetByUser(int userId)
        {
            var result = _context.CourseUser.Where(cu => cu.UserId == userId);
            return result;
        }

        public IEnumerable<CourseUserModel> GetByExtensionLimit(int extLimit)
        {
            var result = _context.CourseUser.Where(cu => cu.ExtensionLimit == extLimit);
            return result;
        }

        public IEnumerable<CourseUserModel> GetByExcuseLimit(int excLimit)
        {
            var result = _context.CourseUser.Where(cu => cu.ExcuseLimit == excLimit);
            return result;
        }

        public IEnumerable<CourseUserModel> GetByPermissions(int permissions)
        {
            var result = _context.CourseUser.Where(cu => cu.Permissions == permissions);
            return result;
        }

        public IEnumerable<CourseUserModel> GetByLambda(Expression<Func<CourseUserModel, bool>> exp)
        {
            var result = _context.CourseUser.Where(exp);
            return result;
        }
    }
}
