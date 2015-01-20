namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseRepository : GenericRepository<CourseModel>, ICourseRepository
    {
        public CourseRepository(DatabaseContext db)
            : base(db)
        { }

        public async Task<IEnumerable<CourseModel>> GetByName(string name)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.Name == name));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByShortName(string shortName)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.ShortName == shortName));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByCourseNumber(string courseNumber)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.CourseNumber == courseNumber));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetBySemester(int semester)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.Semester == semester));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByYear(int year)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.Year == year));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByStartDate(DateTime startDate)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.StartDate == startDate));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByEndDate(DateTime endDate)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.EndDate == endDate));
            return searchResult;
        }
        public async Task<IEnumerable<CourseModel>> GetByOwnerId(int ownerId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.OwnerId == ownerId));
            return searchResult;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (Context == null)
            {
                return;
            }
            Context.Dispose();
            Context = null;
        }
    }
}
