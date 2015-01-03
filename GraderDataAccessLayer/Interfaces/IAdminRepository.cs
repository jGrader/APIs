namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    public interface IAdminRepository : IDisposable
    {
        IEnumerable<AdminModel> GetAll();
        AdminModel GetByUserId(int userId);
        AdminModel Get(int id);

        Task<AdminModel> Add(AdminModel item);
        Task<AdminModel> Update(AdminModel item);
        Task<bool> Delete(int userId);
    }
}
