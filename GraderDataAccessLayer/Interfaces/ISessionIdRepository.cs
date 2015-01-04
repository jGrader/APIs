namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;


    public interface ISessionIdRepository : IDisposable
    {
        Task<SessionIdModel> GetBySesionId(Guid sessionId);
        Task<SessionIdModel> Get(int userId);

        Task<bool> IsAuthorized(SessionIdModel sessionIdModel);

        Task<SessionIdModel> Add(int userId);
        Task<bool> Delete(int id);
    }
}
