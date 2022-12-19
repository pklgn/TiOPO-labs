// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using ShopTests.UI.PassingAuthorization.WebDriverMethods;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShopTests.UI.PassingAuthorization.Tests
{
    [TestClass]
    public class AuthTests
    {
        private AuthMethods _authMethods;
        private IWebDriver _webDriver;
        private string _url = "http://shop.qatl.ru/";
        private string _loginUrl = "user/login";
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
            _webDriver.Navigate().GoToUrl(_url);
            _webDriver.Manage().Window.Maximize();

            _authMethods = new AuthMethods(_webDriver);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "..\\..\\UI\\PassingAuthorization\\Configs\\Authorize.xml",
                    "Test",
                    DataAccessMethod.Sequential)]
        public void Authorize()
        {
            var drop = _authMethods.GetDropElement();
            _authMethods.SetBaseElement(drop);

            var dropdownMenu = _authMethods.GetDropdownMenuElement();
            Assert.IsFalse(dropdownMenu.Displayed,
                "Expected not to see login dropdown right after opening shop page");

            _authMethods.ToggleAccountDropdownElement();
            Assert.IsTrue(dropdownMenu.Displayed,
                "Expected to see login dropdown after toggling account button");
            _authMethods.SetBaseElement(dropdownMenu);

            _authMethods.SwitchToLoginPage();
            Assert.AreEqual(_url + _loginUrl, _webDriver.Url,
                "Expected to switch to the login page");

            _authMethods.SetBaseElement(_authMethods.GetLoginFormElement());

            string login = TestContext.DataRow["Login"].ToString();
            string password = TestContext.DataRow["Password"].ToString();
            _authMethods.SubmitUserInfo(login, password);

            string alertMessage = TestContext.DataRow["AlertMessage"].ToString();
            var alertMessageElement = _authMethods.GetAlertMessageElement();
            Assert.AreEqual(alertMessage, alertMessageElement.Text,
                "Expected to get alert message that corresponds user data");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _webDriver.Quit();
        }
    }
}
