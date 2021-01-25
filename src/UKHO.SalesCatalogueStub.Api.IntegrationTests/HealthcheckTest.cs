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
            var healthcheckUrl = new Uri("https://localhost:44325/health", UriKind.Absolute);
            var isSuccess = false;
            var returnedValue = "";

            using (var httpClient = new HttpClient())
            {

                var expectedResult = await httpClient.GetAsync(healthcheckUrl.ToString());

                isSuccess = expectedResult.IsSuccessStatusCode;
                returnedValue = await expectedResult.Content.ReadAsStringAsync();
            }

            Assert.IsTrue(isSuccess);
            Assert.AreEqual(returnedValue, "Healthy");
        }
    }
}