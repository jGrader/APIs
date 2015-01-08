using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.Tests.RepositoryTests
{
    [TestClass]
    public class CourseRepositoryTests
    {
        #region Initialization and Cleanup
            readonly ICourseRepository _cr = new CourseRepository();

            [ClassInitialize]
            public static void TestInitialize(TestContext ctx)
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            
                var initializer = new DatabaseInitializer();
                Database.SetInitializer(initializer);
            }

            [ClassCleanup]
            public static void Cleanup()
            {
                Database.Delete("DefaultConnection");
            }
        #endregion

        
        [TestMethod]
        public async Task TestGetAll()
        {
            var res = await _cr.GetAll();
            Assert.AreEqual(2, res.Count());
        }

        [TestMethod]
        public async Task TestGet()
        {
            var res = await _cr.Get(1);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public async Task TestGet_null()
        {
            var res = await _cr.Get(100);
            Assert.IsNull(res);
        }


        [TestMethod]
        public async Task TestGetByName()
        {
            var res = await _cr.GetByName("General Procrastination");
            Assert.AreEqual(res.Count(), 1);
        }
        [TestMethod]
        public async Task TestGetByShortName()
        {
            var res = await _cr.GetByShortName("GenPro");
            Assert.AreEqual(res.Count(), 1);
        }
        [TestMethod]
        public async Task TestGetByCourseNumber()
        {
            var res = await _cr.GetByCourseNumber("52001");
            Assert.AreEqual(res.Count(), 1);
        }
        [TestMethod]
        public async Task TestGetBySemester()
        {
            var res = await _cr.GetBySemester(1);
            Assert.AreEqual(res.Count(), 1);
        }
        [TestMethod]
        public async Task TestGetByYear()
        {
            var res = await _cr.GetByYear(2014);
            Assert.AreEqual(res.Count(), 2); 
        }
        [TestMethod]
        public async Task TestGetByStartDate()
        {
            var res = await _cr.GetByStartDate(new DateTime(2014, 10, 22));
            Assert.AreEqual(res.Count(), 1); 
        }
        [TestMethod]
        public async Task TestGetByEndDate()
        {
            var res = await _cr.GetByEndDate(new DateTime(2014, 11, 23));
            Assert.AreEqual(res.Count(), 1); 
        }
        [TestMethod]
        public async Task TestGetByOwnerId()
        {
            var res = await _cr.GetByOwnerId(1);
            Assert.AreEqual(res.Count(), 1); 
        }


        [TestMethod]
        public async Task TestAdd()
        {
            var res = await _cr.Add(new CourseModel { Name = "General Procrastination 2", CourseNumber = "52101", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 2, ShortName = "GenPro2", Year = 2015, OwnerId = 1});
            Assert.IsNotNull(res);
            Assert.IsTrue(await _cr.Delete(res.Id));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TestAdd_nullArg()
        {
            await _cr.Add(null);
        }


        [TestMethod]
        public async Task TestUpdate()
        {
            var oldCourse = await _cr.Get(1);
            var oldValue = oldCourse.Semester;
            oldCourse.Semester = 2500;

            var res = await _cr.Update(oldCourse);
            Assert.AreEqual(res.Semester, 2500);

            // Revert to original value
            oldCourse.Semester = oldValue;
            res = await _cr.Update(oldCourse);
            Assert.AreEqual(res.Semester, oldValue);
        }
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public async Task TestUpdate_nullArg()
        {
            await _cr.Update(null);
        }
        [TestMethod]
        [ExpectedException(typeof (ObjectNotFoundException))]
        public async Task TestUpdate_notFound()
        {
            var existingObject = await _cr.Get(1);
            existingObject.Id = 3000;

            await _cr.Update(existingObject);
        }

        [TestMethod]
        public async Task TestRemove()
        {
            var existingObject = await _cr.Add(new CourseModel { Name = "General Procrastination 2", CourseNumber = "52101", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 2, ShortName = "GenPro2", Year = 2015, OwnerId = 1 });
            var res = await _cr.Delete(existingObject.Id);
            Assert.IsTrue(res);
        }
        [TestMethod]
        [ExpectedException(typeof (ObjectNotFoundException))]
        public async Task TestRemove_notFound()
        {
            await _cr.Delete(3000);
        }    
    }
}
