namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Linq;
    using System.Threading.Tasks;


    public class UserRepository : IUserRepository
    {
        private DatabaseContext _db;

        public UserRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<UserModel> Get(int id)
        {
            var searchResult = await _db.User.FirstOrDefaultAsync(u => u.Id == id);
            return searchResult;
        }
        public async Task<UserModel> GetByUsername(string username)
        {
            var searchResult = await _db.User.FirstOrDefaultAsync(u => u.UserName == username);
            return searchResult;
        }

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            return await Task.Run(() => _db.User);
        }
        public async Task<IEnumerable<UserModel>> GetAllByGraduationYear(string year)
        {
            return await Task.Run(() => _db.User.Where(w => w.GraduationYear == year));
        }

        public async Task<string> GetEmail(int id)
        {
            var searchResult = await _db.User.FirstOrDefaultAsync(w => w.Id == id);
            return searchResult == null ? string.Empty : searchResult.Email;
        }

        public async Task<UserModel> Add(UserModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.User.Add(item);
                await _db.SaveChangesAsync();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<UserModel> Update(UserModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = _db.User.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ObjectNotFoundException();
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
        public async Task<bool> Delete(int userId)
        {
            var item = await _db.User.FirstOrDefaultAsync(c => c.Id == userId);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.User.Remove(item);
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

            _db = null;
        }      
    }
}
