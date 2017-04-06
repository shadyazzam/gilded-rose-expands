using GildedRose.DataContracts;
using GildedRose.Entities;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Tests.Integration
{
	[TestClass]
	public class ShoppingTest// : IntegrationTest
	{
		TestDatabaseInitializer _testData;

		[TestInitialize]
		public void TestInit()
		{
			_testData = new TestDatabaseInitializer();
			Database.SetInitializer(_testData);
		}

		[TestCleanup]
		public void TestCleanup()
		{
            if (!string.IsNullOrWhiteSpace(_testData.DbName))
    			Database.Delete(_testData.DbName);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task GetAllItems_ShouldReturnAllItems()
		{
			using (var server = TestServer.Create<TestStartup>())
			{
				// Arrange

				// Act
				var response = await server.HttpClient.GetAsync("api/items");
				string json = await response.Content.ReadAsStringAsync();
				var items = JsonConvert.DeserializeObject<List<ItemDto>>(json);

				// Assert
				Assert.IsTrue(response.IsSuccessStatusCode, "Failed to get all items.");
				Assert.AreEqual(items.Count, _testData.Items.Count, "Unexpected number of items returned.");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task GetItem_WithValidId_ShouldReturnCorrectItem()
		{
			using (var server = TestServer.Create<TestStartup>())
			{
				// Arrange
				var itemToGet = _testData.Items.First();

				// Act
				var response = await server.HttpClient.GetAsync($"api/items/{itemToGet.Id}");
				string json = await response.Content.ReadAsStringAsync();
				var item = JsonConvert.DeserializeObject<ItemDto>(json);

				// Assert
				Assert.IsTrue(response.IsSuccessStatusCode, "Failed to get item.");
				Assert.AreEqual(item.Id, itemToGet.Id, "Incorrect item returned.");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task GetItem_WithInvalidId_ShouldReturnNotFound()
		{
			using (var server = TestServer.Create<TestStartup>())
			{
				// Arrange

				// Act
				var response = await server.HttpClient.GetAsync($"api/items/{Guid.NewGuid()}");
				string json = await response.Content.ReadAsStringAsync();
				var item = JsonConvert.DeserializeObject<ItemDto>(json);

				// Assert
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Did not fail to get item with invalid id.");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task GetAllItems_ForOfficeCategory_ShouldReturnAllOfficeItems()
		{
			using (var server = TestServer.Create<TestStartup>())
			{
				// Arrange

				// Act
				var response = await server.HttpClient.GetAsync($"api/categories/{_testData.OfficeCategoryId}/items");
				string json = await response.Content.ReadAsStringAsync();
				var items = JsonConvert.DeserializeObject<List<ItemDto>>(json);

				// Assert
				Assert.IsTrue(response.IsSuccessStatusCode, "Failed to get all items in a particular category.");

				var expectedCount = _testData.Items.Where(i => i.CategoryId == _testData.OfficeCategoryId).Count();

				Assert.AreEqual(items.Count, expectedCount, "Unexpected number of items returned for a particular category.");
			}
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task GetAllItems_ForInvalidCategory_ShouldReturnEmptyList()
		{
			using (var server = TestServer.Create<TestStartup>())
			{
				// Arrange

				// Act
				var response = await server.HttpClient.GetAsync($"api/categories/{Guid.NewGuid()}/items");
				string json = await response.Content.ReadAsStringAsync();
				var items = JsonConvert.DeserializeObject<List<ItemDto>>(json);

				// Assert
				Assert.IsTrue(response.IsSuccessStatusCode, "Failed to get empty item list for invalid category.");
				Assert.AreEqual(items.Count, 0, "Unexpected item count for invalid category.");
			}
		}
        
		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWhenLoggedIn_RequestPurchaseDetailsByValidIdWhenLoggedIn_ShouldSucceed()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));
            
                // Make the purchase

                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + itemToPurchase.Id + @""", ""Quantity"" : ""1"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                //var purchaseResponse = await server.HttpClient.PostAsync("api/purchases", purchaseRequestContent);
				string purchaseResponseJson = await purchaseResponse.Content.ReadAsStringAsync();
				var purchaseDetails = JsonConvert.DeserializeObject<PurchaseDto>(purchaseResponseJson);

				// Assert
				Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.Created, "Failed to make a purchase.");
				Assert.AreEqual(purchaseDetails.PurchasedItems.Count(), 1, "Unexpected number of items in purchase details.");
				Assert.AreEqual(purchaseDetails.PurchasedItems.First().Item.Id, itemToPurchase.Id, "Incorrect item in purchase details.");

                // Get the purchase details

                // Arrange

                // Act
                //var purchaseDetailsResponse = await server.HttpClient.GetAsync($"api/purchases/{purchaseDetails.Id}");
                message = new HttpRequestMessage(HttpMethod.Get, $"api/purchases/{purchaseDetails.Id}");
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseDetailsResponse = await server.HttpClient.SendAsync(message);

                string purchaseDetailsResponseJson = await purchaseDetailsResponse.Content.ReadAsStringAsync();
                var returnedPurchaseDetails = JsonConvert.DeserializeObject<PurchaseDto>(purchaseDetailsResponseJson);

                // Assert
                Assert.AreEqual(purchaseDetails, returnedPurchaseDetails, "Mismatch in the purchase details.");
            }
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWhenLoggedIn_RequestPurchaseDetailsByInvalidIdWhenLoggedIn_ShouldSucceedThenReturnBadRequest()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + itemToPurchase.Id + @""", ""Quantity"" : ""1"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                //var purchaseResponse = await server.HttpClient.PostAsync("api/purchases", purchaseRequestContent);
                string purchaseResponseJson = await purchaseResponse.Content.ReadAsStringAsync();
                var purchaseDetails = JsonConvert.DeserializeObject<PurchaseDto>(purchaseResponseJson);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.Created, "Failed to make a purchase.");
                Assert.AreEqual(purchaseDetails.PurchasedItems.Count(), 1, "Unexpected number of items in purchase details.");
                Assert.AreEqual(purchaseDetails.PurchasedItems.First().Item.Id, itemToPurchase.Id, "Incorrect item in purchase details.");

                // Get the purchase details

                // Arrange

                // Act
                //var purchaseDetailsResponse = await server.HttpClient.GetAsync($"api/purchases/{purchaseDetails.Id}");
                message = new HttpRequestMessage(HttpMethod.Get, $"api/purchases/{Guid.NewGuid()}");
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseDetailsResponse = await server.HttpClient.SendAsync(message);
                
                // Assert
                Assert.AreEqual(purchaseDetailsResponse.StatusCode, HttpStatusCode.NotFound, "Somehow found non-existent purchase details.");
            }
        }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWhenLoggedIn_RequestPurchaseDetailsWhenNotLoggedIn_ShouldSucceedThenReturnUnauthorized()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + itemToPurchase.Id + @""", ""Quantity"" : ""1"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                //var purchaseResponse = await server.HttpClient.PostAsync("api/purchases", purchaseRequestContent);
                string purchaseResponseJson = await purchaseResponse.Content.ReadAsStringAsync();
                var purchaseDetails = JsonConvert.DeserializeObject<PurchaseDto>(purchaseResponseJson);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.Created, "Failed to make a purchase.");
                Assert.AreEqual(purchaseDetails.PurchasedItems.Count(), 1, "Unexpected number of items in purchase details.");
                Assert.AreEqual(purchaseDetails.PurchasedItems.First().Item.Id, itemToPurchase.Id, "Incorrect item in purchase details.");

                // Get the purchase details

                // Arrange

                // Act
                server.HttpClient.DefaultRequestHeaders.Clear();
                var purchaseDetailsResponse = await server.HttpClient.GetAsync($"api/purchases/{purchaseDetails.Id}");
                
                // Assert
                Assert.AreEqual(purchaseDetailsResponse.StatusCode, HttpStatusCode.Unauthorized, "Did not fail to access purchase details when not logged in.");
            }
        }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWhenNotLoggedIn_ShouldReturnUnauthorized()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + itemToPurchase.Id + @""", ""Quantity"" : ""1"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}");

                // Act
                var purchaseResponse = await server.HttpClient.PostAsync("api/purchases", purchaseRequestContent);

                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.Unauthorized, "Did not fail to make an unauthorized purchase.");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task MakePurchaseWithNoItemsWhenLoggedIn_ShouldReturnBadRequest()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.BadRequest, "Did not fail to make purchase with no items.");
            }
        }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWithNonExistentItemsWhenLoggedIn_ShouldReturnBadRequest()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + Guid.NewGuid() + @""", ""Quantity"" : ""1"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.BadRequest, "Did not fail to make purchase with non-existent items.");
            }
        }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseForMoreStockThanAvailableWhenLoggedIn_ShouldReturnBadRequest()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{
					""Items"":[ { ""ItemId"" : """ + itemToPurchase.Id + @""", ""Quantity"" : ""100"" } ],
						""PaymentInfo"": {
							""card"":""visa"",
							""cardholdername"":""Shady Azzam"",
							""cardnumber"":""4532795275110686"",
							""expirymonth"":""8"",
							""expiryyear"":""2017"",
							""cardsecuritycode"":""456"",
							""billingaddress"":""my new address""
						}
					}", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.BadRequest, "Did not fail to make purchase for items with not enough stock.");
            }
        }

		[TestMethod]
		[TestCategory("Integration")]
		public async Task MakePurchaseWithMalformedRequestWhenLoggedIn_ShouldReturnBadRequest()
		{
            using (var server = TestServer.Create<TestStartup>())
            {
                // Register and login first
                // Arrange
                var userInfoContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", userInfoContent);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, "Failed to login.");

                string json = await tokenResponse.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                JToken value;
                Assert.IsTrue(jObject.TryGetValue("access_token", out value), "Failed to get token.");

                string token = (string)value.ToObject(typeof(string));

                // Make the purchase

                // Arrange
                var itemToPurchase = _testData.Items.First();

                var purchaseRequestContent = new StringContent(
                @"{	malformed }", Encoding.UTF8, "application/json");

                // Act
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/purchases")
                {
                    Content = purchaseRequestContent
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                server.HttpClient.DefaultRequestHeaders
                  .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var purchaseResponse = await server.HttpClient.SendAsync(message);

                // Assert
                Assert.AreEqual(purchaseResponse.StatusCode, HttpStatusCode.BadRequest, "Did not fail to make purchase with malformed request.");
            }
        }
	}
}
