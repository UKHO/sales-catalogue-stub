using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.IntegrationTests.Configuration;
using UKHO.SalesCatalogueStub.Api.IntegrationTests.Helpers;

namespace UKHO.SalesCatalogueStub.Api.IntegrationTests
{
    public class AuthTest
    {
        private IntegrationTestConfig _integrationTestConfig;
        private AppRegistrationConfig _appRegistrationConfig;
        private AppRegistrationClientConfig _appRegistrationClientConfig;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var keyVaultConfigRoot = AzureKeyVaultConfigConfigurationRoot.Instance;

            _integrationTestConfig = new IntegrationTestConfig();
            keyVaultConfigRoot.GetSection("IntegrationTest").Bind(_integrationTestConfig);

            _appRegistrationConfig = new AppRegistrationConfig();
            keyVaultConfigRoot.GetSection("AppRegistration").Bind(_appRegistrationConfig);

            _appRegistrationClientConfig = new AppRegistrationClientConfig();
            keyVaultConfigRoot.GetSection("AppRegistrationClient").Bind(_appRegistrationClientConfig);
        }

        [Test]
        public async Task Test_Given_Valid_Credentials_When_Catalogue_Resource_Is_Requested_Then_Authorization_Is_Successful_And_Status_Code_Is_Successful()
        {
            var catalogueUrl = new Uri($"{_integrationTestConfig.SiteBaseUrl}/v1/productData/productType/catalogue/catalogueType", UriKind.Absolute);

            using (var httpClient = new HttpClient())
            {
                var credential = new ClientSecretCredential(_appRegistrationClientConfig.TenantId, _appRegistrationClientConfig.ClientId, _appRegistrationClientConfig.ClientSecret);

                var tokenRequest = new TokenRequestContext(new[] { $"{_appRegistrationConfig.ClientId}/.default" });

                var token = await credential.GetTokenAsync(tokenRequest, CancellationToken.None);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                var result = await httpClient.GetAsync(catalogueUrl);

                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }

        [Test]
        public async Task Test_Given_Valid_Credentials_When_ExchangeService_Resource_Is_Requested_Then_Authorization_Is_Successful_And_Status_Code_Is_Successful()
        {
            var exchangeService = new Uri($"{_integrationTestConfig.SiteBaseUrl}/v1/productData/productType/products/productIdentifiers", UriKind.Absolute);

            using (var httpClient = new HttpClient())
            {
                var credential = new ClientSecretCredential(_appRegistrationClientConfig.TenantId, _appRegistrationClientConfig.ClientId, _appRegistrationClientConfig.ClientSecret);

                var tokenRequest = new TokenRequestContext(new[] { $"{_appRegistrationConfig.ClientId}/.default" });

                var token = await credential.GetTokenAsync(tokenRequest, CancellationToken.None);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                var result = await httpClient.PostAsync(exchangeService, new StringContent("[\"string\"]", Encoding.UTF8, MediaTypeNames.Application.Json));

                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }

        [Test]
        public async Task Test_Given_Invalid_Token_When_Catalogue_Resource_Is_Requested_Then_Authorization_Is_Unsuccessful_And_Status_Code_Is_Unauthorized()
        {
            var catalogueUrl = new Uri($"{_integrationTestConfig.SiteBaseUrl}/v1/productData/productType/catalogue/catalogueType", UriKind.Absolute);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "INVALID_TOKEN");

                var result = await httpClient.GetAsync(catalogueUrl);

                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Test]
        public async Task Test_Given_Invalid_Token_When_ExchangeService_Resource_Is_Requested_Then_Authorization_Is_Unsuccessful_And_Status_Code_Is_Unauthorized()
        {
            var exchangeService = new Uri($"{_integrationTestConfig.SiteBaseUrl}/v1/productData/productType/products/productIdentifiers", UriKind.Absolute);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "INVALID_TOKEN");

                var result = await httpClient.PostAsync(exchangeService, new StringContent("[\"string\"]", Encoding.UTF8, MediaTypeNames.Application.Json));

                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }
    }
}