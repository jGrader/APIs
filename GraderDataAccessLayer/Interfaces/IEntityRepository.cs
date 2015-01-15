namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEntityRepository : IGenericRepository<EntityModel>, IDisposable
    {
        Task<IEnumerable<EntityModel>> GetByTask(int taskId);
        Task<IEnumerable<EntityModel>> GetByName(string name);
        Task<IEnumerable<EntityModel>> GetByCourseId(int courseId);
        Task<IEnumerable<EntityModel>> GetByOpenDate(DateTime openTime);
        Task<IEnumerable<EntityModel>> GetByCloseDate(DateTime closeTime);
        Task<IEnumerable<EntityModel>> GetActiveBetween(DateTime time1, DateTime time2);

    }
}
