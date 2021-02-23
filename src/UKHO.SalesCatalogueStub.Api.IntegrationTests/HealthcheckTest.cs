using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.IntegrationTests.Configuration;
using UKHO.SalesCatalogueStub.Api.IntegrationTests.Helpers;

namespace UKHO.SalesCatalogueStub.Api.IntegrationTests
{
    public class HealthcheckTest
    {
        private IntegrationTestConfig _integrationTestConfig;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var keyVaultConfigRoot = AzureKeyVaultConfigConfigurationRoot.Instance;

            _integrationTestConfig = new IntegrationTestConfig();
            keyVaultConfigRoot.GetSection("IntegrationTest").Bind(_integrationTestConfig);
        }

        [Test]
        public async Task Test_HealthcheckTest()
        {
            var healthcheckUrl = new Uri($"{_integrationTestConfig.SiteBaseUrl}/health", UriKind.Absolute);
            var isSuccess = false;
            var returnedValue = "";

            using (var httpClient = new HttpClient())
            {

                var expectedResult = await httpClient.GetAsync(healthcheckUrl);

                isSuccess = expectedResult.IsSuccessStatusCode;
                returnedValue = await expectedResult.Content.ReadAsStringAsync();
            }

            Assert.IsTrue(isSuccess);
            Assert.AreEqual(returnedValue, "Healthy");
        }
    }
}