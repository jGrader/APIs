namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;


    public class AdminRepository : IAdminRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public IEnumerable<AdminModel> GetAll()
        {
            return _db.Admin.Where(w => w.Id > 0);
        }

        public AdminModel GetByUserId(int userId)
        {
            var searchResult = _db.Admin.Where(w => w.UserId == userId);
            return searchResult.FirstOrDefault();
        }

        public AdminModel Get(int id)
        {
            var searchResult = _db.Admin.Where(w => w.Id == id);
            return searchResult.FirstOrDefault();
        }

        public async Task<AdminModel> Add(AdminModel item)
        {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Admin.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(a => a.User).Load();
                return item;
            }
            catch (DbException) {
                return null;
            }
        }

        public async Task<AdminModel> Update(AdminModel item)
        {
            var dbItem = _db.Admin.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null) {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Entry(dbItem).CurrentValues.SetValues(item);
                await _db.SaveChangesAsync();
                return Get(item.Id);
            }
            catch (DbException) {
                return null;
            }
        }

        public async Task<bool> Delete(int userId)
        {
            var item = _db.Admin.FirstOrDefault(c => c.UserId == userId);
            if (item == null) {
                throw new ArgumentNullException("userId");
            }

            try
            {
                _db.Admin.Remove(item);
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
