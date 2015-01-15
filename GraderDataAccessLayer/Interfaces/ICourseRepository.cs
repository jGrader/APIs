namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ICourseRepository : IGenericRepository<CourseModel>,IDisposable
    {
        Task<IEnumerable<CourseModel>> GetByName(string name);
        Task<IEnumerable<CourseModel>> GetByShortName(string shortName);
        Task<IEnumerable<CourseModel>> GetByCourseNumber(string courseNumber);
        Task<IEnumerable<CourseModel>> GetBySemester(int semester);
        Task<IEnumerable<CourseModel>> GetByYear(int year);
        Task<IEnumerable<CourseModel>> GetByStartDate(DateTime startDate);
        Task<IEnumerable<CourseModel>> GetByEndDate(DateTime endDate);
        Task<IEnumerable<CourseModel>> GetByOwnerId(int ownerId);

    }
}
