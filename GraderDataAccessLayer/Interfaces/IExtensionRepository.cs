namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IExtensionRepository : IDisposable
    {
        Task<ExtensionModel> Get(int extensionId);

        Task<IEnumerable<ExtensionModel>> GetAll();
        Task<IEnumerable<ExtensionModel>> GetAllForEntity(int entityId);
        Task<IEnumerable<ExtensionModel>> GetAllForUser(int userId);
        Task<IEnumerable<Models.ExtensionModel>> GetAllByLambda(Expression<Func<ExtensionModel, bool>> e);

        Task<ExtensionModel> Add(ExtensionModel item);
        Task<ExtensionModel> Update(ExtensionModel item);
        Task<bool> Delete(int id);
    }
}
