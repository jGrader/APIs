namespace GraderDataAccessLayer.Repositories
{
    using Models;
    using Interfaces;

    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Threading.Tasks;


    public class SessionIdRepository : ISessionIdRepository
    {
        private DatabaseContext _db;

        public SessionIdRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SessionIdModel>> GetAll()
        {
            return await Task.Run(() => _db.SessionId);
        }

        public async Task<SessionIdModel> Get(int userId)
        {
            var searchResult = await _db.SessionId.FirstOrDefaultAsync(s => s.UserId == userId);
            return searchResult;
        }
        public async Task<SessionIdModel> GetBySesionId(Guid sessionId)
        {
            var searchResult = await _db.SessionId.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            return searchResult;
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
                _db.Entry(sessionIdModel).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }

        public async Task<SessionIdModel> Add(int userId)
        {
            var dbUser = await _db.User.FirstOrDefaultAsync(u => u.Id == userId);
            if (dbUser == null)
            {
                throw new ArgumentNullException("userId");
            }

            try
            {
                var newSession = new SessionIdModel(userId);
                _db.SessionId.Add(newSession);
                await _db.SaveChangesAsync();

                //Load virtual properties and return object
                _db.Entry(newSession).Reference(c => c.User).Load();
                return newSession;
            }
            catch (DbException)
            {
                return null;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var dbSession = await _db.SessionId.FirstOrDefaultAsync(u => u.Id == id);
            if (dbSession == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                _db.SessionId.Remove(dbSession);
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
