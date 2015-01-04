namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public class CourseUserRepository : ICourseUserRepository
    {
        private DatabaseContext _db = new DatabaseContext();


        public async Task<CourseUserModel> Get(int id)
        {
            var searchResult = await _db.CourseUser.FirstOrDefaultAsync(cu => cu.Id == id);
            return searchResult;
        }
       
        public async Task<IEnumerable<CourseUserModel>> GetAll()
        {
            return await Task.Run(() => _db.CourseUser);
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(w => w.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByUser(int userId)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(w => w.UserId == userId));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByExtensionLimit(int extLimit)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(w => w.ExtensionLimit == extLimit));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByExcuseLimit(int excLimit)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(w => w.ExcuseLimit == excLimit));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByPermissions(int permissions)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(w => w.Permissions == permissions));
            return searchResult;
        }
        public async Task<IEnumerable<CourseUserModel>> GetAllByLambda(Expression<Func<CourseUserModel, bool>> exp)
        {
            var searchResult = await Task.Run(() => _db.CourseUser.Where(exp));
            return searchResult;
        }

        public async Task<CourseUserModel> Add(CourseUserModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.CourseUser.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.User).Load();
                _db.Entry(item).Reference(c => c.Course).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<CourseUserModel> Update(CourseUserModel item)
        {
            var dbItem = await _db.CourseUser.FirstOrDefaultAsync(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Entry(dbItem).CurrentValues.SetValues(item);
                await _db.SaveChangesAsync();
                return await Get(item.Id);
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var item = await _db.CourseUser.FirstOrDefaultAsync(c => c.Id == id);
            if (item == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.CourseUser.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbException)
            {
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
