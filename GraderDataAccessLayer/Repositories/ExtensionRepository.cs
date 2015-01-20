namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExtensionRepository : GenericRepository<ExtensionModel>, IExtensionRepository
    {

        public ExtensionRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<ExtensionModel>> GetByEntityId(int entityId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<ExtensionModel>> GetByUserId(int userId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.UserId == userId));
            return searchResult;
        }

        public async Task<IEnumerable<ExtensionModel>> AddToTeam(ExtensionModel extension, IEnumerable<UserModel> teamMembers)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var returnValues = new List<ExtensionModel>();
                    foreach (var newExtension in teamMembers.Select(tm => new ExtensionModel { UserId = tm.Id, 
                        EntityId = extension.EntityId, IsGranted = extension.IsGranted, NewDeadline = extension.NewDeadline }))
                    {
                        Context.Entry(newExtension).State = EntityState.Added;
                        returnValues.Add(newExtension);
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return returnValues;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public async Task<IEnumerable<ExtensionModel>> UpdateForTeam(ExtensionModel extension, IEnumerable<UserModel> teamMembers)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var returnValues = new List<ExtensionModel>();
                    foreach (var tm in teamMembers)
                    {
                        var tm1 = tm;
                        var existingExtension = await DbSet.FirstOrDefaultAsync(f => f.EntityId == extension.EntityId && f.UserId == tm1.Id);
                        if (existingExtension == null)
                        {
                            var newExtension = new ExtensionModel { EntityId = extension.EntityId, NewDeadline = extension.NewDeadline, IsGranted = extension.IsGranted };
                            Context.Entry(newExtension).State = EntityState.Added;
                            returnValues.Add(newExtension);
                            continue;
                        }

                        existingExtension.IsGranted = extension.IsGranted;
                        existingExtension.NewDeadline = extension.NewDeadline;
                        Context.Entry(existingExtension).State = EntityState.Modified;
                        returnValues.Add(existingExtension);
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return returnValues;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }
        public async Task<bool> DeleteForTeam(ExtensionModel existingExtension, IEnumerable<UserModel> teamMembers)
        {
            if (existingExtension == null)
            {
                throw new ArgumentNullException("existingExtension");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var tm in teamMembers)
                    {
                        var tm1 = tm;
                        var oldExtension = await DbSet.FirstOrDefaultAsync(g => g.EntityId == existingExtension.EntityId && g.UserId == tm1.Id);
                        if (oldExtension != null)
                        {
                            Context.Entry(oldExtension).State = EntityState.Deleted;
                        }
                    }

                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
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
