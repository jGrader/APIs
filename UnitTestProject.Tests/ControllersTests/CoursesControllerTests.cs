<<<<<<< HEAD
﻿using GraderApi;
=======
﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
>>>>>>> 5eefaf5f4de963a3ba49483443e4523b10ecb45f
using GraderApi.Controllers;
using GraderDataAccessLayer;
using GraderDataAccessLayer.Interfaces;
using GraderDataAccessLayer.Models;
using GraderDataAccessLayer.Repositories;
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
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC01");

            var responseJson = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(responseJson);

            var all = (await Cr.GetAll()).Count();
            Assert.AreEqual(all, courses.Count(), "#CC02");
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
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC03");

            var responseJson = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.AreEqual(1, course.Id, "#CC04");
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
<<<<<<< HEAD
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "#CC05");
=======
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Revert
            var responseJson = await response.Content.ReadAsStringAsync();
            course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.IsTrue(await Cr.Delete(course.Id));
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
            var existingItem = await Cr.Get(1);
            var oldValue = existingItem.Semester;
            existingItem.Semester = 2500;

            var response = await _cc.Update(existingItem.Id, existingItem);

            // Assert
            var responseJson = await response.Content.ReadAsStringAsync();
            var course = JsonConvert.DeserializeObject<CourseModel>(responseJson);
            Assert.AreEqual(2500, course.Semester);

            // Revert
            existingItem.Semester = oldValue;
            var res = await Cr.Update(existingItem);   
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public async Task TestUpdate_invalid()
        {
            // Arrange
            _cc.Request = new HttpRequestMessage();
            _cc.Configuration = new HttpConfiguration();

            // Act
            var existingItem = await Cr.Get(1);
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
            var existingItem = await Cr.Get(1);
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
                Name = "General Procrastination 2",
                CourseNumber = "52101",
                StartDate = new DateTime(2014, 10, 22),
                EndDate = new DateTime(2014, 11, 23),
                Semester = 2,
                ShortName = "GenPro2",
                Year = 2015,
                OwnerId = 1
            };
            var response = await Cr.Add(course);
            Assert.IsNotNull(response);

            var dbItem = await Cr.Get(response.Id);
            Assert.IsNotNull(dbItem);

            var res = await _cc.Delete(response.Id);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            dbItem = await Cr.Get(response.Id);
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
>>>>>>> 5eefaf5f4de963a3ba49483443e4523b10ecb45f
        }
    }
}
