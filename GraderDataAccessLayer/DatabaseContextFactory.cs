namespace GraderDataAccessLayer
{
    using Resources;
    using System.Configuration;
    using System.Data.Entity.Infrastructure;

    public class DatabaseContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create()
        {
            return new DatabaseContext(ConfigurationManager.ConnectionStrings[DatabaseConnections.MySQL].ConnectionString);
        }
    }
}
