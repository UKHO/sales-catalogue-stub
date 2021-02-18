using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task Test_AuthTest()
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
    }
}