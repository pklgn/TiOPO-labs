// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenQA.Selenium;

namespace ShopTests3.UI.PassingAuthorization.WebDriverMethods
{
    internal class AuthMethods
    {
        private IWebDriver _webDriver;

        AuthMethods(IWebDriver webDriver) => _webDriver = webDriver;

        public bool IsElementVisible(IWebElement element)
        {
            return element.Displayed;
        }
    }
}
