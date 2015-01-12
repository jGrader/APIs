namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGradeRepository : IDisposable
    {
        Task<GradeModel> Get(int id);
        Task<IEnumerable<GradeModel>> GetAll();
        Task<IEnumerable<GradeModel>> GetByUserId(int userId);
        Task<IEnumerable<GradeModel>> GetByGraderId(int graderId);
        Task<IEnumerable<GradeModel>> GetGradesByEntityId(int entityId);
        Task<IEnumerable<GradeModel>> GetGradesByLambda(Expression<Func<GradeModel, bool>> exp);

        Task<GradeModel> Add(GradeModel grade);
        Task<GradeModel> Update(GradeModel grade);
        Task<bool> Delete(int gradeId);
    }
}
