namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;


    public class SubmissionRepository: ISubmissionRepository
    {
        private DatabaseContext _db = new DatabaseContext();


        public async Task<SubmissionModel> Get(int id)
        {
            var searchResult = await _db.Submission.FirstOrDefaultAsync(s => s.Id == id);
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> GetAll()
        {
            return await Task.Run(() => _db.Submission);
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllByEntityId(int id)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.EntityId == id));
            return searchResult;
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllByUserId(int id)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.UserId == id));
            return searchResult;
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllBefore(DateTime timestamp)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.TimeStamp < timestamp));
            return searchResult;
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllAfter(DateTime timestamp)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.TimeStamp > timestamp));
            return searchResult;
        }
        public async Task<IEnumerable<SubmissionModel>> GetAllByUserIdAndEntityId(int userId, int entityId)
        {
            var searchResult = await Task.Run(() => _db.Submission.Where(s => s.UserId == userId && s.EntityId == entityId));
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
                _db.Entry(item).Reference(c => c.Entity).Load();
                return item;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<SubmissionModel> Update(SubmissionModel item)
        {
            var dbItem = await _db.Submission.FirstOrDefaultAsync(c => c.Id == item.Id);
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
        public async Task<bool> DeleteSubmission(int sumbmissionId)
        {
            var item = await _db.Submission.FirstOrDefaultAsync(c => c.Id == sumbmissionId);
            if (item == null)
            {
                throw new ArgumentNullException("sumbmissionId");
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



        public async Task<FileModel> GetFile(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FileModel>> GetAllFiles()
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<FileModel>> GetAllFilesByEntityId(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<FileModel>> GetAllFilesBefore(DateTime timestamp)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<FileModel>> GetAllFilesAfter(DateTime timestamp)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<FileModel>> GetAllFilesByUserIdAndEntityId(int userId, int entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Add(FileModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                if (File.Exists(item.Filename))
                {
                    var extension = Path.GetExtension(item.Filename);
                    var name = Path.ChangeExtension(item.Filename, null);
                    var i = 1;
                    while (File.Exists(name + i.ToString(CultureInfo.InvariantCulture) + extension))
                    {
                        i++;
                    }
                    item.Filename = name + i.ToString(CultureInfo.InvariantCulture) + extension;
                }
                // File.Create(file.Filename);
                File.WriteAllText(item.Filename, item.Contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> Update(FileModel item)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteFile(int fileId)
        {
            throw new NotImplementedException();
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
