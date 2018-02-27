using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenparcCourse.Controllers;
using System;
using System.Threading;
using System.Web.Mvc;

namespace SenparcCourse.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private static int FinishedThreadCount = 0;
        private int TotalThreadCount = 100;


        [TestMethod]
        public void LockTest()
        {
            for (int i = 0; i < TotalThreadCount; i++)
            {
                var thread = new Thread(RunSingleLockTest);
                thread.Start();
            }

            while (FinishedThreadCount < TotalThreadCount)
            {
                //等待100次的线程执行完成
                ;
            }

            Console.WriteLine("测试完成，线程总数：" + FinishedThreadCount.ToString());
        }

        public void RunSingleLockTest()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ContentResult result = controller.LockTest() as ContentResult;
            // Assert
            // Assert.IsNotNull(result);
            Console.WriteLine("结果：" + result.Content);

            FinishedThreadCount++; //完成一次 +1 
        }
    }
}
