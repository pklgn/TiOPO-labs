// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace lab9.Tools.Http;

internal class UriExtension
{
    public static Uri AddParameter(Uri uri, string? paramName, string? paramValue)
    {
        if (paramName is null || paramValue is null)
        {
            throw new ArgumentNullException();
        }

        var uriBuilder = new UriBuilder(uri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query[paramName] = paramValue;
        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }
}
