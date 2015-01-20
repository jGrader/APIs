namespace GraderDataAccessLayer.Repositories
{
    using System;
    using Interfaces;
    using Models;

    public class MessageRepository : GenericRepository<MessageModel>, IMessageRepository
    {
        public MessageRepository(DatabaseContext db) : base(db)
        { }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (Context == null)
            {
                return;
            }

            Context.Dispose();
            Context = null;
        }
    }
}
