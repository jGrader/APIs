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
        CourseUserRepository cur = new CourseUserRepository();
            
        [TestMethod]
        public void TestGetAll()
        {
            IEnumerable<CourseUserModel> cum = cur.GetAll();
            Assert.AreEqual(cum.Count(),1);
        }

        [TestMethod]
        public void TestGet()
        {
            CourseUserModel cum = cur.Get(1);
            Assert.IsNotNull(cum);
        }

        [TestMethod]
        public void TestGetByCourseId()
        {
            IEnumerable<CourseUserModel> cum = cur.GetByCourseId(2);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public void TestGetByUser()
        {
            IEnumerable<CourseUserModel> cum = cur.GetByUser(1);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public void TestGetByExtensionLimit()
        {
            IEnumerable<CourseUserModel> cum = cur.GetByExtensionLimit(1);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public void TestGetByExcuseLimit()
        {
            IEnumerable<CourseUserModel> cum = cur.GetByExcuseLimit(0);
            Assert.AreEqual(cum.Count(), 1);
        }

        [TestMethod]
        public void TestGetByPermissions()
        {
            IEnumerable<CourseUserModel> cum = cur.GetByPermissions(700);
            Assert.AreEqual(cum.Count(), 1);
        }
    }
}
