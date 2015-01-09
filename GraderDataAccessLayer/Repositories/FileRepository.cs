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

    public class FileRepository : IFileRepository
    {
        private DatabaseContext _db = new DatabaseContext();


        public async Task<FileModel> Get(int id)
        {
            var searchResult = await _db.File.FirstOrDefaultAsync(f => f.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<FileModel>> GetAll()
        {
            return await Task.Run(() => _db.File);
        }
        public async Task<IEnumerable<FileModel>> GetAllByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => _db.File.Where(f => f.Entity.Task.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<FileModel>> GetAllByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => _db.File.Where(f => f.EntityId == entityId));
            return searchResult;
        }  

        public async Task<FileModel> Add(FileModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.File.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Entity).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<FileModel> Update(FileModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.File.FirstOrDefaultAsync(c => c.Id == item.Id);
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
            var item = await _db.File.FirstOrDefaultAsync(c => c.Id == id);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.File.Remove(item);
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
