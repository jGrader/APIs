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

    public class GradeRepository : IGradeRepository
    {
        private DatabaseContext _db;

        public GradeRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<GradeModel> Get(int id)
        {
            return await _db.Grade.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<GradeModel>> GetAll()
        {
            return await Task.Run(() => _db.Grade);
        }

        public async Task<IEnumerable<GradeModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => _db.Grade.Where(g => g.UserId == userId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> GetByGraderId(int graderId)
        {
            var searchResult = await Task.Run(() => _db.Grade.Where(g => g.GraderId == graderId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> GetGradesByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => _db.Grade.Where(g => g.EntityId == entityId));
            return searchResult;
        }

        public async Task<IEnumerable<GradeModel>> GetGradesByLambda(Expression<Func<GradeModel, bool>> exp)
        {
            var searchResult = await Task.Run(() => _db.Grade.Where(exp));
            return searchResult;
        }

        public async Task<GradeModel> Add(GradeModel grade)
        {
            if (grade == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                _db.Grade.Add(grade);
                await _db.SaveChangesAsync();

                _db.Entry(grade).Reference(c => c.User).Load();
                _db.Entry(grade).Reference(c => c.Grader).Load();
                _db.Entry(grade).Reference(c => c.Entity).Load();

                return grade;
            }
            catch (DbException)
            {
                return null;
            }
        }

        public async Task<GradeModel> Update(GradeModel grade)
        {
            if (grade == null)
            {
                throw new ArgumentNullException("grade");
            }

            var dbItem = await _db.Course.FirstOrDefaultAsync(c => c.Id == grade.Id);
            if (dbItem == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Entry(dbItem).CurrentValues.SetValues(grade);
                await _db.SaveChangesAsync();
                return await Get(grade.Id);
            }
            catch (DbException)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int gradeId)
        {
            var item = await _db.Course.FirstOrDefaultAsync(c => c.Id == gradeId);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Course.Remove(item);
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
