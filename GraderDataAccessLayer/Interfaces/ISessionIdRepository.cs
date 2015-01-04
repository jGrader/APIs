namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ISessionIdRepository : IDisposable
    {
        Task<SessionIdModel> Get(int userId);
        Task<SessionIdModel> GetBySesionId(Guid sessionId);
        Task<IEnumerable<SessionIdModel>> GetAll();

        Task<bool> IsAuthorized(SessionIdModel sessionIdModel);

        Task<SessionIdModel> Add(int userId);
        Task<bool> Delete(int id);
    }
}
