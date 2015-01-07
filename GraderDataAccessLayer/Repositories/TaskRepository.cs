using System.Data.Entity.Core;

namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;


    public class TaskRepository : ITaskRepository
    {
        private DatabaseContext _db = new DatabaseContext();
    

        public async Task<TaskModel> Get(int id)
        {
            var searchResult = await _db.Task.FirstOrDefaultAsync(t => t.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<TaskModel>> GetAll()
        {
            return await Task.Run(() => _db.Task);
        }
        public async Task<IEnumerable<TaskModel>> GetAllByName(string name)
        {
            var searchResult = await Task.Run(() => _db.Task.Where(w => w.Name == name));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetAllByCourse(int courseId)
        {
            var searchResult = await Task.Run(() => _db.Task.Where(w => w.CourseId == courseId));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetAllByGradeComponent(int gradeComponentId)
        {
            var searchResult = await Task.Run(() => _db.Task.Where(w => w.GradeComponentId == gradeComponentId));
            return searchResult;
        }
        public async Task<IEnumerable<TaskModel>> GetAllByCourseAndGradeComponent(int courseId, int gradeComponentId)
        {
            var searchResult = await Task.Run(() => _db.Task.Where(w => w.CourseId == courseId && w.GradeComponentId == gradeComponentId));
            return searchResult;
        }

        public async Task<TaskModel> Add(TaskModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Task.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.Course).Load();
                _db.Entry(item).Reference(c => c.GradeComponent).Load();
                _db.Entry(item).Reference(c => c.Entities).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<TaskModel> Update(TaskModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.Task.FirstOrDefaultAsync(c => c.Id == item.Id);
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
            var item = await _db.Task.FirstOrDefaultAsync(c => c.Id == id);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Task.Remove(item);
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
