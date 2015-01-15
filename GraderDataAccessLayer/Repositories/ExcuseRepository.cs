namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExcuseRepository : GenericRepository<ExcuseModel>, IExcuseRepository
    {

        public ExcuseRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<ExcuseModel>> GetByEntity(int entityId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(c => c.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<ExcuseModel>> GetByUser(int userId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(c => c.UserId == userId));
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
