namespace GraderDataAccessLayer.Interfaces
{
    using Models;
    using System;

    public interface IMessageRepository : IGenericRepository<MessageModel>, IDisposable
    {
        // Something like get all messages by grade
    }
}
