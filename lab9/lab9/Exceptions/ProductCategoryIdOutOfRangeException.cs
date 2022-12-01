// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace lab9.Exceptions;

public class ProductCategoryIdOutOfRangeException : System.Exception
{
    public ProductCategoryIdOutOfRangeException() : base("Product category_id property is out of range")
    {
    }
}
