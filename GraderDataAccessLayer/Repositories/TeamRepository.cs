namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TeamRepository : GenericRepository<TeamModel>, ITeamRepository
    {

        public TeamRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<TeamModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(e => e.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<TeamModel>> GetByCoureId(int courseId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(e => e.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (context == null)
            {
                return;
            }

            context.Dispose();
            context = null;
        }  
    }
}
