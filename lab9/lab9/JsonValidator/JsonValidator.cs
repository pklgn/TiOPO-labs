// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
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
        JSchema schema = JSchema.Parse(schemaString);

        if (!json.IsValid(schema))
        {
            wasSuccess = false;
        }

        wasSuccess = true;
    }
}
