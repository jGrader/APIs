namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileRepository :  GenericRepository<FileModel>,IFileRepository
    {

        public FileRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<FileModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(f => f.Entity.Task.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<FileModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(f => f.EntityId == entityId));
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
