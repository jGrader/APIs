namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;


    public interface ISessionIdRepository : IGenericRepository<SessionIdModel>, IDisposable
    {
        Task<SessionIdModel> GetByUserId(int userId);
        Task<SessionIdModel> GetBySesionId(Guid sessionId);

        Task<SessionIdModel> Add(int userId);

        Task<bool> IsAuthorized(SessionIdModel sessionIdModel);
    }
}
