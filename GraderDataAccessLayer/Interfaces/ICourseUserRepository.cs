using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Interfaces
{
    public interface ICourseUserRepository
    {
        IEnumerable<CourseUserModel> GetAll();
        IEnumerable<CourseUserModel> Get(int id);
        IEnumerable<CourseUserModel> GetByCourseId(int courseId);
        IEnumerable<CourseUserModel> GetByUser(int userId);
        IEnumerable<CourseUserModel> GetByExtensionLimit(int extLimit);
        IEnumerable<CourseUserModel> GetByExcuseLimit(int excLimit);
        IEnumerable<CourseUserModel> GetByPermissions(int permissions);
    }
}
