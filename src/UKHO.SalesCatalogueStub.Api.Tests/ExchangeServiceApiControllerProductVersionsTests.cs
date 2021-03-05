using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ExchangeServiceApiControllerProductVersionsTests
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
        public void Calling_PostProductVersions_With_No_Matching_Products_Should_Return_Status_Code_400()
        {
            var dummyInput = A.Dummy<ProductVersions>();
            A.CallTo(() => _productRepo.GetProductVersions(A<ProductVersions>.Ignored)).Returns((new Products(), GetProductVersionResponseEnum.NoProductsFound));
            var response = _exchangeServiceApiController.PostProductVersions(A.Dummy<string>(), dummyInput) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }

        [Test]
        public void Calling_PostProductVersions_With_No_Products_That_Have_Updates_Should_Return_Status_Code_304()
        {
            var dummyInput = A.Dummy<ProductVersions>();
            //A.CollectionOfDummy<ProductVersionsInner>(1).ToList();
            A.CallTo(() => _productRepo.GetProductVersions(A<ProductVersions>.Ignored)).Returns((new Products(), GetProductVersionResponseEnum.NoUpdatesFound));
            var response = _exchangeServiceApiController.PostProductVersions(A.Dummy<string>(), dummyInput) as ObjectResult;
            response?.StatusCode.Should().Be(304);
        }

        [Test]
        public void Calling_PostProductVersions_With_List_Of_Null_Should_Return_Status_Code_400()
        {
            var testData = new ProductVersions
            {
                null
            };
            A.CallTo(() => _productRepo.GetProductVersions(testData)).Returns((new Products(), GetProductVersionResponseEnum.NoProductsFound));
            var response = _exchangeServiceApiController.PostProductVersions(A.Dummy<string>(), testData) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }

        [Test]
        public void Calling_PostProductVersions_With_At_Least_One_Matching_Product_Should_Return_Status_Code_200()
        {
            var productVersions = A.Dummy<ProductVersions>();
            A.CallTo(() => _productRepo.GetProductVersions(productVersions)).Returns((new Products
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
            }, GetProductVersionResponseEnum.UpdatesFound));
            var response = _exchangeServiceApiController.PostProductVersions(A.Dummy<string>(), productVersions) as ObjectResult;
            response?.StatusCode.Should().Be(200);
        }

        [Test]
        public void Calling_PostProductVersions_With_A_Product_List_Should_Return_Expected_Json_Response()
        {
            var productVersions = A.Dummy<ProductVersions>();

            var testResponse = new Products
            {
                new ProductsInner
                {
                    EditionNumber = 1, FileSize = 100, ProductName = "AU220120",
                    UpdateNumbers = new List<int?> {1, 2, 3}
                },
                new ProductsInner
                {
                    EditionNumber = 1, FileSize = 100, ProductName = "EG3GOA01",
                    UpdateNumbers = new List<int?> {1, 2, 3}
                }
            };

            const string expectedJson = "[\r\n  {\r\n    \"productName\": \"AU220120\",\r\n    \"editionNumber\": 1,\r\n    \"updateNumbers\": [\r\n      1,\r\n      2,\r\n      3\r\n    ],\r\n    \"fileSize\": 100\r\n  },\r\n  {\r\n    \"productName\": \"EG3GOA01\",\r\n    \"editionNumber\": 1,\r\n    \"updateNumbers\": [\r\n      1,\r\n      2,\r\n      3\r\n    ],\r\n    \"fileSize\": 100\r\n  }\r\n]";
            A.CallTo(() => _productRepo.GetProductVersions(productVersions)).Returns((testResponse, GetProductVersionResponseEnum.UpdatesFound));
            var response = _exchangeServiceApiController.PostProductVersions(A.Dummy<string>(), productVersions) as ObjectResult;
            response?.Value.Should().BeEquivalentTo(expectedJson);
        }
    }
}
