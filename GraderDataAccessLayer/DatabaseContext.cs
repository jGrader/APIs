using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using GraderDataAccessLayer.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace GraderDataAccessLayer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<CourseModel> Course { get; set; }
        public DbSet<AdminModel> Admin { get; set; }
        public DbSet<CourseUserModel> CourseUser { get; set; }
        public DbSet<EntityModel> Entity { get; set; }
        public DbSet<ExtensionModel> Extension { get; set; }
        public DbSet<GradeComponentModel> GradeComponent { get; set; }
        public DbSet<GradeModel> Grade { get; set; }
        public DbSet<SshKeyModel> SSHKey { get; set; }
        public DbSet<SubmissionModel> Submission { get; set; }
        public DbSet<TaskModel> Task { get; set; }
        public DbSet<TeamMemberModel> TeamMember { get; set; }
        public DbSet<TeamModel> Team { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<SessionIdModel> SessionId { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            var courses = new List<CourseModel>
            {
                new CourseModel { Name = "General Procrastination", CourseNumber = "52001", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro" },
                new CourseModel { Name = "General Useless Studies", CourseNumber = "71501", StartDate = new DateTime(2014, 1, 2), EndDate = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS" }
            };

            var users = new List<UserModel>
            {
                new UserModel {Email = "f.stankovski@jacobs-university.de", GraduationYear = "2015", Name = "Filip", PasswordHash = "", Surname = "Stankovski", UserName = "fstankovsk"}
            };

            var sessions = new List<SessionIdModel>
            {
                new SessionIdModel {ExpirationTime = new DateTime(2014, 12, 31), SessionId = new Guid(), UserId = 1}
            };

            var courseUsers = new List<CourseUserModel>
            {
                new CourseUserModel {ExcuseLimit = 0, ExtensionLimit = 1, UserId = 1, CourseId = 2, Permissions = 700}
            };
            courses.ForEach(c => context.Course.Add(c));
            users.ForEach(u => context.User.Add(u));

            context.SaveChanges();

            courseUsers.ForEach(cu => context.CourseUser.Add(cu));
            sessions.ForEach(s => context.SessionId.Add(s));

            context.SaveChanges();
        }
    }
}