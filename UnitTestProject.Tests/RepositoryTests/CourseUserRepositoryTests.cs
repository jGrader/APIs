﻿using System;
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
        readonly ICourseUserRepository _cur = new CourseUserRepository();

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
            var cum = await _cur.Get(1);
            Assert.IsNotNull(cum);
        }
        [TestMethod]
        public async Task TestGet_null()
        {
            var res = await _cur.Get(1000);
            Assert.IsNull(res);
        }


        [TestMethod]
        public async Task TestGetAll()
        {
            var res = await _cur.GetAll();
            Assert.AreEqual(2, res.Count());
        }
        [TestMethod]
        public async Task TestGetByCourseId()
        {
            var res = await _cur.GetAllByCourseId(1);
            Assert.AreEqual(2, res.Count());
        }
        [TestMethod]
        public async Task TestGetByUser()
        {
            var res = await _cur.GetAllByUser(1);
            Assert.AreEqual(1, res.Count());
        }
        [TestMethod]
        public async Task TestGetByExtensionLimit()
        {
            var res = await _cur.GetAllByExtensionLimit(1);
            Assert.AreEqual(2, res.Count());
        }
        [TestMethod]
        public async Task TestGetByExcuseLimit()
        {
            var res = await _cur.GetAllByExcuseLimit(1);
            Assert.AreEqual(2, res.Count());
        }
        [TestMethod]
        public async Task TestGetByPermissions()
        {
            var res = await _cur.GetAllByPermissions(700);
            Assert.AreEqual(2, res.Count());
        }


        [TestMethod]
        public async Task TestAdd()
        {
            var res = await _cur.Add(new CourseUserModel { UserId = 2, CourseId = 1, ExcuseLimit  = 2, ExtensionLimit = 3, Permissions = 1500 });
            Assert.IsNotNull(res);
            Assert.IsTrue(await _cur.Delete(res.Id));
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
            Assert.AreEqual(res.Permissions, 2500);

            // Revert to original value
            oldCourseUser.Permissions = oldValue;
            res = await _cur.Update(oldCourseUser);
            Assert.AreEqual(res.Permissions, oldValue);
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
            var existingObject = await _cur.Add(new CourseUserModel { UserId = 2, CourseId = 1, ExcuseLimit = 2, ExtensionLimit = 3, Permissions = 1500 });
            var res = await _cur.Delete(existingObject.Id);
            Assert.IsTrue(res);
        }
        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task TestRemove_notFound()
        {
            await _cur.Delete(3000);
        }    
    }
}
