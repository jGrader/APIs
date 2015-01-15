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
    public class CourseUserRepositoryTests
    {
        #region Initialization and Cleanup
        private DatabaseContext _context =  new DatabaseContext();
        private ICourseUserRepository _cur;

        [ClassInitialize]
        public static void TestInitialize(TestContext ctx)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            var initializer = new DatabaseInitializer();
            Database.SetInitializer(initializer);
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            Database.Delete("DefaultConnection");
        }

        [TestInitialize]
        public void Initialize()
        {
            _cur = new CourseUserRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cur.Dispose();
        }
        #endregion

        [TestMethod]
        public async Task TestGet()
        {
            var cum = await _cur.Get(1);
            Assert.IsNotNull(cum, "#CUR01");
        }
        [TestMethod]
        public async Task TestGet_null()
        {
            var res = await _cur.Get(1000);
            Assert.IsNull(res, "#CUR02");
        }


        [TestMethod]
        public async Task TestGetAll()
        {
            var res = await _cur.GetAll();
            Assert.AreEqual(2, res.Count(), "#CUR03");
        }
        [TestMethod]
        public async Task TestGetByCourseId()
        {
            var res = await _cur.GetByCourseId(1);
            Assert.AreEqual(2, res.Count(), "#CUR04");
        }
        [TestMethod]
        public async Task TestGetByUser()
        {
            var res = await _cur.GetByUserId(1);
            Assert.AreEqual(1, res.Count(), "#CUR05");
        }
        [TestMethod]
        public async Task TestGetByPermissions()
        {
            var res = await _cur.GetByPermissions(700);
            Assert.AreEqual(2, res.Count(), "#CUR08");
        }


        [TestMethod]
        public async Task TestAdd()
        {
            var res = await _cur.Add(new CourseUserModel { UserId = 2, CourseId = 1, ExcuseNumber = 2, ExtensionNumber = 3, Permissions = 1500 });
            Assert.IsNotNull(res, "#CUR09");
            Assert.IsTrue(res.Id > 0, "#CUR10");

            var query = await _cur.Get(res.Id);
            Assert.IsNotNull(query, "#CUR11");

            // Revert
            var isDeleted = await _cur.Delete(query.Id);
            Assert.IsTrue(isDeleted, "#CUR12");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TestAdd_nullArg()
        {
            await _cur.Add(null);
        }


        [TestMethod]
        public async Task TestUpdate()
        {
            var oldCourseUser = await _cur.Get(1);
            var oldValue = oldCourseUser.Permissions;
            oldCourseUser.Permissions = 2500;

            var res = await _cur.Update(oldCourseUser);
            Assert.AreEqual(res.Permissions, 2500, "#CUR13");

            // Revert to original value
            oldCourseUser.Permissions = oldValue;
            res = await _cur.Update(oldCourseUser);
            Assert.AreEqual(res.Permissions, oldValue, "#CUR14");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TestUpdate_nullArg()
        {
            await _cur.Update(null);
        }
        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task TestUpdate_notFound()
        {
            var existingObject = await _cur.Get(1);
            existingObject.Id = 3000;

            await _cur.Update(existingObject);
        }


        [TestMethod]
        public async Task TestRemove()
        {
            // Arrange
            var res = await _cur.Add(new CourseUserModel { UserId = 2, CourseId = 1, ExcuseNumber = 2, ExtensionNumber = 3, Permissions = 1500 });
            Assert.IsNotNull(res, "#CUR09");
            Assert.IsTrue(res.Id > 0, "#CUR10");

            var existingObject = (await _cur.GetByPermissions(1500)).ToList();
            Assert.IsNotNull(existingObject, "#CUR15");
            Assert.IsTrue(existingObject.Any(), "#CUR16");

            // Act
            var query = await _cur.Delete(existingObject.First().Id);
            Assert.IsTrue(query, "#CUR14");
        }
        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task TestRemove_notFound()
        {
            await _cur.Delete(3000);
        }
    }
}
