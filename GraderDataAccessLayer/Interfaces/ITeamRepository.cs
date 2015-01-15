namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamRepository : IGenericRepository<TeamModel>, IDisposable
    {
        Task<IEnumerable<TeamModel>> GetByEntityId(int entityId);
        Task<IEnumerable<TeamModel>> GetByCoureId(int courseId);
    }
}
