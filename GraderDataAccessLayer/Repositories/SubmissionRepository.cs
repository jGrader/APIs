namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SubmissionRepository : GenericRepository<SubmissionModel>, ISubmissionRepository
    {

        public SubmissionRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<SubmissionModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(s => s.File.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(s => s.File.Entity.Task.CourseId == userId));
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
