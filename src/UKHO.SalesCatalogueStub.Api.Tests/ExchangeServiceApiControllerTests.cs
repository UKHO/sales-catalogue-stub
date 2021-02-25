using FakeItEasy;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ExchangeServiceApiControllerTests
    {
        [SetUp]
        public void Setup()
        {
            var productRepo = A.Fake<IProductEditionService>();
            var exchangeServiceApiController = new ExchangeServiceApiController(productRepo);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}