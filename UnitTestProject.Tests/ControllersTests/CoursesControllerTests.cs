using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Windows.Markup;
using GraderApi;
using GraderApi.Controllers;
using GraderApi.Handlers;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTestProject.Tests.ControllersTests
{
    [TestClass]
    public class CoursesControllerTests
    {
        #region Initialization and Cleanup

        static readonly ICourseRepository Cr = new CourseRepository();
        readonly CoursesController _cc = new CoursesController(Cr);

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
        public async Task TestAll()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var response = await _cc.All();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(responseJson); 
            Assert.AreEqual(2, courses.Count());
        }

        [TestMethod]
        public async Task TestGet()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();
            
            // Act
            var response = await _cc.Get(1);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.AreEqual(1, course.Id);
        }

        [TestMethod]
        public async Task TestAdd()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();
            
            // Act
            var course = new CourseModel()
            {
                Name = "General Procrastination 2", CourseNumber = "52101",
                StartDate = new DateTime(2014, 10, 22), EndDate = new DateTime(2014, 11, 23), Semester = 2,
                ShortName = "GenPro2", Year = 2015, OwnerId = 1
            };
            var response = await _cc.Add(course);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.IsTrue(await Cr.Delete(course.Id));
        }
    }
}
