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
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class ExtensionRepository : IExtensionRepository
    {
        private DatabaseContext _db;
        public ExtensionRepository(DatabaseContext db)
        {
            _db = db;
        }


        public async Task<ExtensionModel> Get(int extesionId)
        {
            return await _db.Extension.FirstOrDefaultAsync(c => c.Id == extesionId);
        }

        public async Task<IEnumerable<ExtensionModel>> GetAll()
        {
            return await Task.Run(() => _db.Extension);
        }
        public async Task<IEnumerable<ExtensionModel>> GetAllForEntity(int entityId)
        {
            var searchResult = await Task.Run(() => _db.Extension.Where(c => c.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<ExtensionModel>> GetAllForUser(int userId)
        {
            var searchResult = await Task.Run(() => _db.Extension.Where(c => c.UserId == userId));
            return searchResult;
        }
        public async Task<IEnumerable<ExtensionModel>> GetAllByLambda(Expression<Func<ExtensionModel, bool>> e)
        {
            var searchResult = await Task.Run(() => _db.Extension.Where(e));
            return searchResult;
        }

        public async Task<ExtensionModel> Add(ExtensionModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Extension.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Entity).Load();
                _db.Entry(item).Reference(c => c.User).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<ExtensionModel> Update(ExtensionModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.Extension.FirstOrDefaultAsync(c => c.Id == item.Id);
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
        public async Task<bool> Delete(int id)
        {
            var item = await _db.Extension.FirstOrDefaultAsync(c => c.Id == id);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Extension.Remove(item);
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
