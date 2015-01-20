namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Threading.Tasks;


    public class SessionIdRepository : GenericRepository<SessionIdModel>, ISessionIdRepository
    {

        public SessionIdRepository(DatabaseContext db) : base(db)
        { }

        public async Task<SessionIdModel> GetByUserId(int userId)
        {
            var searchResult = await DbSet.FirstOrDefaultAsync(s => s.UserId == userId);
            return searchResult;
        }
        public async Task<SessionIdModel> GetBySesionId(Guid sessionId)
        {
            var searchResult = await DbSet.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            return searchResult;
        }

        public async Task<SessionIdModel> Add(int userId)
        {
            try
            {
                var newSession = new SessionIdModel(userId);
                DbSet.Add(newSession);
                await Context.SaveChangesAsync();

                //Load virtual properties and return object
                Context.Entry(newSession).Reference(c => c.User).Load();
                return newSession;
            }
            catch (DbException)
            {
                return null;
            }
        }

        public async Task<bool> IsAuthorized(SessionIdModel sessionIdModel)
        {
            if (sessionIdModel == null)
            {
                throw new ArgumentNullException("sessionIdModel");
            }


            if (sessionIdModel.ExpirationTime < DateTime.UtcNow)
            {
                return false;
            }

            try
            {
                sessionIdModel.ExpirationTime = DateTime.UtcNow.AddMinutes(15);
                await Update(sessionIdModel);
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int userId)
        {
            try
            {
                var model = await DbSet.FirstOrDefaultAsync(s => s.UserId == userId);
                return await Delete(model);
            }
            catch (Exception e)
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

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
        }      
    }
}
