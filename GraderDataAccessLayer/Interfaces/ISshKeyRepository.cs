namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    interface ISshKeyRepository
    {
        Task<SshKeyModel> Get(int id);
        Task<string> GetKeyById(int id);
        Task<string> GetKeyByUserId(int id);

        Task<IEnumerable<SshKeyModel>> GetAll();

        Task<SshKeyModel> Add(SshKeyModel item);
        Task<SshKeyModel> Update(SshKeyModel item);
        Task<bool> Delete(int id);
    }
}
