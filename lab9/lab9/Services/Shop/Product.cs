// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json.Linq;


namespace Shop.Services.Shop;

using Id = System.Int32;
using Flag = System.Byte;
using Money = System.Decimal;

public class Product
{
    private static Id NO_ID = -1;
    private static Flag NO_FLAG = System.Byte.MaxValue;
    private static Money NO_MONEY = -1;

    public Id? id { get; set; }
    public Id category_id { get; set; } = NO_ID;
    public string title { get; set; }
    public string? alias { get; set; }
    public string content { get; set; }
    public Money price { get; set; } = NO_MONEY;
    public Money old_price { get; set; } = NO_MONEY;
    public Flag status { get; set; } = NO_FLAG;
    public string keywords { get; set; }
    public string description { get; set; }
    public Flag hit { get; set; } = NO_FLAG;

    public static JToken? GetProductById(int productId, JArray products)
    {
        foreach (var product in products)
        {
            if (product["id"]!.ToObject<int>() == productId)
            {
                return product;
            }
        }

        return null;
    }
}
