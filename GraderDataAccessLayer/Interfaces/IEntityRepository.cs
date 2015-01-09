namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface IEntityRepository : IDisposable
    {
        Task<EntityModel> Get(int id);

        Task<IEnumerable<EntityModel>> GetAll();
        Task<IEnumerable<EntityModel>> GetAllForTask(int taskId);
        Task<IEnumerable<EntityModel>> GetAllByName(string name);
        Task<IEnumerable<EntityModel>> GetAllByCourseId(int courseId);
        Task<IEnumerable<EntityModel>> GetAllByOpenDate(DateTime openTime);
        Task<IEnumerable<EntityModel>> GetAllByCloseDate(DateTime closeTime);
        Task<IEnumerable<EntityModel>> GetAllActiveBetweenDates(DateTime time1, DateTime time2);

        Task<EntityModel> Add(EntityModel item);
        Task<EntityModel> Update(EntityModel item);
        Task<bool> Delete(int id);
    }
}
