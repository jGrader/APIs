namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface IUserRepository
    {
        IEnumerable<UserModel> GetAll();
        IEnumerable<UserModel> GetAllByGraduationYear(int year);
        UserModel Get(int id);
        string GetEmail(int id);
        bool Add(UserModel item);
        bool Remove(int id);
        bool Update(UserModel item);
    }
}
