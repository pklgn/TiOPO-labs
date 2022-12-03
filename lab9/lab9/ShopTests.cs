// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Shop.Services.Shop;

namespace lab9;


internal static class CustomProductAssert
{
    public static void IsSameProduct(this Assert assert, JToken expected, JToken actual)
    {
        Assert.AreEqual(actual["alias"], actual["alias"],
            "Expected to get same alias after editing");
        Assert.AreEqual(expected["title"], actual["title"],
            "Expected to get same title as edit json has");
        Assert.AreEqual(expected["price"], actual["price"],
            "Expected to get same price as edit json has");
        Assert.AreEqual(expected["old_price"], actual["old_price"],
            "Expected to get same old_price as edit json has");
        Assert.AreEqual(expected["status"], actual["status"],
            "Expected to get same status as edit json has");
        Assert.AreEqual(expected["keyword"], actual["keyword"],
            "Expected to get same keyword as edit json has");
        Assert.AreEqual(expected["description"], actual["description"],
            "Expected to get same description as edit json has");
        Assert.AreEqual(expected["hit"], actual["hit"],
            "Expected to get same hit as edit json has");
    }
}


[TestClass]
public class ShopApiTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static Uri _baseUri;
    private static HttpClient _httpClient;
    private static ShopAPI _api;
    private static JsonValidator.JsonValidator _jsonValidator;
    private static string _productsListSchemaJson;
    private static string _addProductResponseSchemaJson;
    private static List<int> _testProductsIds = new();
    private static JObject _addProductTestsJson;
    private static JObject _editProductTestsJson;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    [ClassInitialize]
    public static void Initialize(TestContext testContext)
    {
        _baseUri = new("http://shop.qatl.ru/");
        _httpClient = new();
        _api = new ShopAPI(_baseUri, _httpClient);
        _jsonValidator = new();
        _productsListSchemaJson = File.ReadAllText(@"..\..\..\JsonSchemas\productsListSchema.json");
        _addProductResponseSchemaJson = File.ReadAllText(@"..\..\..\JsonSchemas\addProductResponseSchema.json");
        _addProductTestsJson = JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\addProductTests.json"));
        _editProductTestsJson = JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\editProductTests.json"));
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        Debug.Print("Running TestCleanup");
        foreach (var id in _testProductsIds)
        {
            await _api.DeleteProductAsync(id);
        }
        _testProductsIds.Clear();
    }

    [TestMethod]
    public async Task Test_GetProductsReturnsNotEmptyArray()
    {
        //arrange

        //act
        var result = await _api.GetProductsAsync();

        //assert
        _jsonValidator.Validate(_productsListSchemaJson, result);
        Assert.IsTrue(result.Count > 0,
            "Expected not empty array of products, got count equals to 0");
    }

    [TestMethod]
    public async Task Test_GetProductsMatchesJsonSchema()
    {
        //arrange
        //TODO: схема act + assert

        //act
        var products = await _api.GetProductsAsync();

        //assert
        _jsonValidator.Validate(_productsListSchemaJson, products);
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected array of products, got data that doesn't match products list schema");
    }

    [TestMethod]
    public async Task Test_AddValidProduct()
    {
        //arrange

        //act
        var response = await _api.AddProductAsync(_addProductTestsJson["valid"]!.ToObject<Product>()!);

        //assert
        var productId = response["id"]!.ToObject<int>();
        _jsonValidator.Validate(_addProductResponseSchemaJson, response);
        _testProductsIds.Add(response["id"]!.ToObject<int>());
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected success status code when adding valid product");
        Assert.IsNotNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to find recently created valid product in product list");
    }

    [TestMethod]
    public async Task Test_AddInvalidProductWithCategoryIdLess()
    {
        //arrange

        //act
        var result = await _api.AddProductAsync(_addProductTestsJson["invalidByCategoryIdLess"]!.ToObject<Product>()!);

        //assert
        var productId = result["id"]!.ToObject<int>();
        _jsonValidator.Validate(_addProductResponseSchemaJson, result);
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently created invalid product in product list");
    }

    //TODO: параметризировать тесты для работы с разными невалидными продуктами
    [TestMethod]
    public async Task Test_AddInvalidProductWithCategoryIdMore()
    {
        //arrange

        //act
        var result = await _api.AddProductAsync(_addProductTestsJson["invalidByCategoryIdMore"]!.ToObject<Product>()!);

        //assert
        var productId = result["id"]!.ToObject<int>();
        _jsonValidator.Validate(_addProductResponseSchemaJson, result);
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently created invalid product in product list");
    }

    [TestMethod]
    public async Task Test_AddValidProductWithRussianTitle()
    {
        //arrange
        var expectedAlias = "taytl";

        //act
        var result = await _api.AddProductAsync(_addProductTestsJson["validWithRussianTitle"]!.ToObject<Product>()!);

        //assert
        var productId = result["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        Assert.AreEqual(expectedAlias, Product.GetProductById(productId, await _api.GetProductsAsync())!["alias"],
            "Expected to get right translit version of russian title");
    }

    [TestMethod]
    public async Task Test_AddSeveralValidProductWithSameTitle()
    {
        //arrange
        var expectedAliasFirst = "premium-chasy1";
        var expectedAliasSecond = "premium-chasy1-0";
        var expectedAliasThird = "premium-chasy1-0-0";

        //act
        var resultFirst = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);
        var resultSecond = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);
        var resultThird = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);

        //assert
        var productIdFirst = resultFirst["id"]!.ToObject<int>();
        var productIdSecond = resultSecond["id"]!.ToObject<int>();
        var productIdThird = resultThird["id"]!.ToObject<int>();
        _testProductsIds.Add(productIdFirst);
        _testProductsIds.Add(productIdSecond);
        _testProductsIds.Add(productIdThird);
        var products = await _api.GetProductsAsync();

        Assert.AreEqual(expectedAliasFirst, Product.GetProductById(productIdFirst, products)!["alias"],
            "Expected to get alias without -0 postfix");
        Assert.AreEqual(expectedAliasSecond, Product.GetProductById(productIdSecond, products)!["alias"],
            "Expected to get alias with signle -0 postfix");
        Assert.AreEqual(expectedAliasThird, Product.GetProductById(productIdThird, products)!["alias"],
            "Expected to get alias with double -0-0 postfix");
    }
    //TODO: перенсти cleanup наверх

    [TestMethod]
    public async Task Test_EditExistingProduct()
    {
        //arrange
        var response = await _api.AddProductAsync(_addProductTestsJson["validProductToFurtherEdit"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var editProduct = _editProductTestsJson["valid"]!;
        editProduct["id"] = productId;

        //act
        var result = await _api.EditProductAsync(_editProductTestsJson["valid"]!.ToObject<Product>()!);

        //assert
        //TODO: вынести проверку для всех полей в отдельный метод
        var actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync())!;
        Assert.That.IsSameProduct(editProduct, actualProduct);
    }

    [TestMethod]
    public async Task Test_EditNotExistingProduct()
    {
        //arrange
        var expectedStatus = 1;
        var expectedProduct = _editProductTestsJson["validWithNotExistingId"]!;

        //act
        var response = await _api.EditProductAsync(_editProductTestsJson["validWithNotExistingId"]!.ToObject<Product>()!);

        //assert
        var status = response["status"]!;
        var products = await _api.GetProductsAsync();
        var editedProduct = products.Last();
        var editedProductId = editedProduct["id"]!.ToObject<int>();
        expectedProduct["id"] = editedProductId;
        _testProductsIds.Add(editedProductId);
        //Assert.AreEqual(expectedAlias, Product.GetProductById(editedProductId, products)!["alias"],
        //    "Expected to get alias with foreign id after editing");
        Assert.That.IsSameProduct(expectedProduct, editedProduct);
        Assert.AreEqual(expectedStatus, status);
    }

    [TestMethod]
    public async Task Test_EditProductWithoutId()
    {
        //arrange
        var expectedStatus = 0;

        //act
        var response = await _api.EditProductAsync(_editProductTestsJson["validWithoutId"]!.ToObject<Product>()!);

        //assert
        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
            "Expected to get 0 status after trying editing product without specified id");
    }

    [TestMethod]
    public async Task Test_DeleteExistingProduct()
    {
        //arrange
        var expectedStatus = 1;
        var response = await _api.AddProductAsync(_addProductTestsJson["validProductToFurtherEdit"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);

        //act
        response = await _api.DeleteProductAsync(productId);

        //assert
        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
            "Expected to get 0 status after trying editing product without specified id");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently deleted product in product list");
    }

    [TestMethod]
    public async Task Test_DeleteNotExistingProduct()
    {
        //arrange
        var expectedStatus = 0;
        var productId = 77777;

        //act
        var result = await _api.DeleteProductAsync(productId);

        //assert
        Assert.AreEqual(expectedStatus, result["status"]!.ToObject<int>(),
            "Expected to get 0 status after trying deleting not existing product");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find non-existent in product list");
    }
}
