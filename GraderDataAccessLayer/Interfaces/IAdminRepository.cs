namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface IAdminRepository : IDisposable
    {
        Task<AdminModel> Get(int id);
        Task<AdminModel> GetByUserId(int userId);
        Task<IEnumerable<AdminModel>> GetAll();

        Task<AdminModel> Add(AdminModel item);
        Task<AdminModel> Update(AdminModel item);
        Task<bool> Delete(int userId);
    }
}
