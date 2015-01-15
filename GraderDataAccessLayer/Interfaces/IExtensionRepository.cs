namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExtensionRepository : IGenericRepository<ExtensionModel>, IDisposable
    {
        Task<IEnumerable<ExtensionModel>> GetByEntityId(int entityId);
        Task<IEnumerable<ExtensionModel>> GetByUserId(int userId);
    }
}
