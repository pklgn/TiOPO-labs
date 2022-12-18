using System;
using System.Data;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopTests.UI.PassingAuthorization.WebDriverMethods;

//namespace ShopTests
//{
//    [TestClass]
//    public class ShopTests
//    {
//        private IWebDriver _webDriver;
//        private string _url = "http://shop.qatl.ru/";

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
//            _webDriver.Navigate().GoToUrl(_url);
//            _webDriver.Manage().Window.Maximize();
//        }

//        public TestContext TestContext { get; set; }

//        [TestMethod]
//        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
//                    "..\\..\\UI\\PassingAuthorization\\Configs\\Authorize.xml",
//                    "Test",
//                    DataAccessMethod.Sequential)]
//        public void Authorize()
//        {
//            var authMethods = new AuthMethods(_webDriver);

//            var drop = authMethods.GetDropElement();
//            authMethods.SetBaseElement(drop);

//            var dropdownMenu = authMethods.GetDropdownMenuElement();
//            Assert.IsFalse(dropdownMenu.Displayed,
//                "Expected not to see login dropdown right after opening shop page");

//            authMethods.ToggleAccountDropdownElement();
//            Assert.IsTrue(dropdownMenu.Displayed,
//                "Expected to see login dropdown after toggling account button");
//            authMethods.SetBaseElement(dropdownMenu);

//            authMethods.SwitchToLoginPage();
//            Assert.AreEqual(_url + "user/login", _webDriver.Url, "Expected to switch to the login page");

//            authMethods.SetBaseElement(authMethods.GetLoginFormElement());

//            string login = TestContext.DataRow["Login"].ToString();
//            string password = TestContext.DataRow["Password"].ToString();
//            authMethods.SubmitUserInfo(login, password);

//            string alertMessage = TestContext.DataRow["AlertMessage"].ToString();
//            var alertMessageElement = authMethods.GetAlertMessageElement();
//            Assert.AreEqual(alertMessage, alertMessageElement.Text);
//        }
//    }
//}
