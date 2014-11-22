
namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface ICourseRepository : IDisposable
    {
        IEnumerable<CourseModel> GetAll();
        CourseModel Get(int id);
        bool Add(CourseModel item);
        bool Remove(int id);
        bool Update(CourseModel item);
    }
}
