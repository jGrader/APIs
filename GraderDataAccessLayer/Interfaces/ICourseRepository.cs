
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
        Task<bool> Add(CourseModel item);
        Task<bool> Remove(int id);
        Task<bool> Update(CourseModel item);
    }
}
