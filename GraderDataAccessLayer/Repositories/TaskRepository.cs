
namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;


    public class TaskRepository : GenericRepository<TaskModel>, ITaskRepository
    {

        public TaskRepository(DatabaseContext db) : base(db)
        { }

        public async Task<TaskModel> Get(int id)
        {
            var searchResult = await dbSet.FirstOrDefaultAsync(t => t.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<TaskModel>> GetByName(string name)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.Name == name));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetByGradeComponentId(int gradeComponentId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.GradeComponentId == gradeComponentId));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetByCourseIdAndGradeComponentId(int courseId, int gradeComponentId)
        {
            var searchResult = await Task.Run(() => dbSet.Where(w => w.CourseId == courseId && w.GradeComponentId == gradeComponentId));
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
