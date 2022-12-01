// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using lab9.Tools.Http;
using lab9.JsonValidator;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Shop.Services.Shop;

public class ShopAPI
{
    private HttpClient _httpClient;
    private Uri _baseUri;

    private struct ApiMethodsUri
    {
        public static string GetProducts = "api/products";
        public static string DeleteProduct = "api/deleteproduct";
        public static string AddProduct = "api/addproduct";
        public static string EditProduct = "api/editproduct";

        public ApiMethodsUri()
        {
        }
    }

    public ShopAPI(Uri baseUri, HttpClient httpClient)
    {
        _baseUri = baseUri;
        _httpClient = httpClient;
    }

    public async Task<JArray> GetProductsAsync()
    {
        var requestUri = new Uri(_baseUri, ApiMethodsUri.GetProducts);
        var response = await _httpClient.GetAsync(requestUri);

        return (JArray)await GetJsonFromResponse(response);
    }

    public async Task<JObject> DeleteProductAsync(int id)
    {
        var requestUri = new Uri(_baseUri, ApiMethodsUri.DeleteProduct);
        requestUri = UriExtension.AddParameter(requestUri, nameof(Product.id), id.ToString());
        var response = await _httpClient.GetAsync(requestUri);

        return (JObject)await GetJsonFromResponse(response);
    }

    public async Task<JObject> AddProductAsync(Product product)
    {
        var requestUri = new Uri(_baseUri, ApiMethodsUri.AddProduct);
        var response = await HttpPostProductAsync(requestUri, product);

        return (JObject)await GetJsonFromResponse(response);
    }

    public async Task<JObject> EditProductAsync(Product product)
    {
        var requestUri = new Uri(_baseUri, ApiMethodsUri.EditProduct);
        var response = await HttpPostProductAsync(requestUri, product);

        return (JObject)await GetJsonFromResponse(response);
    }

    private async Task<HttpResponseMessage> HttpPostProductAsync(Uri requestUri, Product product)
    {
        var json = JsonConvert.SerializeObject(product);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(requestUri, data);
    }

    private async Task<JToken> GetJsonFromResponse(HttpResponseMessage response)
    {
        var test = await response.Content.ReadAsStringAsync();
        var jsonContent = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

        var jsonContentString = jsonContent!.ToString();
        if (jsonContentString is null)
        {
            throw new Exception("Error was found while handling the response");
        }

        var result = JToken.Parse(jsonContentString);

        return result;
    }
}
