using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UKHO.SalesCatalogueStub.Api.IntegrationTests
{
    public class HealthcheckTest
    {
        [Test]
        public async Task Test_HealthcheckTest()
        {
            var healthcheckUrl = new Uri("https://m-salescatstub-api-qa-appservice.azurewebsites.net/health", UriKind.Absolute);
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