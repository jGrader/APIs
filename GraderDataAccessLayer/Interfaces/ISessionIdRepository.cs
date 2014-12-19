namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface ISessionIdRepository : IDisposable
    {
        SessionIdModel GetBySesionId(Guid sessionId);
        SessionIdModel Get(int userId);
        bool IsAuthorized(SessionIdModel sessionIdModel);
    }
}
