namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface ISubmissionRepository : IDisposable
    {
        Task<SubmissionModel> Get(int id);
        
        Task<IEnumerable<SubmissionModel>> GetAll();
        Task<IEnumerable<SubmissionModel>> GetAllByCourseId(int courseId);
        Task<IEnumerable<SubmissionModel>> GetAllByUserId(int userId);
        Task<IEnumerable<SubmissionModel>> GetAllByLambda(Expression<Func<SubmissionModel, bool>> exp);

        Task<SubmissionModel> Add(SubmissionModel item);
        Task<bool> DeleteSubmission(int sumbmissionId);
    }
}
