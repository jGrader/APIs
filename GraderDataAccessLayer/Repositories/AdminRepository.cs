namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class AdminRepository : GenericRepository<AdminModel>,IAdminRepository
    {
        public AdminRepository(DatabaseContext db) : base(db)
        { }

        public async Task<AdminModel> GetByUserId(int userId)
        {
            var searchResult = await dbSet.FirstOrDefaultAsync(a => a.UserId == userId);
            return searchResult;
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
