namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFileRepository : IDisposable
    {
        Task<FileModel> Get(int id);

        Task<IEnumerable<FileModel>> GetAll();
        Task<IEnumerable<FileModel>> GetAllByCourseId(int courseId);
        Task<IEnumerable<FileModel>> GetAllByEntityId(int entityId);

        Task<FileModel> Add(FileModel item);
        Task<FileModel> Update(FileModel item);
        Task<bool> Delete(int id);
    }
}
