namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;

    public interface IAdminRepository : IGenericRepository<AdminModel>,IDisposable
    {
        Task<AdminModel> GetByUserId(int userId);
    }
}
