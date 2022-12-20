// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenQA.Selenium;
using ShopTests.UI.AddingProductToCart.WebDriverMethods;

namespace ShopTests.UI.MakingOrder.WebDriverMethods
{
    public struct CustomerInfo
    {
        public CustomerInfo(
            string login,
            string password,
            string name,
            string email,
            string address,
            string note
            )
        {
            this.login = login;
            this.password = password;
            this.name = name;
            this.email = email;
            this.address = address;
            this.note = note;
        }

        public string login;
        public string password;
        public string name;
        public string email;
        public string address;
        public string note;
    }

    internal class MakeOrderMethods : AddToCartMethods
    {
        private static readonly By
            _formXPath = By.XPath("//form[@action='cart/checkout']"),
            _inputLoginXPath = By.XPath(".//input[@id='login']"),
            _inputPasswordXPath = By.XPath(".//input[@id='pasword']"),
            _inputNameXPath = By.XPath(".//input[@id='name']"),
            _inputEmailXPath = By.XPath(".//input[@id='email']"),
            _inputAddressXPath = By.XPath(".//input[@id='address']"),
            _inputNoteXPath = By.XPath(".//textarea[@name='note']"),
            _inputSubmitButtonXPath = By.XPath(".//button[@type='submit']"),
            _modalMakeOrderButtonXPath = By.XPath("//a[@href='cart/view']"),
            _orderAlertXPath = By.XPath("//div[contains(@class,'alert')]");

        public MakeOrderMethods(IWebDriver webDriver) : base(webDriver)
        {
        }

        public void SwitchToCartPage(string baseUrl)
        {
            _webDriver.FindElement(_modalMakeOrderButtonXPath).Click();
        }

        public void SubmitCustomerInfo(CustomerInfo customerInfo)
        {
            var form = _webDriver.FindElement(_formXPath);
            form.FindElement(_inputLoginXPath).SendKeys(customerInfo.login);
            form.FindElement(_inputPasswordXPath).SendKeys(customerInfo.password);
            form.FindElement(_inputNameXPath).SendKeys(customerInfo.name);
            form.FindElement(_inputEmailXPath).SendKeys(customerInfo.email);
            form.FindElement(_inputAddressXPath).SendKeys(customerInfo.address);
            form.FindElement(_inputNoteXPath).SendKeys(customerInfo.note);

            form.FindElement(_inputSubmitButtonXPath).Click();
        }

        public string GetOrderAlertText()
        {
            return _webDriver.FindElement(_orderAlertXPath).Text;
        }
    }
}
