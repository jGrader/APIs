namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ICourseUserRepository : IGenericRepository<CourseUserModel>, IDisposable
    {
        Task<IEnumerable<CourseUserModel>> GetByCourseId(int courseId);
        Task<IEnumerable<CourseUserModel>> GetByUserId(int userId);
        Task<IEnumerable<CourseUserModel>> GetByPermissions(int permissions);
    } 
}
