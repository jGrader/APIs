using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraderDataAccessLayer.Repositories;
using GraderDataAccessLayer.Models;

namespace UnitTestProject.Tests
{
    [TestClass]
    public class CourseUserRepositoryTest
    {
        readonly CourseUserRepository _cur = new CourseUserRepository();
            
        [TestMethod]
        public async void TestGetAll()
        {
            var cum = await _cur.GetAll();
            Assert.AreEqual(cum.Count(),1);
        }

        [TestMethod]
        public async void TestGet()
        {
            var cum = await _cur.Get(1);
            Assert.IsNotNull(cum);
        }

        [TestMethod]
        public async void TestGetByCourseId()
        {
            var cum = await _cur.GetAllByCourseId(2);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public async void TestGetByUser()
        {
            var cum = await _cur.GetAllByUser(1);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public async void TestGetByExtensionLimit()
        {
            var cum = await _cur.GetAllByExtensionLimit(1);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public async void TestGetByExcuseLimit()
        {
            var cum = await _cur.GetAllByExcuseLimit(0);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public async void TestGetByPermissions()
        {
            var cum = await _cur.GetAllByPermissions(700);
            Assert.AreEqual(cum.Count(), 1);
        }
    }
}
