// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenQA.Selenium;
using ShopTests.UI.common.WebDriverMethods;

namespace ShopTests.UI.PassingAuthorization.WebDriverMethods
{
    internal class AuthMethods : BaseMethods
    {
        private static readonly By
            _alertMessageXPath = By.XPath("//div[contains(@class,'alert')]"),
            _authFormXPath = By.XPath("//form[@id='login']"),
            _authInputXPath = By.XPath(".//input[@id='login']"),
            _authPasswordInputXPath = By.XPath(".//input[@id='pasword']"),
            _authSubmitButtonXPath = By.XPath(".//button[@type='submit']"),
            _dropdownAccountXPath = By.XPath(".//*[@class='dropdown-toggle']"),
            _dropdownMenuXPath = By.XPath(".//*[contains(@class,'dropdown-menu')]"),
            _dropdownSignInButtonXPath = By.XPath($".//a[contains(@href,'user/login')]"),
            _dropXPath = By.XPath("//div[contains(@class,'drop')]");

        public AuthMethods(IWebDriver webDriver) : base(webDriver)
        {
        }

        public IWebElement GetDropElement()
        {
            return _webDriver.FindElement(_dropXPath);
        }

        public IWebElement GetDropdownMenuElement()
        {
            return _baseElement.FindElement(_dropdownMenuXPath);
        }

        public void ToggleAccountDropdownElement()
        {
            var dropdownAccount = _baseElement.FindElement(_dropdownAccountXPath);
            dropdownAccount.Click();
        }

        public void SwitchToLoginPage()
        {
            var signInButton = _baseElement.FindElement(_dropdownSignInButtonXPath);
            signInButton.Click();
        }

        public IWebElement GetLoginFormElement()
        {
            return _webDriver.FindElement(_authFormXPath);
        }

        public void SubmitUserInfo(string login, string password)
        {
            _baseElement.FindElement(_authInputXPath).SendKeys(login);
            _baseElement.FindElement(_authPasswordInputXPath).SendKeys(password);
            _baseElement.FindElement(_authSubmitButtonXPath).Click();
        }

        public IWebElement GetAlertMessageElement()
        {
            return _webDriver.FindElement(_alertMessageXPath);
        }
    }
}
