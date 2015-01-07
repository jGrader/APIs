namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository : IDisposable
    {
        Task<UserModel> Get(int id);
        Task<UserModel> GetByUsername(string username);

        Task<IEnumerable<UserModel>> GetAll();
        Task<IEnumerable<UserModel>> GetAllByGraduationYear(string year);
        
        Task<string> GetEmail(int id);

        Task<bool> Delete(int userId);
    }
}
