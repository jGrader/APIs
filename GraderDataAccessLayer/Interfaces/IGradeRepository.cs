namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGradeRepository : IGenericRepository<GradeModel>, IDisposable
    {
        Task<IEnumerable<GradeModel>> GetByUserId(int userId);
        Task<IEnumerable<GradeModel>> GetByGraderId(int graderId);
        Task<IEnumerable<GradeModel>> GetByEntityId(int entityId);
    }
}
