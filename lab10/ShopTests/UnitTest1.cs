using System;
using System.Data;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShopTests
{
    [TestClass]
    public class ShopTests
    {
        private IWebDriver _webDriver;
        private string _url = "http://shop.qatl.ru/";

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
            _webDriver.Navigate().GoToUrl(_url);
            _webDriver.Manage().Window.Maximize();
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "..\\..\\UI\\PassingAuthorization\\Configs\\Authorize.xml",
                    "Test",
                    DataAccessMethod.Sequential)]
        public void Authorize()
        {
            string login = TestContext.DataRow["Login"].ToString();
            string password = TestContext.DataRow["Password"].ToString();
            string alertMessage = TestContext.DataRow["AlertMessage"].ToString();


            By _dropXPath = By.XPath("//div[contains(@class, 'drop')]");
            var drop = _webDriver.FindElement(_dropXPath);

            By _dropdownMenuXPath = By.XPath("//*[contains(@class, 'dropdown-menu')]");
            var dropdownMenu = drop.FindElement(_dropdownMenuXPath);
            Assert.IsFalse(dropdownMenu.Displayed);

            By _accountDropdownXPath = By.XPath("//*[@class='dropdown-toggle']");
            var accountDropdown = drop.FindElement(_accountDropdownXPath);
            accountDropdown.Click();
            Assert.IsTrue(dropdownMenu.Displayed);

            By _signInButtonXPath = By.XPath("//a[contains(@href, 'user/login')]");
            var signInButton = dropdownMenu.FindElement(_signInButtonXPath);
            signInButton.Click();

            Assert.AreEqual(_url + "user/login", _webDriver.Url, "Expected to go to the login page");

            By _loginFormXPath = By.XPath("//form[@id='login']");
            var loginForm = _webDriver.FindElement(_loginFormXPath);

            By _loginInputXPath = By.XPath("//input[@id='login']");
            var loginInput = loginForm.FindElement(_loginInputXPath);
            loginInput.SendKeys(login);

            By _passwordInputXPath = By.XPath("//input[@id='pasword']");
            var passwordInput = loginForm.FindElement(_passwordInputXPath);
            passwordInput.SendKeys(password);

            By _submitButtonXPath = By.XPath("//button[@type = 'submit']");
            var submitButton = loginForm.FindElement(_submitButtonXPath);
            submitButton.Click();

            By _alertMessageDivXPath = By.XPath("//div[contains(@class, 'alert')]");
            var alertMessageDiv = _webDriver.FindElement(_alertMessageDivXPath);
            Assert.AreEqual(alertMessage, alertMessageDiv.Text);

            Assert.IsTrue(true);
        }
    }
}
