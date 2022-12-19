// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ShopTests.UI.common.WebDriverMethods;

namespace ShopTests.UI.AddingProductToCart.WebDriverMethods
{
    public struct CartProduct
    {
        public CartProduct(string name, string quantity, string price)
        {
            this.name = name;
            this.quantity = quantity;
            this.price = price;
        }

        public string name;
        public string quantity;
        public string price;
    }

    public struct CartTotal
    {
        public CartTotal(string quantity, string price)
        {
            this.quantity = quantity;
            this.price = price;
        }

        public string quantity;
        public string price;
    }

    internal class AddToCartMethods : BaseMethods
    {
        protected static readonly By
            _cartAddXPath = By.XPath(".//*[contains(@class, 'add-to-cart-link')]"),
            _cartQuantityXPath = By.XPath("//input[@name='quantity']"),
            _cartXPath = By.XPath("//*[contains(@class,'simpleCart_shelfItem')]"),
            _modalXPath = By.XPath("//div[contains(@class,'modal-content')]"),
            _modalProductRowsXPath = By.XPath(".//tr[descendant::a]"),
            _modalTotalXPath = By.XPath(".//tr[descendant::td[contains(@class, 'cart')]]"),
            _cartSimpleTotalXpath = By.XPath("//*[@class='simpleCart_total']"),
            _modalCloseXPath = By.XPath("//*[contains(@class, 'close')]");

        protected Dictionary<string, int> _modalProductRowColumns = new Dictionary<string, int>
        {
            {"product", 0},
            {"name", 1},
            {"quantity", 2},
            {"price", 3},
        };

        protected Dictionary<string, int> _modalTotal = new Dictionary<string, int>
        {
            {"quantity", 1},
            {"price", 1},
        };

        public AddToCartMethods(IWebDriver webDriver) : base(webDriver)
        {
        }

        public IWebElement GetSimpleCartElement()
        {
            return _webDriver.FindElement(_cartXPath);
        }

        public void AddProductToCart(int quantity)
        {
            if (quantity != 1)
            {
                var quantityElement = _baseElement.FindElement(_cartQuantityXPath);
                quantityElement.Clear();
                quantityElement.SendKeys(quantity.ToString());
            }

            var cartAdd = _baseElement.FindElement(_cartAddXPath);
            cartAdd.Click();

            WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
            wait.Until(e => _webDriver
                .FindElement(_modalXPath)
                .Displayed);
        }

        public IWebElement GetModalElement()
        {
            return _webDriver.FindElement(_modalXPath);
        }

        public ReadOnlyCollection<IWebElement> GetCartProducts()
        {
            return _webDriver.FindElements(_modalProductRowsXPath);
        }

        public CartProduct GetCartProduct(int index)
        {
            var products = _webDriver.FindElements(_modalProductRowsXPath);
            var product = products[index];

            var columns = product.FindElements(By.XPath(".//td"));

            return new CartProduct(
                columns[_modalProductRowColumns["name"]].Text,
                columns[_modalProductRowColumns["quantity"]].Text,
                columns[_modalProductRowColumns["price"]].Text
                );
        }

        public CartTotal GetCartTotal()
        {
            var total = _webDriver.FindElements(_modalTotalXPath);

            var totalQuantityRow = total.First();
            var totalPriceRow = total.Last();

            var totalQuantityColumns = totalQuantityRow.FindElements(By.XPath(".//td"));
            var totalPriceColumns = totalPriceRow.FindElements(By.XPath(".//td"));

            return new CartTotal(
                totalQuantityColumns[_modalTotal["quantity"]].Text,
                totalPriceColumns[_modalTotal["price"]].Text.Remove(0, 1)
                ); ;
        }

        public string GetSimpleCartTotal()
        {
            return _webDriver.FindElement(_cartSimpleTotalXpath).Text.Remove(0, 1);
        }

        public IWebElement GetProductItemCartElement(string link)
        {
            return _webDriver.FindElement(By.XPath($"//div[descendant::a[contains(@href, '{link}')] and contains(@class, 'product-bottom')]"));
        }

        public void CloseModal()
        {
            _webDriver.FindElement(_modalCloseXPath).Click();
        }
    }
}
