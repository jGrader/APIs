using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Core;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Interfaces;


namespace GraderDataAccessLayer.Repositories
{
    public class SessionIdRepository : ISessionIdRepository
    {
        DatabaseContext _db = new DatabaseContext();

        public SessionIdModel GetBySesionId(Guid sessionId)
        {
            var result = _db.SessionId.FirstOrDefault(s => s.SessionId == sessionId);

            if (result == null)
            {
                throw new ObjectNotFoundException();
            }

            return result;
        }

        public Guid Add(int userId)
        {
            try
            {
                var sessionModel = new SessionIdModel(userId);

                _db.SessionId.Add(sessionModel);
                _db.SaveChanges();
                return sessionModel.SessionId;
            }
            catch (DbUpdateConcurrencyException)
            {
                return Guid.Empty;
            }
        }

        public SessionIdModel Get(int userId)
        {
            var result = _db.SessionId.FirstOrDefault(s => s.UserId == userId);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }

            return result;
        }

        public bool IsAuthorized(SessionIdModel sessionIdModel)
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
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
