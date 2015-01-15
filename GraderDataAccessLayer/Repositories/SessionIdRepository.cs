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
            var searchResult = await dbSet.FirstOrDefaultAsync(s => s.UserId == userId);
            return searchResult;
        }
        public async Task<SessionIdModel> GetBySesionId(Guid sessionId)
        {
            var searchResult = await dbSet.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            return searchResult;
        }

        public async Task<SessionIdModel> Add(int userId)
        {
            var dbUser = await dbSet.FirstOrDefaultAsync(u => u.Id == userId);
            if (dbUser == null)
            {
                throw new ArgumentNullException("userId");
            }

            try
            {
                var newSession = new SessionIdModel(userId);
                dbSet.Add(newSession);
                await context.SaveChangesAsync();

                //Load virtual properties and return object
                context.Entry(newSession).Reference(c => c.User).Load();
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

            if (context == null)
            {
                return;
            }

            context.Dispose();
            context = null;
        }      
    }
}
