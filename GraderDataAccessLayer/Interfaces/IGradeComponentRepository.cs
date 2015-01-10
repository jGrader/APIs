namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;


    public interface IGradeComponentRepository : IDisposable
    {
        Task<GradeComponentModel> Get(int id);
        Task<IEnumerable<GradeComponentModel>> GetAll();
        Task<IEnumerable<GradeComponentModel>> GetAllByCourse(int courseId);
        
        Task<GradeComponentModel> Add(GradeComponentModel item);
        Task<GradeComponentModel> Update(GradeComponentModel item);
        Task<bool> Delete(int id);
    }
}
