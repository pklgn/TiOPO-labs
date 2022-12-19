// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using ShopTests.UI.PassingAuthorization.WebDriverMethods;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace ShopTests.UI.SearchingProductInCatalog.Tests
{
    internal static class CustomWebElementAssert
    {
        public static void IsEachWebElementEnabled(this Assert assert, ReadOnlyCollection<IWebElement> webElements)
        {
            foreach (var element in webElements)
            {
                Assert.IsTrue(element.Enabled,
                    "Expected to see enabled web element");
            }
        }
    }


    [TestClass]
    public class SearchTests
    {
        private SearchMethods _searchMethods;
        private IWebDriver _webDriver;
        private string _url = "http://shop.qatl.ru/";
        private string _searchUrl = "search/";
        private string _categoryUrl = "category/";
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_DIR"));
            _webDriver.Navigate().GoToUrl(_url);
            _webDriver.Manage().Window.Maximize();

            _searchMethods = new SearchMethods(_webDriver);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _webDriver.Quit();
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "..\\..\\UI\\SearchingProductInCatalog\\Configs\\Search.xml",
                    "TestSearch",
                    DataAccessMethod.Sequential)]
        public void SearchProducts()
        {
            _searchMethods.SetBaseElement(_searchMethods.GetSearchFormElement());
            var searchMenu = _searchMethods.GetSearchMenuElement();

            _searchMethods.SetBaseElement(_searchMethods.GetSearchInputElement());

            Assert.IsFalse(searchMenu.Displayed,
                "Expected not to see search menu when nothing was given to search");
            string query = TestContext.DataRow["Query"].ToString();
            _searchMethods.FillInputElement(query);
            Assert.IsTrue(searchMenu.Displayed,
                "Expected to see search menu when text was given to search");

            int suggestionsCount = int.Parse(TestContext.DataRow["Count"].ToString());
            _searchMethods.SetBaseElement(searchMenu);
            Assert.AreEqual(suggestionsCount, _searchMethods.GetSearchMenuSuggesstionsCount(),
                $"Expected to get {suggestionsCount} number of suggestions");

            _searchMethods.SubmitSearchInput();
            Assert.AreEqual($"{_url}{_searchUrl}", $"{ _webDriver.Url.Split('?').First()}/",
                "Expected to switch to search result page");
            Assert.IsTrue(_searchMethods.GetBreadCrumbText().Contains(query),
                "Expected to have query string in search bread crumb");

            Assert.That.IsEachWebElementEnabled(_searchMethods.GetSearchResults());
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "..\\..\\UI\\SearchingProductInCatalog\\Configs\\Search.xml",
            "TestCategory",
            DataAccessMethod.Sequential)]
        public void SearchCategory()
        {
            _searchMethods.SetBaseElement(_searchMethods.GetCategoryMenuElement());

            string baseCategory = TestContext.DataRow["BaseCategory"].ToString();
            string concreteCategory = TestContext.DataRow["ConcreteCategory"].ToString();
            concreteCategory = (concreteCategory == "")
                ? baseCategory
                : concreteCategory;
            _searchMethods.SelectCategory(baseCategory, concreteCategory);

            Assert.AreEqual($"{_url}{_categoryUrl}{concreteCategory}", _webDriver.Url);
            var searchBreadCrumbText = _searchMethods.GetBreadCrumbText().ToLower();
            Assert.IsTrue(searchBreadCrumbText.Contains(baseCategory));
            Assert.IsTrue(searchBreadCrumbText.Contains(concreteCategory));

            var searchResults = _searchMethods.GetSearchResults();
            if (searchResults.Count == 0)
            {
                var searchResultContainer = _searchMethods.GetSearchResultContainerElement();
                Assert.AreEqual("В этой категории товаров пока нет...", searchResultContainer.Text);
                return;
            }

            _searchMethods.SetBaseElement(searchResults.First());
            var productLink = _searchMethods.SelectSearchResultProduct();
            Assert.AreEqual($"{productLink}", _webDriver.Url);

            var actualCategory = _searchMethods.GetProductCategory().ToLower();
            Assert.AreEqual(concreteCategory, actualCategory);
        }
    }
}
