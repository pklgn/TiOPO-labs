// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace ShopTests.UI.PassingAuthorization.WebDriverMethods
{
    internal class SearchMethods
    {
        private IWebDriver _webDriver;
        private IWebElement _baseElement;
        private static readonly By
            _categoryLinkXPath = By.XPath(".//a[contains(@href,'category')]"),
            _categoryMenuXPath = By.XPath("//div[@class='menu']"),
            _categoryTagXPath = By.XPath("//ul[contains(@class,'tag')]"),
            _searchBreadCrumbXPath = By.XPath("//*[@class='breadcrumb']"),
            _searchFormXPath = By.XPath("//form[@action='search']"),
            _searchInputXPath = By.XPath(".//*[@id='typeahead']"),
            _searchMenuSuggestionXPath = By.XPath(".//div[contains(@class,'tt-suggestion')]"),
            _searchMenuXPath = By.XPath(".//div[contains(@class,'tt-menu')]"),
            _searchOpenMenuXPath = By.XPath(".//div[contains(@class,'tt-open')]"),
            _searchResultContainerXPath = By.XPath("//div[contains(@class,'prdt-left')]"),
            _searchResultLinkXPath = By.XPath(".//a[contains(@href, 'product/')]"),
            _searchSubmitInputXPath = By.XPath("//input[contains(@class,'tt-hint')]");


        public SearchMethods(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void SetBaseElement(IWebElement webElement)
        {
            _baseElement = webElement;
        }

        public IWebElement GetSearchFormElement()
        {
            return _webDriver.FindElement(_searchFormXPath);
        }

        public IWebElement GetSearchInputElement()
        {
            return _webDriver.FindElement(_searchInputXPath);
        }

        public void FillInputElement(string searchQuery)
        {
            _baseElement.SendKeys(searchQuery);
            WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
            wait.Until(e => _webDriver
                .FindElement(_searchFormXPath)
                .FindElement(_searchOpenMenuXPath)
                .Displayed);
        }

        public IWebElement GetSearchMenuElement()
        {
            return _baseElement.FindElement(_searchMenuXPath);
        }

        public int GetSearchMenuSuggesstionsCount()
        {
            var menuDataset = _baseElement.FindElements(_searchMenuSuggestionXPath);
            return menuDataset.Count;
        }

        public void SubmitSearchInput()
        {
            _baseElement.FindElement(_searchSubmitInputXPath).SendKeys(Keys.Enter);
        }

        public string GetBreadCrumbText()
        {
            var breadCrumb = _webDriver.FindElement(_searchBreadCrumbXPath);
            return breadCrumb.Text;
        }

        public ReadOnlyCollection<IWebElement> GetSearchResults()
        {
            return _webDriver.FindElements(_searchResultLinkXPath);
        }

        public IWebElement GetCategoryMenuElement()
        {
            return _webDriver.FindElement(_categoryMenuXPath);
        }

        public void SelectCategory(string baseCategory, string concreteCategory)
        {
            By baseCategoryXPath = By.XPath($".//a[contains(@href, 'category/{baseCategory}')]");
            var baseCategoryElement = _baseElement.FindElement(baseCategoryXPath);

            if (concreteCategory == baseCategory)
            {
                baseCategoryElement.Click();
                return;
            }
            else
            {
                Actions builder = new Actions(_webDriver);
                builder.MoveToElement(baseCategoryElement).Perform();
            }

            By concreteCategoryXPath = By.XPath($".//a[contains(@href, 'category/{concreteCategory}')]");
            _baseElement.FindElement(concreteCategoryXPath).Click();
        }

        public string SelectSearchResultProduct()
        {
            string href = _baseElement.GetAttribute("href");
            _baseElement.Click();
            return href;
        }

        public string GetProductCategory()
        {
            var categoryTag = _webDriver.FindElement(_categoryTagXPath);
            return categoryTag.FindElement(_categoryLinkXPath).Text;
        }

        public IWebElement GetSearchResultContainerElement()
        {
            return _webDriver.FindElement(_searchResultContainerXPath);
        }
    }
}
