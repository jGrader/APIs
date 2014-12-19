

using System.Data;
using System.Data.Entity.Infrastructure;
using GraderDataAccessLayer.Interfaces;

namespace GraderDataAccessLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using GraderDataAccessLayer.Models;
    using System.Linq;
    using System.Data.Entity.Core;
    using System.Text;
    using System.Threading.Tasks;

    public class UserRepository : IUserRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public IEnumerable<UserModel> GetAll()
        {
            var result = _db.User.Where(u => u.Id > 0);
            if (!result.Any())
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public IEnumerable<UserModel> GetAllByGraduationYear(int year)
        {
            throw new NotImplementedException();
        }

        public UserModel Get(int id)
        {
            var result = _db.User.FirstOrDefault(u => u.Id == id);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public UserModel GetByUsername(string username)
        {
            var result = _db.User.FirstOrDefault(u => u.UserName == username);
            if (result == null)
            {
                throw new ObjectNotFoundException();
            }
            return result;
        }

        public string GetEmail(int id)
        {
            throw new NotImplementedException();
        }

        public bool Add(UserModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.User.Add(item);
                _db.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(UserModel item)
        {
            var dbItem = _db.User.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.User.Remove(dbItem);
                _db.User.Add(item);
                _db.SaveChanges();

                return true;
            }
            catch (DBConcurrencyException)
            {
                return false;
            }
        }
    }
}
