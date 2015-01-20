namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExcuseRepository : GenericRepository<ExcuseModel>, IExcuseRepository
    {

        public ExcuseRepository(DatabaseContext db) : base(db)
        { }

        public async Task<IEnumerable<ExcuseModel>> GetByEntity(int entityId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.EntityId == entityId));
            return searchResult;
        }
        public async Task<IEnumerable<ExcuseModel>> GetByUser(int userId)
        {
            var searchResult = await Task.Run(() => DbSet.Where(c => c.UserId == userId));
            return searchResult;
        }

        public async Task<IEnumerable<ExcuseModel>> AddToTeam(ExcuseModel excuse, IEnumerable<UserModel> teamMembers)
        {
            if (excuse == null)
            {
                throw new ArgumentNullException("excuse");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var returnValues = new List<ExcuseModel>();
                    foreach (var newExcuse in teamMembers.Select(tm => new ExcuseModel { UserId = tm.Id, EntityId = excuse.EntityId, IsGranted = excuse.IsGranted }))
                    {
                        Context.Entry(newExcuse).State = EntityState.Added;
                        returnValues.Add(newExcuse);
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
        public async Task<IEnumerable<ExcuseModel>> UpdateForTeam(ExcuseModel excuse, IEnumerable<UserModel> teamMembers)
        {
            if (excuse == null)
            {
                throw new ArgumentNullException("excuse");
            }
            if (teamMembers == null)
            {
                throw new ArgumentNullException("teamMembers");
            }

            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var returnValues = new List<ExcuseModel>();
                    foreach (var tm in teamMembers)
                    {
                        var tm1 = tm;
                        var existingExcuse = await DbSet.FirstOrDefaultAsync(f => f.EntityId == excuse.EntityId && f.UserId == tm1.Id);
                        if (existingExcuse == null)
                        {
                            var newExcuse = new ExcuseModel { EntityId = excuse.EntityId, IsGranted = excuse.IsGranted };
                            Context.Entry(newExcuse).State = EntityState.Added;
                            returnValues.Add(newExcuse);
                            continue;
                        }

                        existingExcuse.IsGranted = excuse.IsGranted;
                        Context.Entry(existingExcuse).State = EntityState.Modified;
                        returnValues.Add(excuse);
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
        public async Task<bool> DeleteForTeam(ExcuseModel existingExcuse, IEnumerable<UserModel> teamMembers)
        {
            if (existingExcuse == null)
            {
                throw new ArgumentNullException("existingExcuse");
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
                        var oldExcuse = await DbSet.FirstOrDefaultAsync(g => g.EntityId == existingExcuse.EntityId && g.UserId == tm1.Id);
                        if (oldExcuse != null)
                        {
                            Context.Entry(oldExcuse).State = EntityState.Deleted;
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
