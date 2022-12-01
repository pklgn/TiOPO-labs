// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace lab9.Exceptions;

public class ProductStatusOutOfRangeException : System.Exception
{
    public ProductStatusOutOfRangeException() : base("Product status property is out of range")
    {
    }
}
