using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<Course> Course { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            var courses = new List<Course>
            {
                new Course { Name = "General Procrastination", CourseNumber = "52001", StarTime = new DateTime(2014, 10, 22), EndTime = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro" },
                new Course { Name = "General Useless Studies", CourseNumber = "71501", StarTime = new DateTime(2014, 1, 2), EndTime = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS" }
            };
            courses.ForEach(c => context.Course.Add(c));
            context.SaveChanges();
        }
    }
}