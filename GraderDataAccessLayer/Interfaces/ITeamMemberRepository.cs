

namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    interface ITeamMemberRepository
    {
        IEnumerable<TeamMemberModel> GetAll();
        IEnumerable<UserModel> GetUsersByTeamId(int id);
        IEnumerable<UserModel> GetTeammatesByUserId(int id);
        TeamMemberModel Get(int id);
        bool Add(TeamMemberModel item);
        bool AddUserToTeam(int userId, int teamId);
        bool Remove(int id);
        bool Update(TeamMemberModel item);
    }
}
