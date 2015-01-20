namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class TeamRepository : GenericRepository<TeamModel>, ITeamRepository
    {

        public TeamRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<TeamModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<TeamModel>> GetByCoureId(int courseId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(e => e.Entity.Task.CourseId == courseId));
            return searchResult;
        }

        public override async Task<TeamModel> Add(TeamModel team)
        {
            if (team == null)
            {
                throw new ArgumentNullException("team");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var tm in team.TeamMembers)
                    {
                        // In case one of the users in the team already has 
                        // a submission, excuse, or extension for this entity, delete ALL of them
                        var tm1 = tm;
                        var submissions = Context.Submission.Where(e => e.File.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var s in submissions)
                        {
                            Context.Entry(s).State = EntityState.Deleted;
                        }

                        var extensions = Context.Extension.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in extensions)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }

                        var excuses = Context.Excuse.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in excuses)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }
                    }

                    Context.Entry(team).State = EntityState.Added;
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return team;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public override async Task<TeamModel> Update(TeamModel team)
        {
            if (team == null)
            {
                throw new ArgumentNullException("team");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var oldTeam = await Context.Team.FirstOrDefaultAsync(f => f.Id == team.Id);
                    var deletedMembers = oldTeam.TeamMembers.Where(tm => !team.TeamMembers.Contains(tm)).ToList();
                    var addedMembers = team.TeamMembers.Where(tm => !oldTeam.TeamMembers.Contains(tm)).ToList();

                    var filesToDelete = new List<string>();
                    foreach (var tm in deletedMembers)
                    {
                        // Delete all submission, extensions, excuses of this team for the deleted team members
                        var tm1 = tm;
                        var submissions = Context.Submission.Where(e => e.File.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var s in submissions)
                        {
                            filesToDelete.Add(s.FilePath);
                            Context.Entry(s).State = EntityState.Deleted;
                        }

                        var extensions = Context.Extension.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in extensions)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }

                        var excuses = Context.Extension.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in excuses)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }
                    }

                    var constantMember = oldTeam.TeamMembers.First(tm => !deletedMembers.Contains(tm));
                    var teamSubmissions = Context.Submission.Where(e => e.File.EntityId == team.EntityId && e.UserId == constantMember.Id);
                    var teamExtensions = Context.Extension.Where(e => e.EntityId == team.EntityId && e.UserId == constantMember.Id);
                    var teamExcuses = Context.Excuse.Where(e => e.EntityId == team.EntityId && e.UserId == constantMember.Id);
                    var filesToAdd = new List<string>();
                    foreach (var tm in addedMembers)
                    {
                        // Add all submissions, extensions, excuses of the team to the new team members
                        var tm1 = tm;
                        foreach (var s in teamSubmissions)
                        {
                            filesToAdd.Add(s.FilePath);
                            var newSubmission = new SubmissionModel { FileId = s.FileId, FilePath = s.FilePath, TimeStamp = s.TimeStamp, UserId = tm1.Id };
                            Context.Entry(newSubmission).State = EntityState.Added;
                        }

                        foreach (var newExtensions in teamExtensions.Select(e => new ExtensionModel { EntityId = e.EntityId, IsGranted = e.IsGranted, NewDeadline = e.NewDeadline, UserId = tm1.Id }))
                        {
                            Context.Entry(newExtensions).State = EntityState.Added;
                        }

                        foreach (var newExcuse in teamExcuses.Select(e => new ExcuseModel { EntityId = e.EntityId, IsGranted = e.IsGranted, UserId = tm1.Id }))
                        {
                            Context.Entry(newExcuse).State = EntityState.Added;
                        }
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    // Now manipulate the files for the submissions
                    foreach (var path in filesToDelete.Where(File.Exists))
                    {
                        // Delete the files of the removed users
                        File.Delete(path);
                    }
                    foreach (var tm in addedMembers)
                    {
                        // Add the files to the users added to the team
                        foreach (var path in filesToAdd.Where(File.Exists))
                        {
                            var newPath = path.Replace(constantMember.UserName, tm.UserName);
                            File.Copy(path, newPath, true);
                        }
                    }

                    return team;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public override async Task<bool> Delete(TeamModel team)
        {
            if (team == null)
            {
                throw new ArgumentNullException("team");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var filesToDelete = new List<string>();
                    foreach (var tm in team.TeamMembers)
                    {
                        // Delete all submissions, extensions, excuses this team had
                        var tm1 = tm;
                        var submissions = Context.Submission.Where(s => s.File.EntityId == team.EntityId && s.UserId == tm1.Id);
                        foreach (var s in submissions)
                        {
                            filesToDelete.Add(s.FilePath);
                            Context.Entry(s).State = EntityState.Deleted;
                        }

                        var extensions = Context.Extension.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in extensions)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }

                        var excuses = Context.Excuse.Where(e => e.EntityId == team.EntityId && e.UserId == tm1.Id);
                        foreach (var e in excuses)
                        {
                            Context.Entry(e).State = EntityState.Deleted;
                        }
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    // Now delete the files
                    foreach (var path in filesToDelete.Where(File.Exists))
                    {
                        File.Delete(path);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return false;
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
