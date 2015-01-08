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
            var users = new List<UserModel>
            {
                new UserModel {Email = "f.stankovski@jacobs-university.de", GraduationYear = "2015", Name = "Filip", PasswordHash = "", Surname = "Stankovski", UserName = "fstankovsk"},
                new UserModel { Email = "a.hegyes@jacobs-university.de", GraduationYear = "2017", Name = "Antonius Cezar", PasswordHash = "", Surname = "Hegyes", UserName = "ahegyes" }
            };
            users.ForEach(u => context.User.Add(u));
            context.SaveChanges();

            var admins = new List<AdminModel>
            {
                new AdminModel { UserId = 1, IsSuperUser  = true },
                new AdminModel { UserId = 2, IsSuperUser =  true }
            };
            admins.ForEach(a => context.Admin.Add(a));
            context.SaveChanges();
            
            var courses = new List<CourseModel>
            {
                new CourseModel { Name = "General Procrastination", CourseNumber = "52001", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro", Year = 2014, OwnerId = 1},
                new CourseModel { Name = "General Useless Studies", CourseNumber = "71501", StartDate = new DateTime(2014, 1, 2), EndDate = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS", Year = 2014, OwnerId = 2}
            };
            courses.ForEach(c => context.Course.Add(c));
            context.SaveChanges();

            var gradeComponents = new List<GradeComponentModel>
            {
                new GradeComponentModel() { CourseId = 2, Name = "Midterm examination", Percentage = 20 },
                new GradeComponentModel() { CourseId = 2, Name = "Final examination", Percentage = 30 },
                new GradeComponentModel() { CourseId = 2, Name = "Homework assignments", Percentage = 20 },
                new GradeComponentModel() { CourseId = 2, Name = "Weekly quizes", Percentage = 30 },
            };
            gradeComponents.ForEach(g => context.GradeComponent.Add(g));
            context.SaveChanges();

            var courseUsers = new List<CourseUserModel>
            {
                new CourseUserModel {ExcuseLimit = 0, ExtensionLimit = 1, UserId = 1, CourseId = 2, Permissions = 700}
            };
            
            courseUsers.ForEach(cu => context.CourseUser.Add(cu));
            context.SaveChanges();
        }
    }
}