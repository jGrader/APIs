namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface ICourseRepository : IDisposable
    {
        Task<CourseModel> Get(int courseId);
        Task<IEnumerable<CourseModel>> GetAll();

        Task<IEnumerable<CourseModel>> GetByName(string name);
        Task<IEnumerable<CourseModel>> GetByShortName(string shortName);
        Task<IEnumerable<CourseModel>> GetByCourseNumber(string courseNumber);
        Task<IEnumerable<CourseModel>> GetBySemester(int semester);
        Task<IEnumerable<CourseModel>> GetByYear(int year);
        Task<IEnumerable<CourseModel>> GetByStartDate(DateTime startDate);
        Task<IEnumerable<CourseModel>> GetByEndDate(DateTime endDate);
        Task<IEnumerable<CourseModel>> GetByOwnerId(int ownerId);
        Task<IEnumerable<CourseModel>> GetByLambda(Expression<Func<CourseModel, bool>> e);

        Task<CourseModel> Add(CourseModel item);
        Task<CourseModel> Update(CourseModel item);
        Task<bool> Delete(int id);
    }
}
