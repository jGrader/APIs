﻿namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;


    public interface ICourseRepository : IDisposable
    {
        Task<IEnumerable<CourseModel>> GetAll();
        Task<CourseModel> Get(int courseId);

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
