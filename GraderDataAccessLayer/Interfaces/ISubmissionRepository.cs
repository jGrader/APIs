namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISubmissionRepository
    {
        Task<SubmissionModel> Get(int id);
        Task<IEnumerable<SubmissionModel>> GetAllByEntityId(int id);
        Task<IEnumerable<SubmissionModel>> GetAllByUserId(int id);
        Task<IEnumerable<SubmissionModel>> GetAllBefore(DateTime timestamp);
        Task<IEnumerable<SubmissionModel>> GetAllAfter(DateTime timestamp);
        Task<IEnumerable<SubmissionModel>> GetAllByUserIdAndEntityId(int userId, int entityId);

        Task<IEnumerable<SubmissionModel>> GetAll();

        Task<SubmissionModel> Add(SubmissionModel item);
        Task<SubmissionModel> Update(SubmissionModel item);
        Task<bool> DeleteSubmission(int sumbmissionId);


        Task<FileModel> GetFile(int id);
        Task<IEnumerable<FileModel>> GetAllFiles();
        Task<IEnumerable<FileModel>> GetAllFilesByEntityId(int id);
        Task<IEnumerable<FileModel>> GetAllFilesBefore(DateTime timestamp);
        Task<IEnumerable<FileModel>> GetAllFilesAfter(DateTime timestamp);
        Task<IEnumerable<FileModel>> GetAllFilesByUserIdAndEntityId(int userId, int entityId);

        Task<bool> Add(FileModel item);
        Task<bool> Update(FileModel item);
        Task<bool> DeleteFile(int fileId);
 

        /* Why do these exist?
        int GetUserId(int id);
        int GetEntityId(int id);
        string GetFileName(int id);
        string GetFilePath(int id);
        string GetTimeStamp(int id);
        */
    }
}
