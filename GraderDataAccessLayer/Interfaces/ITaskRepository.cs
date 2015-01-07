namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ITaskRepository : IDisposable
    {
        Task<TaskModel> Get(int id);
        
        Task<IEnumerable<TaskModel>> GetAll();
        Task<IEnumerable<TaskModel>> GetAllByName(string name);
        Task<IEnumerable<TaskModel>> GetAllByCourse(int courseId);
        Task<IEnumerable<TaskModel>> GetAllByGradeComponent(int gradeComponentId);
        Task<IEnumerable<TaskModel>> GetAllByCourseAndGradeComponent(int courseId, int gradeComponentId);

        Task<TaskModel> Add(TaskModel item);
        Task<TaskModel> Update(TaskModel item);
        Task<bool> Delete(int id);
    }
}
