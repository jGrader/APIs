namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class CourseUserRepository : GenericRepository<CourseUserModel>,ICourseUserRepository
    {

        public CourseUserRepository(DatabaseContext db) 
            : base(db)
        { }

        public async Task<IEnumerable<CourseUserModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.UserId == userId));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetByPermissions(int permissions)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.Permissions == permissions));
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
