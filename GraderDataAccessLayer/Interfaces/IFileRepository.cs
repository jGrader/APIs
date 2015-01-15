namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFileRepository : IGenericRepository<FileModel>, IDisposable
    {
        Task<IEnumerable<FileModel>> GetByCourseId(int courseId);
        Task<IEnumerable<FileModel>> GetByEntityId(int entityId);
    }
}
