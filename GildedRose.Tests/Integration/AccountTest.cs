using GildedRose.Models;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Tests.Integration
{
    [TestClass]
    public class AccountTest
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task RegisterUserAndLogin_ShouldSucceed()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
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
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task RegisterUserWithWeakPassword_ShouldReturnBadRequest()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                // Arrange
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "weak"),
                    new KeyValuePair<string, string>("confirmpassword", "weak")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", content);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Did not fail creating user with weak password.");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task RegisterExistingEmail_ShouldReturnBadRequest()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                // Arrange
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register user
                var response = await server.HttpClient.PostAsync("api/account/register", content);

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode, "Failed to register user.");

                // Arrange
                var content2 = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", "johnDoe@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1"),
                    new KeyValuePair<string, string>("confirmpassword", "!Password1")
                });

                // Act - register the same email
                response = await server.HttpClient.PostAsync("api/account/register", content2);

                // Assert
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Did not fail to register existing email.");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task LoginWithInvalidCredentials_ShouldReturnBadRequest()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                // Arrange
                var credContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "idontexist@gmail.com"),
                    new KeyValuePair<string, string>("password", "!Password1")
                });

                // Act - login (request token)
                var tokenResponse = await server.HttpClient.PostAsync("token", credContent);

                // Assert
                Assert.AreEqual(tokenResponse.StatusCode, HttpStatusCode.BadRequest, "Did not fail to login with invalid credentials.");
            }
        }
    }
}
