namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IExcuseRepository : IDisposable
    {
        Task<ExcuseModel> Get(int excuseId);

        Task<IEnumerable<ExcuseModel>> GetAll();
        Task<IEnumerable<ExcuseModel>> GetAllForEntity(int entityId);
        Task<IEnumerable<ExcuseModel>> GetAllForUser(int userId);
        Task<IEnumerable<Models.ExcuseModel>> GetAllByLambda(Expression<Func<ExcuseModel, bool>> e);

        Task<ExcuseModel> Add(ExcuseModel item);
        Task<ExcuseModel> Update(ExcuseModel item);
        Task<bool> Delete(int id);
    }
}
