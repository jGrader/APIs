using System.Collections.Generic;
using System.Threading.Tasks;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Interfaces
{
    public interface IGenericTeamActionsRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> AddToTeam(TEntity entity, IEnumerable<UserModel> teamMembers);
        Task<IEnumerable<TEntity>> UpdateForTeam(TEntity entity, IEnumerable<UserModel> teamMembers);
        Task<bool> DeleteForTeam(TEntity existingEntity, IEnumerable<UserModel> teamMembers);
    }
}
