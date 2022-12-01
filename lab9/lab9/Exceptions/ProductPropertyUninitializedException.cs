// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace lab9.Exceptions;

public class ProductPropertyUnitializedException : System.Exception
{
    public ProductPropertyUnitializedException() : base($"Product property is unitialized")
    {
    }
}
