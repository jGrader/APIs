using System.Data.Entity;
using System.IO;

namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SubmissionRepository : GenericRepository<SubmissionModel>, ISubmissionRepository
    {
        public SubmissionRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<SubmissionModel>> GetByCourseId(int courseId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(s => s.File.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(s => s.File.Entity.Task.CourseId == userId));
            return searchResult;
        }

        public async Task<IEnumerable<SubmissionModel>> AddSubmissionToTeam(string fileSavePath, string tempFilePath, FileModel file, IEnumerable<UserModel> teamMembers)
        {
            if (String.IsNullOrEmpty(fileSavePath))
            {
                throw new ArgumentNullException("fileSavePath");
            }
            if (String.IsNullOrEmpty(tempFilePath))
            {
                throw new ArgumentNullException("tempFilePath");
            }
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                var backUpFiles = new List<string>();

                try
                {
                    var addedSubmission = new List<SubmissionModel>();
                    foreach (var teamMember in teamMembers)
                    {
                        var member = teamMember;

                        var finalPath = Path.Combine(fileSavePath, member.UserName);
                        Directory.CreateDirectory(finalPath);

                        // Copy the file to the correct location with the correct name
                        var finalFilePath = finalPath + "\\" + (file.FileName + file.Extension);
                        if (File.Exists(finalFilePath))
                        {
                            // Make backup first; just in case
                            var backUpPath = finalFilePath + ".bak";
                            File.Copy(finalFilePath, backUpPath);
                            backUpFiles.Add(backUpPath);
                        }
                        File.Copy(tempFilePath, finalFilePath, true);


                        // Now it's time to register a submission for this file in the Database
                        var submission = (Context.Submission.Where(s => s.FileId == file.Id && s.UserId == member.Id)).FirstOrDefault();
                        if (submission != null)
                        {
                            // This is not the first submission; update the old one
                            submission.FileId = file.Id;
                            submission.FilePath = finalFilePath;
                            submission.TimeStamp = DateTime.UtcNow;
                            submission.UserId = member.Id;

                            Context.Entry(submission).State = EntityState.Modified;
                        }
                        else
                        {
                            // This is the very first submission
                            submission = new SubmissionModel { FileId = file.Id, FilePath = finalFilePath, TimeStamp = DateTime.UtcNow, UserId = member.Id };
                            Context.Entry(submission).State = EntityState.Added;
                        }

                        addedSubmission.Add(submission);
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    foreach (var backUpPath in backUpFiles)
                    {
                        File.Delete(backUpPath);
                    }

                    return addedSubmission;
                }
                catch (Exception e)
                {
                    Logger.Log(e);

                    dbContextTransaction.Rollback();
                    foreach (var backUpPath in backUpFiles)
                    {
                        File.Copy(backUpPath, backUpPath.Replace(".bak", String.Empty), true);
                        File.Delete(backUpPath);
                    }

                    return null;
                }
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

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
        }
    }
}
