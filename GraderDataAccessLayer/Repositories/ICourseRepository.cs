
namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface ICourseRepository : IDisposable
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(int id);
        Task<bool> Add(Course item);
        Task<bool> Remove(int id);
        Task<bool> Update(Course item);
    }
}
