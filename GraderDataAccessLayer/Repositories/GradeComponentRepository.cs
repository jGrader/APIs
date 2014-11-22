using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Interfaces;

namespace GraderDataAccessLayer.Repositories
{
    class GradeComponentRepository : IGradeComponentRepository
    {
        private DatabaseContext _db = new DatabaseContext();

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
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
