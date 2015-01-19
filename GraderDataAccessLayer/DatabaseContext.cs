namespace GraderDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using GraderDataAccessLayer.Models;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Data.Entity.Infrastructure;
    using MySql.Data.Entity;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultMySqlConnection")
        {
        }

        public DbSet<CourseModel> Course { get; set; }
        public DbSet<AdminModel> Admin { get; set; }
        public DbSet<CourseUserModel> CourseUser { get; set; }
        public DbSet<EntityModel> Entity { get; set; }
        public DbSet<ExtensionModel> Extension { get; set; }
        public DbSet<ExcuseModel> Excuse { get; set; }
        public DbSet<FileModel> File { get; set; }
        public DbSet<GradeComponentModel> GradeComponent { get; set; }
        public DbSet<GradeModel> Grade { get; set; }
        public DbSet<MessageModel> Message { get; set; }
        public DbSet<SshKeyModel> SshKey { get; set; }
        public DbSet<SubmissionModel> Submission { get; set; }
        public DbSet<TaskModel> Task { get; set; }
        public DbSet<TeamModel> Team { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<SessionIdModel> SessionId { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Resolve the Many-To-Many relation between Teams and Users
            modelBuilder.Entity<TeamModel>().HasMany(c => c.TeamMembers).WithMany(p => p.Teams).
                Map(
                    m =>
                    {
                        m.MapLeftKey("TeamId");
                        m.MapRightKey("UserId");
                        m.ToTable("UserTeams");
                    });
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
                new CourseModel { Name = "General Procrastination", CourseNumber = "52001", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro", Year = 2014, OwnerId = 1, ExtensionLimit = 2, ExcuseLimit = 1 },
                new CourseModel { Name = "General Useless Studies", CourseNumber = "71501", StartDate = new DateTime(2014, 1, 2), EndDate = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS", Year = 2014, OwnerId = 2, ExtensionLimit = 3, ExcuseLimit = 0 }
            };
            courses.ForEach(c => context.Course.Add(c));
            context.SaveChanges();

            var courseUsers = new List<CourseUserModel>
            {
                new CourseUserModel {ExcuseNumber = 0, ExtensionNumber = 0, UserId = 1, CourseId = 1, Permissions = 700},
                new CourseUserModel {ExcuseNumber = 0, ExtensionNumber = 0, UserId = 2, CourseId = 1, Permissions = 700}
            };

            courseUsers.ForEach(cu => context.CourseUser.Add(cu));
            context.SaveChanges();

            var gradeComponents = new List<GradeComponentModel>
            {
                new GradeComponentModel { CourseId = 2, Name = "Midterm examination", Percentage = 20 },
                new GradeComponentModel { CourseId = 2, Name = "Final examination", Percentage = 30 },
                new GradeComponentModel { CourseId = 2, Name = "Homework assignments", Percentage = 20 },
                new GradeComponentModel { CourseId = 2, Name = "Weekly quizes", Percentage = 30 },
                new GradeComponentModel { CourseId = 1, Name = "Monthly quizzes", Percentage = 30 },
                new GradeComponentModel { CourseId = 1, Name = "Final exam", Percentage = 70 }
            };
            gradeComponents.ForEach(g => context.GradeComponent.Add(g));
            context.SaveChanges();

            var tasks = new List<TaskModel>
            {
                new TaskModel { CourseId = 2, GradeComponentId = 1, Name = "Assignment 1" },
                new TaskModel { CourseId = 1, GradeComponentId = 5, Name = "Week 1 problems" }
            };
            tasks.ForEach(t => context.Task.Add(t));
            context.SaveChanges();

            var entities = new List<EntityModel>
            {
                new EntityModel { Name = "Problem 1.1", Points = 10, BonusPoints = 0, OpenTime = new DateTime(2015, 1, 10), CloseTime = new DateTime(2015, 1, 17), TaskId = 1 },
                new EntityModel { Name = "Problem 1.2", Points = 25, BonusPoints = 0, OpenTime = new DateTime(2015, 1, 10), CloseTime = new DateTime(2015, 1, 17), TaskId = 1 },
                new EntityModel { Name = "Problem 1.3", Points = 15, BonusPoints = 0, OpenTime = new DateTime(2015, 1, 10), CloseTime = new DateTime(2015, 1, 17), TaskId = 1 },
                new EntityModel { Name = "Test 1", Points = 15, BonusPoints = 0, OpenTime = new DateTime(2015, 1, 10), CloseTime = new DateTime(2015, 1, 17), TaskId = 2 },
                new EntityModel { Name = "Test 2", Points = 25, BonusPoints = 0, OpenTime = new DateTime(2015, 1, 10), CloseTime = new DateTime(2015, 1, 17), TaskId = 2 },
            };
            entities.ForEach(e => context.Entity.Add(e));
            context.SaveChanges();

            var files = new List<FileModel>
            {
                new FileModel { EntityId = 1, FileName = "file1.1", Extension = ".pdf" },
                new FileModel { EntityId = 1, FileName = "file1.2", Extension = ".jpg" },
                new FileModel { EntityId = 2, FileName = "file2", Extension = ".sml" },
                new FileModel { EntityId = 3, FileName = "file3", Extension = ".cpp" },
                new FileModel { EntityId = 4, FileName = "file4", Extension = ".pdf" },
                new FileModel { EntityId = 5, FileName = "file5", Extension = ".bmp" }
            };
            files.ForEach(f => context.File.Add(f));
            context.SaveChanges();

            // Create Teams
            var teams = new List<TeamModel>
            {
                new TeamModel { EntityId = 1 },
                new TeamModel { EntityId = 2 }
            };
            teams[0].TeamMembers.Add(users[0]);
            teams[0].TeamMembers.Add(users[1]);
            teams[1].TeamMembers.Add(users[0]);
            teams[1].TeamMembers.Add(users[1]);

            teams.ForEach(t => context.Team.Add(t));
            context.SaveChanges();

            var grades = new List<GradeModel>
            {
                new GradeModel { UserId = 1, EntityId = 2, Grade = 10, BonusGrade = 0, GraderId = 1, TimeStamp = DateTime.Now, Comment = "This is what you got. Congratulations"},
                new GradeModel { UserId = 1, EntityId = 3, Grade = 10, BonusGrade = 0, GraderId = 1, TimeStamp = DateTime.Now, Comment = "This is what you got again. Congratulations"}
            };

            grades.ForEach(g => context.Grade.Add(g));
            context.SaveChanges();
        }
    }
}