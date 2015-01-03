namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Expressions;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;


    public class CourseRepository : ICourseRepository
    {
        private DatabaseContext _db = new DatabaseContext();


        public async Task<CourseModel> Get(int courseId)
        {
            return await _db.Course.FirstOrDefaultAsync(c => c.Id == courseId);
        }
        public async Task<IEnumerable<CourseModel>> GetAll()
        {
            return await _db.Course.ToListAsync();
        }     

        public async Task<IEnumerable<CourseModel>> GetByName(string name)
        {
            var searchResult = _db.Course.Where(c => c.Name == name);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByShortName(string shortName)
        {
            var searchResult = _db.Course.Where(c => c.ShortName == shortName);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByCourseNumber(string courseNumber)
        {
            var searchResult = _db.Course.Where(c => c.CourseNumber == courseNumber);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetBySemester(int semester)
        {
            var searchResult = _db.Course.Where(c => c.Semester == semester);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByYear(int year)
        {
            var searchResult = _db.Course.Where(c => c.Year == year);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByStartDate(DateTime startDate)
        {
            var searchResult = _db.Course.Where(c => c.StartDate == startDate);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByEndDate(DateTime endDate)
        {
            var searchResult = _db.Course.Where(c => c.EndDate == endDate);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByOwnerId(int ownerId)
        {
            var searchResult = _db.Course.Where(c => c.OwnerId == ownerId);
            return await searchResult.ToListAsync();
        }
        public async Task<IEnumerable<CourseModel>> GetByLambda(Expression<Func<CourseModel, bool>> e)
        {
            var searchResult = _db.Course.Where(e);
            return await searchResult.ToListAsync();
        }

        public async Task<CourseModel> Add(CourseModel item)
        {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Course.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Owner).Load();
                return item;
            }
            catch (DbException) {
                return null;
            }
        }
        public async Task<CourseModel> Update(CourseModel item)
        {
            var dbItem = _db.Course.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null) {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Entry(dbItem).CurrentValues.SetValues(item);
                await _db.SaveChangesAsync();
                return await Get(item.Id);
            }
            catch (DbException) {
                return null;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var item = _db.Course.FirstOrDefault(c => c.Id == id);
            if (item == null) {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.Course.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbException) {
                return false;
            }
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
            if (_db == null)
            {
                return;
            }

            _db.Dispose();
            _db = null;
        }      
    }
}
