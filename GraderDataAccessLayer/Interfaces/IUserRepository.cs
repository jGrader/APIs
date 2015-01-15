namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository : IGenericRepository<UserModel>, IDisposable
    {
        Task<UserModel> GetByUsername(string username);

        Task<IEnumerable<UserModel>> GetAllByGraduationYear(string year);
        
        Task<string> GetEmail(int id);
    }
}
