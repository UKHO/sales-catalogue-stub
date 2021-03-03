using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Calling_PostProductIdentifiers_With_At_Least_One_Matching_Product_Should_Return_Status_Code_200()
        {
            var testData = new List<string>
            {
                "AU220120", "EG3GOA01"
            };
            A.CallTo(() => _productRepo.GetProductEditions(testData)).Returns(new Products
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
            var response = _exchangeServiceApiController.PostProductIdentifiers("AVCS", testData) as ObjectResult;
            response?.StatusCode.Should().Be(200);
        }

        [Test]
        public void Calling_PostProductIdentifiers_With_A_Product_List_Should_Return_Expected_Json_Response()
        {
            var testData = new List<string>
            {
                "AU220120", "EG3GOA01"
            };

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
            A.CallTo(() => _productRepo.GetProductEditions(testData)).Returns(testResponse);
            var response = _exchangeServiceApiController.PostProductIdentifiers("AVCS", testData) as ObjectResult;
            response?.Value.Should().BeEquivalentTo(expectedJson);
        }

        [Test]
        public void Calling_PostProductIdentifiers_With_No_Matching_Products_Should_Return_Status_Code_400()
        {
            var dummyInput = A.CollectionOfDummy<string>(1).ToList();
            A.CallTo(() => _productRepo.GetProductEditions(A<List<string>>.Ignored)).Returns(new Products());
            var response = _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), dummyInput) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }

        [Test]
        public void Calling_PostProductIdentifiers_With_List_Of_Null_Should_Return_Status_Code_400()
        {
            var testData = new List<string>
            {
                null
            };
            A.CallTo(() => _productRepo.GetProductEditions(testData)).Returns(new Products());
            var response = _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), testData) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }

        [Test]
        public void Calling_GetProductEditions_From_Controller_With_Null_Input_Should_Throw_ArgumentNullException()
        {
            A.CallTo(() => _productRepo.GetProductEditions(null)).Throws<ArgumentNullException>();
            _exchangeServiceApiController.Invoking(a => a.PostProductIdentifiers(A.Dummy<string>(), null)).Should().Throw<ArgumentNullException>();
        }
    }
}