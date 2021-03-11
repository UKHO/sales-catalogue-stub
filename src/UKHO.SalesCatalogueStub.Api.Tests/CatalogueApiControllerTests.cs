using FakeItEasy;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class CatalogueApiControllerTests
    {
        private IProductEditionService _productRepo;
        private CatalogueApiController _catalogueApiController;

        [SetUp]
        public void Setup()
        {
            _productRepo = A.Fake<IProductEditionService>();
            _catalogueApiController = new CatalogueApiController(_productRepo);
        }

        /*[Test]
        public async Task Calling_GetProducts_With_At_Least_One_Product_Returned_From_Service_Should_Return_Status_Code_200()
        {
            A.CallTo(() => _productRepo.GetProductEditionsSinceDateTime(A<DateTime>.Ignored)).Returns(new Products
            {
                new ProductsInner()
                {
                    EditionNumber = 1, FileSize = 100, ProductName = "AU220120",
                    UpdateNumbers = new List<int?> {1, 2, 3}
                },
                new ProductsInner()
                {
                    EditionNumber = 1, FileSize = 100, ProductName = "EG3GOA01",
                    UpdateNumbers = new List<int?> {1, 2, 3}
                }
            });
            var response = await _exchangeServiceApiController.GetProducts("AVCS", A.Dummy<DateTime>()) as ObjectResult;
            response?.StatusCode.Should().Be(200);
        }*/


    }
}