namespace GraderDataAccessLayer.Interfaces
{
    using GraderDataAccessLayer.Models;
    using System;
    using System.Collections.Generic;

    interface ISubmissionRepository
    {
        IEnumerable<SubmissionModel> GetAll();
        IEnumerable<SubmissionModel> GetByEntityId(int id);
        IEnumerable<SubmissionModel> GetByUserId(int id);
        IEnumerable<SubmissionModel> GetBefore(DateTime timestamp);
        IEnumerable<SubmissionModel> GetAfter(DateTime timestamp);
        IEnumerable<SubmissionModel> GetByUserIdAndEntityId(int userId, int entityId);

        IEnumerable<FileModel> GetAllFiles();
        IEnumerable<FileModel> GetFileByEntityId(int id);
        IEnumerable<FileModel> GetFilesBefore(DateTime timestamp);
        IEnumerable<FileModel> GetFilesAfter(DateTime timestamp);
        IEnumerable<FileModel> GetFilesByUserIdAndEntityId(int userId, int entityId);

        SubmissionModel Get(int id);
        bool Add(SubmissionModel item);
        bool Remove(int id);
        bool Update(SubmissionModel item);
        /* Why do these exist?
        int GetUserId(int id);
        int GetEntityId(int id);
        string GetFileName(int id);
        string GetFilePath(int id);
        string GetTimeStamp(int id);
        */
    }
}
