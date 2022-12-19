// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenQA.Selenium;

namespace ShopTests.UI.common.WebDriverMethods
{
    internal class BaseMethods
    {
        protected IWebDriver _webDriver;
        protected IWebElement _baseElement;

        public BaseMethods(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void SetBaseElement(IWebElement webElement)
        {
            _baseElement = webElement;
        }
    }
}
