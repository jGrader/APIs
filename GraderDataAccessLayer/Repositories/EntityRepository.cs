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


    public class EntityRepository : IEntityRepository
    {
        private DatabaseContext _db;

        public EntityRepository()
        {
            _db = new DatabaseContext();
        }
        public async Task<EntityModel> Get(int id)
        {
            var searchResult = await _db.Entity.FirstOrDefaultAsync(e => e.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<EntityModel>> GetAll()
        {
            return await Task.Run(() => _db.Entity);
        }
        public async Task<IEnumerable<EntityModel>> GetAllForTask(int taskId)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.TaskId == taskId));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetAllByName(string name)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.Name == name));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetAllByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.Task.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetAllByOpenDate(DateTime openTime)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.OpenTime == openTime));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetAllByCloseDate(DateTime closeTime)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.CloseTime == closeTime));
            return searchResult;
        }
        public async Task<IEnumerable<EntityModel>> GetAllActiveBetweenDates(DateTime time1, DateTime time2)
        {
            var searchResult = await Task.Run(() => _db.Entity.Where(e => e.OpenTime > time1 && e.CloseTime < time2));
            return searchResult;
        }

        public async Task<EntityModel> Add(EntityModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Entity.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Task).Load();
                _db.Entry(item).Collection(c => c.Files).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<EntityModel> Update(EntityModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.Entity.FirstOrDefaultAsync(c => c.Id == item.Id);
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
            var item = await _db.Entity.FirstOrDefaultAsync(c => c.Id == id);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Entity.Remove(item);
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
