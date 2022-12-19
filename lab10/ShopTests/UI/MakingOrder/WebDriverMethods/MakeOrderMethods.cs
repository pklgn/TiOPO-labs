// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenQA.Selenium;
using ShopTests.UI.AddingProductToCart.WebDriverMethods;

namespace ShopTests.UI.MakingOrder.WebDriverMethods
{
    internal class MakeOrderMethods : AddToCartMethods
    {
        public MakeOrderMethods(IWebDriver webDriver) : base(webDriver)
        {
        }
    }
}
