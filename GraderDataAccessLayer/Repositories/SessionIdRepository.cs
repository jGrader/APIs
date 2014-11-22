using System;
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

        public SessionIdModel Get(int userId)
        {
            var result = _db.SessionId.FirstOrDefault(s => s.UserId == userId);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }

            return result;
        }

        public bool IsAuthorized(int? userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            var item = _db.SessionId.FirstOrDefault(i => i.UserId == userId);
            if (item == null || item.ExpirationTime > DateTime.UtcNow)
            {
                return false;
            }

            try
            {
                _db.SessionId.Remove(item);
                item.ExpirationTime = DateTime.UtcNow.AddMinutes(15);
                _db.SessionId.Add(item);

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
