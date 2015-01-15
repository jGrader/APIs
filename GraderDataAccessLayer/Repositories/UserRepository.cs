namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;


    public class UserRepository : GenericRepository<UserModel>, IUserRepository
    {

        public UserRepository(DatabaseContext db) : base(db)
        { }

        public async Task<UserModel> GetByUsername(string username)
        {
            var searchResult = await dbSet.FirstOrDefaultAsync(u => u.UserName == username);
            return searchResult;
        }

        public async Task<IEnumerable<UserModel>> GetAllByGraduationYear(string year)
        {
            return await Task.Run(() => dbSet.Where(w => w.GraduationYear == year));
        }

        public async Task<string> GetEmail(int id)
        {
            var searchResult = await dbSet.FirstOrDefaultAsync(w => w.Id == id);
            return searchResult == null ? string.Empty : searchResult.Email;
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
