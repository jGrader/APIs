
namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface ICourseRepository : IDisposable
    {
        IEnumerable<Course> GetAll();
        Course Get(int id);
        bool Add(Course item);
        bool Remove(int id);
        bool Update(Course item);
    }
}
