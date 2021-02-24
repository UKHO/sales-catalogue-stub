using FakeItEasy;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.EF.Repositories;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ExchangeServiceApiControllerTests
    {
        [SetUp]
        public void Setup()
        {
            var productRepo = A.Fake<IProductEditionRepository>();
            var exchangeServiceApiController = new ExchangeServiceApiController(productRepo);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}