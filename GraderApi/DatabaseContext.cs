using System;
using System.Collections.Generic;

using GraderApi.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace GraderApi
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {

        }

        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class CourseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            var courses = new List<Course>
            {
                new Course { Name = "General Procrastination", CourseNumber = "52001", StarTime = new DateTime(2014, 10, 22), EndTime = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro" },
                new Course { Name = "General Useless Studies", CourseNumber = "71501", StarTime = new DateTime(2014, 1, 2), EndTime = new DateTime(2015, 5, 23), Semester = 2, ShortName = "GenULS" }
            };
            courses.ForEach(c => context.Courses.Add(c));
            context.SaveChanges();
        }
    }
}