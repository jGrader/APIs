namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    interface ITeamRepository
    {
        IEnumerable<TeamModel> GetAll();
        TeamModel Get(int id);
        bool Add(TeamModel item);
        bool Remove(int id);
        bool Update(TeamModel item);
    }
}
