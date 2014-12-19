
using System.Linq.Expressions;

namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface ICourseRepository : IDisposable
    {
        IEnumerable<CourseModel> GetAll();
        CourseModel Get(int id);

        IEnumerable<CourseModel> GetByName(string name);

        IEnumerable<CourseModel> GetByShortName(string shortName);

        IEnumerable<CourseModel> GetByCourseNumber(string courseNumber);

        IEnumerable<CourseModel> GetBySemester(int semester);

        IEnumerable<CourseModel> GetByYear(int year);

        IEnumerable<CourseModel> GetByStartDate(DateTime startDate);

        IEnumerable<CourseModel> GetByEndDate(DateTime endDate);

        IEnumerable<CourseModel> GetByOwnerId(int ownerId);

        IEnumerable<CourseModel> GetByLambda(Expression<Func<CourseModel, bool>> e);

        Task<bool> Add(CourseModel item);

        Task<bool> Remove(int id);

        Task<bool> Update(CourseModel item);
    }
}
