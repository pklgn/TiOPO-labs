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
    private static string _productSchemaJson;
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
        _productSchemaJson = File.ReadAllText(@"..\..\..\JsonSchemas\productSchema.json");
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
        //getting all products with api method
        var response = await _api.GetProductsAsync();

        //assert response
        _jsonValidator.Validate(_productsListSchemaJson, response);
        Assert.IsTrue(response.Count > 0,
            "Expected not empty array of products, got count equals to 0");
    }

    [TestMethod]
    public async Task Test_GetProductsMatchesJsonSchema()
    {
        //getting all products with api method
        var products = await _api.GetProductsAsync();

        //assert
        _jsonValidator.Validate(_productsListSchemaJson, products);
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected array of products, got data that doesn't match products list schema");
    }

    [TestMethod]
    public async Task Test_AddValidProduct()
    {
        //act
        var response = await _api.AddProductAsync(_addProductTestsJson["valid"]!.ToObject<Product>()!);

        //assert
        _jsonValidator.Validate(_addProductResponseSchemaJson, response);
        Assert.IsTrue(_jsonValidator.wasSuccess);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var product = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(product,
            "Expected to find recently created valid product in product list");
        _jsonValidator.Validate(_productSchemaJson, product);
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected to get same json schema as product has");
    }

    //FIXED: параметризировать тесты для работы с разными невалидными продуктами
    [DataRow("invalidByCategoryIdLess")]
    [DataRow("invalidByCategoryIdMore")]
    [TestMethod]
    public async Task Test_AddInvalidProductWithCategoryId(string testCaseName)
    {
        //arrange

        //act
        var result = await _api.AddProductAsync(_addProductTestsJson[testCaseName]!.ToObject<Product>()!);

        //assert
        var productId = result["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently created invalid product in product list");
    }

    [TestMethod]
    public async Task Test_AddSeveralValidProductWithSameTitle()
    {
        //arrange
        var expectedAliasFirst = "premium-chasy1";
        var expectedAliasSecond = "premium-chasy1-0";
        var expectedAliasThird = "premium-chasy1-0-0";

        //add products
        var responseFirst = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);
        var responseSecond = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);
        var responseThird = await _api.AddProductAsync(_addProductTestsJson["validWithWatchTitle"]!.ToObject<Product>()!);

        //retrieve results and assert
        var productIdFirst = responseFirst["id"]!.ToObject<int>();
        var productIdSecond = responseSecond["id"]!.ToObject<int>();
        var productIdThird = responseThird["id"]!.ToObject<int>();
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
    //FIXED: перенести cleanup наверх

    [TestMethod]
    public async Task Test_EditExistingProduct()
    {
        //add product to further edit
        var response = await _api.AddProductAsync(_addProductTestsJson["validProductToFurtherEdit"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var editProduct = _editProductTestsJson["valid"]!;
        editProduct["id"] = productId;
        var expectedAlias = editProduct["title"]!.ToString().ToLower();

        //edit
        var result = await _api.EditProductAsync(editProduct.ToObject<Product>()!);

        //assert result
        //FIXED: вынести проверку для всех полей в отдельный метод
        var actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync())!;
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAlias, actualProduct["alias"],
            "Expected to get alias as lower version of title");
    }

    [TestMethod]
    public async Task Test_EditNotExistingProduct()
    {
        //arrange
        var expectedStatus = 1;
        var expectedProduct = _editProductTestsJson["validWithNotExistingId"]!;
        var expectedAlias = expectedProduct["title"]!.ToString().ToLower();

        //act
        var response = await _api.EditProductAsync(expectedProduct.ToObject<Product>()!);

        //assert
        var actualStatus = response["status"]!;
        var products = await _api.GetProductsAsync();
        var editedProduct = products.Last();
        var editedProductId = editedProduct["id"]!.ToObject<int>();
        _testProductsIds.Add(editedProductId);

        Assert.That.IsSameProduct(expectedProduct, editedProduct);
        Assert.AreEqual(expectedAlias, editedProduct["alias"],
            "Expected to get alias as lower version of title");
        Assert.AreEqual(expectedStatus, actualStatus,
            "Expected to receive successfull status code after edit");
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
            "Expected to get unsuccessfull status after trying editing product without specified id");
    }

    [TestMethod]
    public async Task Test_EditProductWithSameTitleTwice()
    {
        var editProduct = _addProductTestsJson["validProductToFurtherEdit"]!;
        var response = await _api.AddProductAsync(editProduct.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        var expectedAliasAfterFirstEdit = $"{editProduct["title"]!.ToString().ToLower()}-{productId}";
        var expectedAliasAfterSecondEdit = editProduct["title"]!.ToString().ToLower();
        editProduct["id"] = productId;
        _testProductsIds.Add(productId);

        //edit with same data, especially with the same title
        await _api.EditProductAsync(editProduct.ToObject<Product>()!);

        var actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterFirstEdit, actualProduct["alias"]);

        //edit with same data, especially with the same title twice
        await _api.EditProductAsync(editProduct.ToObject<Product>()!);
        actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterSecondEdit, actualProduct["alias"]);
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
