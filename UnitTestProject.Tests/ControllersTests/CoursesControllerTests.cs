using GraderApi.Controllers;
using GraderApi.Filters;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace UnitTestProject.Tests.ControllersTests
{
    [TestClass]
    public class CoursesControllerTests
    {
        #region Initialization and Cleanup
        private static UnitOfWork _uow;
        private CoursesController _cc;

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

        [TestInitialize]
        public void Initialize()
        {
           _uow = new UnitOfWork();
            _cc = new CoursesController(_uow);

            _cc.Configuration = new HttpConfiguration();

            _cc.Configuration.Routes.MapHttpRoute(
               name: "CourseRoute",
                routeTemplate: "api/{controller}/{action}/{courseId}",
                defaults: new { },
                constraints: new { controller = "Courses", courseId = new ApiRouteConstraints() }
               );

            _cc.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
                );
        }

        [TestCleanup]
        public void Clean()
        {
            _cc.Dispose();
        }
        [TestMethod]
        public async Task TestAll()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage(HttpMethod.Get, "/api/Courses/All");

            // Act
            var response = await _cc.All();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC01");

            var responseJson = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(responseJson);

            var all = (await _uow.CourseRepository.GetAll()).Count();
            Assert.AreEqual(all, courses.Count(), "#CC02");
        }

        [TestMethod]
        public async Task TestGet()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage(HttpMethod.Get, "/api/Courses/Get/1");

            // Act
            var response = await _cc.Get(1);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC03");

            var responseJson = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.AreEqual(1, course.Id, "#CC04");
        }

        [TestMethod]
        public async Task TestAdd()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage(HttpMethod.Post, "/api/Courses/Add");

            // Act
            var course = new CourseModel()
            {
                Name = "General Pro_uow.CourseRepositoryastination 2",
                CourseNumber = "52101",
                StartDate = new DateTime(2014, 10, 22),
                EndDate = new DateTime(2014, 11, 23),
                Semester = 2,
                ShortName = "GenPro2",
                Year = 2015,
                OwnerId = 1
            };
            var response = await _cc.Add(course);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC05");
        }
        [TestMethod]
        public async Task TestAdd_invalid()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var course = new CourseModel()
            {
                CourseNumber = "52101",
                StartDate = new DateTime(2014, 10, 22),
                EndDate = new DateTime(2014, 11, 23),
                Semester = 2,
                ShortName = "GenPro2",
                Year = 2015,
                OwnerId = 1
            };
            var response = await _cc.Add(course);

            // Assert
            // We get InternalServerError instead of BadRequest because validation in ModelState 
            // happens on binding which we skip so it assumes that the model is valid even though it has Name = null
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        [TestMethod]
        public async Task TestUpdate()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var existingItem = await _uow.CourseRepository.Get(1);
            var oldValue = existingItem.Semester;
            existingItem.Semester = 2500;

            var response = await _cc.Update(existingItem.Id, existingItem);

            // Assert
            var responseJson = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.AreEqual(2500, course.Semester);

            // Revert
            existingItem.Semester = oldValue;
            var res = await _uow.CourseRepository.Update(existingItem);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public async Task TestUpdate_invalid()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var existingItem = await _uow.CourseRepository.Get(1);
            existingItem.Name = null;

            var response = await _cc.Update(existingItem.Id, existingItem);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        [TestMethod]
        public async Task TestUpdate_wrongIds()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var existingItem = await _uow.CourseRepository.Get(1);
            var response = await _cc.Update(2, existingItem);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [TestMethod]
        public async Task TestDelete()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var course = new CourseModel()
            {
                Name = "General Pro_uow.CourseRepositoryastination 2",
                CourseNumber = "52101",
                StartDate = new DateTime(2014, 10, 22),
                EndDate = new DateTime(2014, 11, 23),
                Semester = 2,
                ShortName = "GenPro2",
                Year = 2015,
                OwnerId = 1
            };
            var response = await _uow.CourseRepository.Add(course);
            Assert.IsNotNull(response);

            var dbItem = await _uow.CourseRepository.Get(response.Id);
            Assert.IsNotNull(dbItem);

            var res = await _cc.Delete(response.Id);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            dbItem = await _uow.CourseRepository.Get(response.Id);
            Assert.IsNull(dbItem);
        }
        [TestMethod]
        public async Task TestDelete_internalError()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var res = await _cc.Delete(500);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, res.StatusCode);
        }
    }
}
