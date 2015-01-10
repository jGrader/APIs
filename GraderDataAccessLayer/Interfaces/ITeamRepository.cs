namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamRepository : IDisposable
    {
        Task<TeamModel> Get(int teamId);

        Task<IEnumerable<TeamModel>> GetAll();
        Task<IEnumerable<TeamModel>> GetAllForEntity(int entityId);
        Task<IEnumerable<TeamModel>> GetAllForCourse(int courseId);

        Task<TeamModel> Add(TeamModel item);
        Task<TeamModel> Update(TeamModel item);
        Task<bool> Delete(int teamId);
    }
}
