namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Interfaces;
    using Models;

    public class SubmissionRepository: ISubmissionRepository
    {
        private readonly DatabaseContext _db = new DatabaseContext();

        public IEnumerable<SubmissionModel> GetAll()
        {
            var result = _db.Submission.Where(s => s.Id > 0);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public IEnumerable<SubmissionModel> GetByEntityId(int id)
        {
            var result = _db.Submission.Where(s => s.EntityId == id);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public IEnumerable<SubmissionModel> GetByUserId(int id)
        {
            var result = _db.Submission.Where(s => s.UserId == id);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public IEnumerable<SubmissionModel> GetBefore(DateTime timestamp)
        {
            var result = _db.Submission.Where(s => s.TimeStamp <= timestamp);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public IEnumerable<SubmissionModel> GetAfter(DateTime timestamp)
        {
            var result = _db.Submission.Where(s => s.TimeStamp > timestamp);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public IEnumerable<SubmissionModel> GetByUserIdAndEntityId(int userId, int entityId)
        {
            var result = _db.Submission.Where(s => s.UserId == userId && s.EntityId == entityId);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public IEnumerable<FileModel> GetAllFiles()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FileModel> GetFileByEntityId(int id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FileModel> GetFilesBefore(DateTime timestamp)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FileModel> GetFilesAfter(DateTime timestamp)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FileModel> GetFilesByUserIdAndEntityId(int userId, int entityId)
        {
            throw new NotImplementedException();
        }

        public SubmissionModel Get(int id)
        {
            var result = _db.Submission.FirstOrDefault(s => s.Id > 0);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }
        public bool Add(SubmissionModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                _db.Submission.Add(item);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddFile(FileModel file)
        {
            if (file == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                if (File.Exists(file.Filename))
                {
                    var extension = Path.GetExtension(file.Filename);
                    var name = Path.ChangeExtension(file.Filename, null);
                    var i = 1;
                    while (File.Exists(name + i.ToString(CultureInfo.InvariantCulture) + extension))
                    {
                        i++;
                    }
                    file.Filename = name + i.ToString(CultureInfo.InvariantCulture) + extension;
                }
                // File.Create(file.Filename);
                File.WriteAllText(file.Filename, file.Contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool RemoveFile(FileModel file)
        {
            throw new NotImplementedException();
        }

        public bool Update(SubmissionModel item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateFile(FileModel file)
        {
            throw new NotImplementedException();
        }
    }
}
