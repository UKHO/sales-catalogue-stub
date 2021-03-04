using System;
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
    public class ExchangeServiceApiControllerGetProductsTests
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
        public void Calling_GetProducts_With_At_Least_One_Product_Returned_From_Service_Should_Return_Status_Code_200()
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
            var response = _exchangeServiceApiController.GetProducts("AVCS", A.Dummy<DateTime>()) as ObjectResult;
            response?.StatusCode.Should().Be(200);
        }

        [Test]
        public void Calling_GetProducts_With_At_Least_One_Product_Returned_From_Service_Should_Return_Expected_Json_Response()
        {
            var testResponse = new Products
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
            };
            A.CallTo(() => _productRepo.GetProductEditionsSinceDateTime(A<DateTime>.Ignored)).Returns(testResponse);
            const string expectedJson = "[\r\n  {\r\n    \"productName\": \"AU220120\",\r\n    \"editionNumber\": 1,\r\n    \"updateNumbers\": [\r\n      1,\r\n      2,\r\n      3\r\n    ],\r\n    \"fileSize\": 100\r\n  },\r\n  {\r\n    \"productName\": \"EG3GOA01\",\r\n    \"editionNumber\": 1,\r\n    \"updateNumbers\": [\r\n      1,\r\n      2,\r\n      3\r\n    ],\r\n    \"fileSize\": 100\r\n  }\r\n]";
            var response = _exchangeServiceApiController.GetProducts("AVCS", A.Dummy<DateTime>()) as ObjectResult;
            response?.Value.Should().BeEquivalentTo(expectedJson);
        }

        [Test]
        public void Calling_GetProducts_With_No_Product_Returned_From_Service_Should_Return_Status_Code_304()
        {
            A.CallTo(() => _productRepo.GetProductEditionsSinceDateTime(A<DateTime>.Ignored)).Returns(new Products());
            var response = _exchangeServiceApiController.GetProducts("AVCS", A.Dummy<DateTime>()) as ObjectResult;
            response?.StatusCode.Should().Be(304);
        }

        [Test]
        public void Calling_GetProducts_From_Controller_With_Null_SinceDateTime_Should_Return_Status_Code_400()
        {
            var response = _exchangeServiceApiController.GetProducts(A.Dummy<string>(), null) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }
    }
}