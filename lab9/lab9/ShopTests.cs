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
    public async Task Test_GetProductsWithNotEmptyArray()
    {
        //getting all products with api method
        var response = await _api.GetProductsAsync();

        //assert response
        _jsonValidator.Validate(_productsListSchemaJson, response);
        Assert.IsTrue(response.Count > 0,
            "Expected not empty array of products, got count equals to 0");
    }

    [TestMethod]
    public async Task Test_GetProductsWithAppropriateJsonSchema()
    {
        //getting all products with api method
        var products = await _api.GetProductsAsync();

        //validate result
        _jsonValidator.Validate(_productsListSchemaJson, products);
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected array of products, got data that doesn't match products list schema");
    }

    [TestMethod]
    public async Task Test_AddValidProduct()
    {
        //add product
        var product = _addProductTestsJson["valid"]!;
        var response = await _api.AddProductAsync(product.ToObject<Product>()!);

        //check response and get added product with schema validation
        _jsonValidator.Validate(_addProductResponseSchemaJson, response);
        Assert.IsTrue(_jsonValidator.wasSuccess);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(actualProduct,
            "Expected to find recently created valid product in product list");
        _jsonValidator.Validate(_productSchemaJson, actualProduct);
        Assert.IsTrue(_jsonValidator.wasSuccess,
            "Expected to get same json schema as product has");
        Assert.That.IsSameProduct(product, actualProduct);
    }

    //FIXED: параметризировать тесты для работы с разными невалидными продуктами
    [DataRow("invalidByCategoryIdLess")]
    [DataRow("invalidByCategoryIdMore")]
    [TestMethod]
    public async Task Test_AddInvalidProductWithWrongCategoryId(string testCaseName)
    {
        //try adding product
        var result = await _api.AddProductAsync(_addProductTestsJson[testCaseName]!.ToObject<Product>()!);

        //make sure that there is no product with response id
        var productId = result["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently created invalid product in product list");
    }

    [TestMethod]
    public async Task Test_AddSeveralValidProductWithSameTitle()
    {
        var expectedAliasFirst = "premium-chasy1";
        var expectedAliasSecond = "premium-chasy1-0";
        var expectedAliasThird = "premium-chasy1-0-0";
        var product = _addProductTestsJson["validWithWatchTitle"]!;

        //add products
        var responseFirst = await _api.AddProductAsync(product.ToObject<Product>()!);
        var responseSecond = await _api.AddProductAsync(product.ToObject<Product>()!);
        var responseThird = await _api.AddProductAsync(product.ToObject<Product>()!);

        //retrieve results and check aliases
        var productIdFirst = responseFirst["id"]!.ToObject<int>();
        var productIdSecond = responseSecond["id"]!.ToObject<int>();
        var productIdThird = responseThird["id"]!.ToObject<int>();
        _testProductsIds.Add(productIdFirst);
        _testProductsIds.Add(productIdSecond);
        _testProductsIds.Add(productIdThird);
        var products = await _api.GetProductsAsync();

        var actualProductFirst = Product.GetProductById(productIdFirst, products)!;
        var actualProductSecond = Product.GetProductById(productIdSecond, products)!;
        var actualProductThird = Product.GetProductById(productIdThird, products)!;

        Assert.That.IsSameProduct(product, actualProductFirst);
        Assert.That.IsSameProduct(product, actualProductSecond);
        Assert.That.IsSameProduct(product, actualProductThird);

        Assert.AreEqual(expectedAliasFirst, actualProductFirst["alias"],
            "Expected to get alias without -0 postfix");
        Assert.AreEqual(expectedAliasSecond, actualProductSecond["alias"],
            "Expected to get alias with signle -0 postfix");
        Assert.AreEqual(expectedAliasThird, actualProductThird["alias"],
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
        var expectedStatus = 1;
        var expectedProduct = _editProductTestsJson["validWithNotExistingId"]!;
        var expectedAlias = expectedProduct["title"]!.ToString().ToLower();

        //edit product
        var response = await _api.EditProductAsync(expectedProduct.ToObject<Product>()!);

        //check response status and compare products
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
        var expectedStatus = 0;

        //edit product
        var response = await _api.EditProductAsync(_editProductTestsJson["validWithoutId"]!.ToObject<Product>()!);

        //check response status
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

        //edit with same data, especially with the same title at first time will ad -id postfix to the alias
        await _api.EditProductAsync(editProduct.ToObject<Product>()!);

        var actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterFirstEdit, actualProduct["alias"]);

        //edit with same data, especially with the same title twice in a row will remove -id postfix from the alias
        await _api.EditProductAsync(editProduct.ToObject<Product>()!);
        actualProduct = Product.GetProductById(productId, await _api.GetProductsAsync());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterSecondEdit, actualProduct["alias"]);
    }

    [TestMethod]
    public async Task Test_DeleteExistingProduct()
    {
        var expectedStatus = 1;
        var response = await _api.AddProductAsync(_addProductTestsJson["validProductToFurtherEdit"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);

        //delete
        response = await _api.DeleteProductAsync(productId);

        //make sure that product was deleted
        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
            "Expected to get 0 status after trying editing product without specified id");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find recently deleted product in product list");
    }

    [TestMethod]
    public async Task Test_DeleteNotExistingProduct()
    {
        var expectedStatus = 0;
        var productId = 77777;

        //try to delete
        var result = await _api.DeleteProductAsync(productId);

        //make sure that there is no new product in product list
        Assert.AreEqual(expectedStatus, result["status"]!.ToObject<int>(),
            "Expected to get 0 status after trying deleting not existing product");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProductsAsync()),
            "Expected to not find non-existent in product list");
    }
}
