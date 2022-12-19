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
        private static readonly int COMMON_PRODUCT_COUNT = 1;
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
                    "..\\..\\UI\\AddingProductToCart\\Configs\\Cart.xml",
                    "TestProductPage",
                    DataAccessMethod.Sequential)]
        public void MakeOrderFromModal()
        {
            
        }
    }
}
