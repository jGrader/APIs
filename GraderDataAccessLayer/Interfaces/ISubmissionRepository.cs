namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    interface ISubmissionRepository
    {
        IEnumerable<SubmissionModel> GetAll();
        IEnumerable<SubmissionModel> GetByEntityId(int id);
        IEnumerable<SubmissionModel> GetByUserId(int id);
        IEnumerable<SubmissionModel> GetBefore(string timestamp);
        IEnumerable<SubmissionModel> GetAfter(string timestamp);
        SubmissionModel Get(int id);
        IEnumerable<SubmissionModel> GetByUserIdAndEntityId(int userId, int entityId);
        bool Add(SubmissionModel item);
        bool Remove(int id);
        bool Update(SubmissionModel item);
        int GetUserId(int id);
        int GetEntityId(int id);
        string GetFileName(int id);
        string GetFilePath(int id);
        string GetTimeStamp(int id);
    }
}
