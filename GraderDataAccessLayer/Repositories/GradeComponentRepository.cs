
namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class GradeComponentRepository : GenericRepository<GradeComponentModel>, IGradeComponentRepository
    {

        public GradeComponentRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<GradeComponentModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run( () => dbSet.Where(w => w.CourseId == courseId));
            return searchResult;
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
