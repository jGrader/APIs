namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GradeRepository : GenericRepository<GradeModel>, IGradeRepository
    {

        public GradeRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<GradeModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(g => g.UserId == userId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> GetByGraderId(int graderId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(g => g.GraderId == graderId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(g => g.EntityId == entityId));
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
