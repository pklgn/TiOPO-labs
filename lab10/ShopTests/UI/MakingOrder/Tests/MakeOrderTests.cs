// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopTests.UI.MakingOrder.WebDriverMethods;

namespace ShopTests.UI.MakingOrder.Tests
{
    [TestClass]
    public class MakeOrderTests
    {
        private MakeOrderMethods _makeOrderMethods;
        private IWebDriver _webDriver;
        private string _url = "http://shop.qatl.ru/";
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
            _webDriver.Navigate().GoToUrl(_url);
            _webDriver.Manage().Window.Maximize();

            _makeOrderMethods = new MakeOrderMethods(_webDriver);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _webDriver.Quit();
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "..\\..\\UI\\MakingOrder\\Configs\\Order.xml",
                    "TestIncognito",
                    DataAccessMethod.Sequential)]
        public void MakeOrderFromModal()
        {
            var productLink = TestContext.DataRow["Link"].ToString();
            _webDriver.Navigate().GoToUrl($"{_url}{productLink}");

            _makeOrderMethods.SetBaseElement(_makeOrderMethods.GetSimpleCartElement());
            var quantity = TestContext.DataRow["Quantity"].ToString();
            _makeOrderMethods.AddProductToCart(int.Parse(quantity));
            var cartProduct = _makeOrderMethods.GetCartProduct(0);

            _makeOrderMethods.SwitchToCartPage(_url);

            var customerInfo = new CustomerInfo(
                TestContext.DataRow["Login"].ToString() + DateTime.Now.ToString(),
                TestContext.DataRow["Password"].ToString(),
                TestContext.DataRow["CustomerName"].ToString(),
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + TestContext.DataRow["Email"].ToString(),
                TestContext.DataRow["Address"].ToString(),
                TestContext.DataRow["Note"].ToString()
                );

            _makeOrderMethods.SubmitCustomerInfo(customerInfo);

            Assert.AreEqual(TestContext.DataRow["Alert"].ToString(), _makeOrderMethods.GetOrderAlertText(),
                "Expected to see text according to the user submit info text");
        }
    }
}
