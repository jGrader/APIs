namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class EntityRepository : GenericRepository<EntityModel>, IEntityRepository
    {

        public EntityRepository(DatabaseContext db)
            : base(db)
        { }

        public async Task<IEnumerable<EntityModel>> GetByTask(int taskId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.TaskId == taskId));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetByName(string name)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.Name == name));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.Task.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetByOpenDate(DateTime openTime)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.OpenTime == openTime));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetByCloseDate(DateTime closeTime)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.CloseTime == closeTime));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetActiveBetween(DateTime time1, DateTime time2)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.OpenTime > time1 && e.CloseTime < time2));
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

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
        }
    }
}
