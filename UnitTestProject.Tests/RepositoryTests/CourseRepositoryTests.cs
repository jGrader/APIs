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
        public async Task TestGet()
        {
            var res = await _cr.Get(1);
            Assert.IsNotNull(res, "#CR01");
        }
        [TestMethod]
        public async Task TestGet_null()
        {
            var res = await _cr.Get(100);
            Assert.IsNull(res, "#CR02");
        }


        [TestMethod]
        public async Task TestGetByName()
        {
            var res = await _cr.GetByName("General Procrastination");
            Assert.AreEqual(res.Count(), 1, "#CR03");
        }
        [TestMethod]
        public async Task TestGetByShortName()
        {
            var res = await _cr.GetByShortName("GenPro");
            Assert.AreEqual(res.Count(), 1, "#CR04");
        }
        [TestMethod]
        public async Task TestGetByCourseNumber()
        {
            var res = await _cr.GetByCourseNumber("52001");
            Assert.AreEqual(res.Count(), 1, "#CR05");
        }
        [TestMethod]
        public async Task TestGetBySemester()
        {
            var res = await _cr.GetBySemester(1);
            Assert.AreEqual(res.Count(), 1, "#CR06");
        }
        [TestMethod]
        public async Task TestGetByYear()
        {
            var res = await _cr.GetByYear(2014);
            Assert.AreEqual(res.Count(), 2, "#CR07"); 
        }
        [TestMethod]
        public async Task TestAdd()
        {
            var res = await _cr.Add(new CourseModel { Name = "General Procrastination 2", CourseNumber = "52101", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 2, ShortName = "GenPro2", Year = 2015, OwnerId = 1});
            Assert.IsNotNull(res, "#CR08");
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
            Assert.AreEqual(res.Semester, 2500, "#CR09");

            // Revert to original value
            oldCourse.Semester = oldValue;
            res = await _cr.Update(oldCourse);
            Assert.AreEqual(res.Semester, oldValue, "#CR10");
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
            var existingObject = await _cr.GetByName("General Procrastination 2");
            var res = await _cr.Delete(existingObject.First().Id);
            Assert.IsTrue(res, "#CR11");
        }
        [TestMethod]
        [ExpectedException(typeof (ObjectNotFoundException))]
        public async Task TestRemove_notFound()
        {
            await _cr.Delete(3000);
        }    
    }
}
