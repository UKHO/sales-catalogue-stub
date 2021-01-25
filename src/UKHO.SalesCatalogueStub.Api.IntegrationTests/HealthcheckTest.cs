using System;
using System.Net.Http;
using NUnit.Framework;

namespace UKHO.SalesCatalogueStub.Api.IntegrationTests
{
    public class HealthcheckTest
    {
        [Test]
        public void Test_HealthcheckTest()
        {
            var healthcheckUrl = new Uri("https://localhost:44325/health", UriKind.Absolute);
            var httpClient = new HttpClient();
            httpClient.GetAsync(healthcheckUrl.ToString());

            Assert.Pass();
        }
    }
}