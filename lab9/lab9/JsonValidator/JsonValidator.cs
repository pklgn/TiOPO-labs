// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace lab9.JsonValidator;


public class JsonValidator
{
    public bool wasSuccess { get; private set; } = false;


    public void Validate(string schemaString, JToken json)
    {
        JSchemaUrlResolver resolver = new JSchemaUrlResolver();
        var path = GetThisFilePath();
        var directory = Directory.GetParent(path)!.Parent!;
        JSchema schema = JSchema.Parse(schemaString, new JSchemaReaderSettings
        {
            Resolver = resolver,
            BaseUri = new Uri(directory.ToString() + Path.DirectorySeparatorChar)
        });

        if (!json.IsValid(schema))
        {
            wasSuccess = false;
        }

        wasSuccess = true;
    }

    private static string GetThisFilePath([CallerFilePath] string path = null)
    {
        return path;
    }

}
