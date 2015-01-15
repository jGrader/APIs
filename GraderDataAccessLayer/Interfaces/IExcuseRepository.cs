namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExcuseRepository : IGenericRepository<ExcuseModel>, IDisposable
    {
        Task<IEnumerable<ExcuseModel>> GetByEntity(int entityId);
        Task<IEnumerable<ExcuseModel>> GetByUser(int userId);
    }
}
