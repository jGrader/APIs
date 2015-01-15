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

    public class SubmissionRepository: ISubmissionRepository
    {
        private DatabaseContext _db;

        public SubmissionRepository()
        {
            _db = new DatabaseContext();
        }

        public async Task<SubmissionModel> Get(int id)
        {
            var searchResult = await _db.Submission.FirstOrDefaultAsync(s => s.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> GetAll()
        {
            return await Task.Run(() => _db.Submission);
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.File.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> GetAllByUserId(int userId)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.File.Entity.Task.CourseId == userId));
            return searchResult;
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllByLambda(Expression<Func<SubmissionModel, bool>> exp)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(exp));
            return searchResult;
        }

        public async Task<SubmissionModel> Add(SubmissionModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Submission.Add(item);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(item).Reference(c => c.User).Load();
                _db.Entry(item).Reference(c => c.File).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<SubmissionModel> Update(SubmissionModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var dbItem = await _db.Submission.FirstOrDefaultAsync(c => c.Id == item.Id);
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
        public async Task<bool> DeleteSubmission(int sumbmissionId)
        {
            var item = await _db.Submission.FirstOrDefaultAsync(c => c.Id == sumbmissionId);
            if (item == null)
            {
                throw new ObjectNotFoundException();
            }

            try
            {
                _db.Submission.Remove(item);
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
