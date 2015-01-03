namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;


    public class GradeComponentRepository : IGradeComponentRepository
    {
        private DatabaseContext _db = new DatabaseContext();


        public async Task<GradeComponentModel> Get(int id)
        {
            return await _db.GradeComponent.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<IEnumerable<GradeComponentModel>> GetAll()
        {
            return _db.GradeComponent;
        }
        public async Task<IEnumerable<GradeComponentModel>> GetAllByCourse(int courseId)
        {
            var searchResult = _db.GradeComponent.Where(w => w.CourseId == courseId);
            return await searchResult.ToListAsync();
        }

        public async Task<GradeComponentModel> Add(GradeComponentModel item)
        {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.GradeComponent.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(r => r.Course).Load();
                return item;
            }
            catch (DbException) {
                return null;
            }
        }
        public async Task<GradeComponentModel> Update(GradeComponentModel item)
        {
            var dbItem = _db.GradeComponent.FirstOrDefault(c => c.Id == item.Id);
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
            var item = _db.GradeComponent.FirstOrDefault(c => c.Id == id);
            if (item == null) {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.GradeComponent.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbException) {
                return false;
            }
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
