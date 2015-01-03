using System;
using System.Collections.Generic;
using System.Linq;
using GraderDataAccessLayer.Repositories;
using GraderDataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestProject.Tests
{
    [TestClass]
    public class CourseRepositoryTest
    {
        readonly CourseRepository _cr = new CourseRepository();

        [TestMethod]
        public void TestGetAll()
        { 
            IEnumerable<CourseModel> res = _cr.GetAll();
            Assert.AreEqual(res.Count(),2);
        }

        [TestMethod]
        public void TestGet()
        {
            CourseModel res = _cr.Get(1);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void TestGetByName()
        {
            IEnumerable<CourseModel> res = _cr.GetByName("General Procrastination");
            Assert.AreEqual(res.Count(), 1);
        }

        [TestMethod]
        public void TestGetByShortName()
        {
            IEnumerable<CourseModel> res = _cr.GetByShortName("GenPro");
            Assert.AreEqual(res.Count(), 1);
        }

        [TestMethod]
        public void TestGetByCourseNumber()
        {
            IEnumerable<CourseModel> res = _cr.GetByCourseNumber("52001");
            Assert.AreEqual(res.Count(), 1);
        }

        [TestMethod]
        public void TestGetBySemester()
        {
            IEnumerable<CourseModel> res = _cr.GetBySemester(1);
            Assert.AreEqual(res.Count(), 1);
        }

        [TestMethod]
        public void TestGetByYear()
        {
            IEnumerable<CourseModel> res = _cr.GetByYear(2014);
            Assert.AreEqual(res.Count(), 2); 
        }

        [TestMethod]
        public void TestGetByStartDate()
        {
            IEnumerable<CourseModel> res = _cr.GetByStartDate(new DateTime(2014, 10, 22));
            Assert.AreEqual(res.Count(), 1); 
        }

        [TestMethod]
        public void TestGetByEndDate()
        {
            IEnumerable<CourseModel> res = _cr.GetByEndDate(new DateTime(2014, 11, 23));
            Assert.AreEqual(res.Count(), 1); 
        }

        [TestMethod]
        public void TestGetByOwnerId()
        {
            IEnumerable<CourseModel> res = _cr.GetByOwnerId(1);
            Assert.AreEqual(res.Count(), 1); 
        }

        [TestMethod]
        public void TestGetByLambda()
        {
             
        }

        [TestMethod]
        public void TestAdd()
        {
            _cr.Add(new CourseModel { Name = "General Procrastination", CourseNumber = "52001", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro", Year = 2014, OwnerId = 1});
            
            IEnumerable<CourseModel> res = _cr.GetByOwnerId(1);
            Assert.AreEqual(res.Count(), 2); 
        }

        [TestMethod]
        public void TestRemove()
        {
            _cr.Remove(1);
            IEnumerable<CourseModel> res = _cr.GetByYear(2014);
            Assert.AreEqual(res.Count(), 1); 
        }

        [TestMethod]
        public void TestUpdate()
        {
            CourseModel cm = new CourseModel { Id = 1, Name = "General Procrastination", CourseNumber = "52001", StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 1, ShortName = "GenPro", Year = 2022, OwnerId = 1 };
            IEnumerable<CourseModel> res = _cr.GetByYear(2022);
            Assert.AreEqual(res.Count(), 1);
        }
    }
}
