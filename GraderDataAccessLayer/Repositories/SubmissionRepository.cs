namespace GraderDataAccessLayer.Repositories
{
    using System.Configuration;
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core;
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


        public async Task<FileModel> GetFile(int courseId, string entityName, string username, string filename)
        {
            // submissions\{courseId}\{entityName}\{username}\{filename}
            // var location = @"submissions\{0}\{1}\{2}\{3}";
            var file = String.Format(ConfigurationManager.AppSettings["submissionLocation"], courseId, entityName, username, filename);
            if (!File.Exists(file))
            {
                return null;
            }
            var text = await Task.Run(() => File.ReadAllText(file));
            var result = new FileModel { Filename = file, Username = username, Contents = text };
            return result;
        }

        // NOT TESTED AT ALL!!
        public async Task<IEnumerable<FileModel>> GetAllFiles(int courseId)
        {
            var result = new List<FileModel>();

            var coursePath = String.Format("submissions\\{0}\\", courseId);
            var directory = new DirectoryInfo(coursePath);
            var fileList = directory.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (var fileInfo in fileList)
            {
                var sr = fileInfo.OpenText();
                var s = await sr.ReadToEndAsync();
                result.Add(new FileModel { Filename = fileInfo.FullName, Contents = s, Username = String.Empty });
            }

            return result;
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
        
        public async Task<string> Add(FileModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
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
                await Task.Run(() => File.WriteAllText(item.Filename, item.Contents));
                return item.Filename;
            }
            catch (Exception)
            {
                return String.Empty;
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
