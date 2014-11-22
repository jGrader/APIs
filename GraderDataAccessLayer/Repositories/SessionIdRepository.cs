using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Repositories
{
    public class SessionIdRepository : ISessionIdRepository
    {
        DatabaseContext _db = new DatabaseContext();

        public SessionIdModel Get(int userId)
        {
            var result = _db.SessionId.FirstOrDefault(s => s.UserId == userId);
            if (result == null)
                return null;

            return result;
        }

        public Task<bool> IsAuthorized(int userId)
        {
            throw new NotImplementedException();
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
