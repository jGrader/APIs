namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISubmissionRepository : IGenericRepository<SubmissionModel>, IDisposable
    {
        Task<IEnumerable<SubmissionModel>> GetByCourseId(int courseId);
        Task<IEnumerable<SubmissionModel>> GetByUserId(int userId);
    }
}
