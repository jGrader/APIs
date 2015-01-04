namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface ICourseUserRepository : IDisposable
    {
        Task<CourseUserModel> Get(int id);

        Task<IEnumerable<CourseUserModel>> GetAll();
        Task<IEnumerable<CourseUserModel>> GetAllByCourseId(int courseId);
        Task<IEnumerable<CourseUserModel>> GetAllByUser(int userId);
        Task<IEnumerable<CourseUserModel>> GetAllByExtensionLimit(int extLimit);
        Task<IEnumerable<CourseUserModel>> GetAllByExcuseLimit(int excLimit);
        Task<IEnumerable<CourseUserModel>> GetAllByPermissions(int permissions);
        Task<IEnumerable<CourseUserModel>> GetAllByLambda(Expression<Func<CourseUserModel, bool>> exp);

        Task<CourseUserModel> Add(CourseUserModel item);
        Task<CourseUserModel> Update(CourseUserModel item);
        Task<bool> Delete(int id);
    } 
}
