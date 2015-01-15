namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;


    public interface IGradeComponentRepository : IGenericRepository<GradeComponentModel>, IDisposable
    {
        Task<IEnumerable<GradeComponentModel>> GetByCourseId(int courseId);
    }
}
