using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OpenQA.Selenium;
using System.Data;

namespace ShopTests3
{
    [TestClass]
    public class ShopTests
    {
        private IWebDriver _webDriver;
        private string _url = "http://shop.qatl.ru/";

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
            _webDriver.Navigate().GoToUrl(_url);
            _webDriver.Manage().Window.Maximize();
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "..\\..\\UI\\PassingAuthorization\\Configs\\Authorize.xml",
                    "Test",
                    DataAccessMethod.Sequential)]
        public void TestMethod1()
        {
            string login = TestContext.DataRow["Login"].ToString();
            Assert.IsTrue(true);
        }
    }
}
