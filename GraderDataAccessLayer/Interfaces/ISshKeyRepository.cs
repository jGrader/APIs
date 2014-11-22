namespace GraderDataAccessLayer.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    interface ISshKeyRepository
    {
        IEnumerable<SshKeyModel> GetAll();
        SshKeyModel Get(int id);
        string GetKeyById(int id);
        string GetKeyByUserId(int id);
        bool Add(SshKeyModel item);
        bool Remove(int id);
        bool Update(SshKeyModel item);
    }
}
