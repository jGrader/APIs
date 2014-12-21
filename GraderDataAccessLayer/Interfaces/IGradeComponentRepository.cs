namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using GraderDataAccessLayer.Models;

    interface IGradeComponentRepository : IDisposable
    {
        IEnumerable<GradeComponentModel> GetAll(int courseId);
        Task<GradeComponentModel> Get(int id);

        Task<bool> Add(GradeComponentModel item);
        Task<bool> Update(GradeComponentModel item);
        Task<bool> Remove(int id);
    }
}
