namespace UnitTestProject.Tests
{
    using System;
    using System.IO;
    using GraderDataAccessLayer.Interfaces;
    using GraderDataAccessLayer.Models;
    using GraderDataAccessLayer.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubmissionRepositoryTest
    {
        private readonly ISubmissionRepository _repository = new SubmissionRepository();
        
        [TestMethod]
        public void AddFileTest()
        {
            var file1 = new FileModel
            {
                Filename = "C:\\Users\\Filip\\Documents\\test.testing",
                Contents = "Blablabla",
                Username = "fstankovsk"
            };
            var file2 = new FileModel
            {
                Filename = "C:\\Users\\Filip\\Documents\\test.testing",
                Contents = "second file",
                Username = "fstankovsk"
            };

            var res = _repository.AddFile(file1);
            Assert.AreEqual(true, res);
            Assert.AreEqual(true, File.Exists(file1.Filename));

            res = _repository.AddFile(file2);
            Assert.AreEqual(true, res);
            Assert.AreEqual(true, File.Exists(file2.Filename));
        }
    }
}
