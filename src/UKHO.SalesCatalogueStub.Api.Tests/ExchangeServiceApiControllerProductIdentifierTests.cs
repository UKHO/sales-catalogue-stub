using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ExchangeServiceApiControllerProductIdentifierTests
    {
        private IProductEditionService _productRepo;
        private ExchangeServiceApiController _exchangeServiceApiController;

        [SetUp]
        public void Setup()
        {
            _productRepo = A.Fake<IProductEditionService>();
            _exchangeServiceApiController = new ExchangeServiceApiController(_productRepo);
        }

        [Test]
        public async Task Calling_PostProductIdentifiers_With_At_Least_One_Matching_Product_Should_Return_Status_Code_200()
        {
            var productResponse = new ProductResponse
            {
                ProductCounts = new ProductCounts
                {
                    RequestedProductCount = 0,
                    RequestedProductsAlreadyUpToDateCount = 0,
                    ReturnedProductCount = 2,
                    RequestedProductsNotReturned = new List<RequestedProductsNotReturned>()
                },
                Products = new Products
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
                }
            };

            var testData = new List<string>
            {
                "AU220120", "EG3GOA01"
            };
            A.CallTo(() => _productRepo.GetProductIdentifiers(testData)).Returns(productResponse);
            var response = await _exchangeServiceApiController.PostProductIdentifiers("AVCS", testData) as JsonResult;
            response?.StatusCode.Should().Be(200);
            response.Should().NotBeNull();
        }

        [Test]
        public async Task Calling_PostProductIdentifiers_With_A_Product_List_Should_Return_Expected_Json_Response()
        {
            var testData = new List<string>
            {
                "AU220120", "EG3GOA01"
            };

            var productResponse = new ProductResponse
            {
                ProductCounts = new ProductCounts
                {
                    RequestedProductCount = 0,
                    RequestedProductsAlreadyUpToDateCount = 0,
                    ReturnedProductCount = 2,
                    RequestedProductsNotReturned = new List<RequestedProductsNotReturned>()
                },
                Products = new Products
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
                }
            };

            A.CallTo(() => _productRepo.GetProductIdentifiers(testData)).Returns(productResponse);
            var response = await _exchangeServiceApiController.PostProductIdentifiers("AVCS", testData) as JsonResult;
            response?.Value.Should().BeOfType<ProductResponse>();
            response.Should().NotBeNull();
        }

        //[Test]
        //public async Task Calling_PostProductIdentifiers_With_No_Matching_Products_Should_Return_Status_Code_400()
        //{
        //    var dummyInput = A.CollectionOfDummy<string>(1).ToList();
        //    A.CallTo(() => _productRepo.GetProductIdentifiers(A<List<string>>.Ignored)).Returns(new Products());
        //    var response = await _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), dummyInput) as ObjectResult;
        //    response?.StatusCode.Should().Be(400);
        //    response.Should().NotBeNull();
        //}

        //[Test]
        //public async Task Calling_PostProductIdentifiers_With_List_Of_Null_Should_Return_Status_Code_400()
        //{
        //    var testData = new List<string>
        //    {
        //        null
        //    };
        //    A.CallTo(() => _productRepo.GetProductIdentifiers(testData)).Returns(new Products());
        //    var response = await _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), testData) as ObjectResult;
        //    response?.StatusCode.Should().Be(400);
        //    response.Should().NotBeNull();
        //}

        //[Test]
        //public void Calling_GetProductEditions_From_Controller_With_Null_Input_Should_Throw_ArgumentNullException()
        //{
        //    A.CallTo(() => _productRepo.GetProductIdentifiers(null)).Throws<ArgumentNullException>();
        //    _exchangeServiceApiController.Invoking(a => a.PostProductIdentifiers(A.Dummy<string>(), null)).Should().Throw<ArgumentNullException>();
        //}
    }
}