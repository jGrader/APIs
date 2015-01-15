namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;


    public interface ISshKeyRepository : IGenericRepository<SshKeyModel>, IDisposable
    {
        Task<string> GetKeyById(int id);
        Task<string> GetKeyByUserId(int id);
    }
}
