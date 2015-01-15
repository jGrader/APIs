namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ITaskRepository : IGenericRepository<TaskModel>, IDisposable
    {
        Task<IEnumerable<TaskModel>> GetByName(string name);
        Task<IEnumerable<TaskModel>> GetByCourseId(int courseId);
        Task<IEnumerable<TaskModel>> GetByGradeComponentId(int gradeComponentId);
        Task<IEnumerable<TaskModel>> GetByCourseIdAndGradeComponentId(int courseId, int gradeComponentId);
    }
}
