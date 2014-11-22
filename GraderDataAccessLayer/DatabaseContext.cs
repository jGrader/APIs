using System;
using System.Collections.Generic;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            var courses = new List<CourseModel>
            {
                new CourseModel { Name = "General Procrastination", CourseNumber = "52001", StarTime = new DateTime(2014, 10, 22), EndTime = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro" },
                new CourseModel { Name = "General Useless Studies", CourseNumber = "71501", StarTime = new DateTime(2014, 1, 2), EndTime = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS" }
            };
            courses.ForEach(c => context.Course.Add(c));
            context.SaveChanges();
        }
    }
}