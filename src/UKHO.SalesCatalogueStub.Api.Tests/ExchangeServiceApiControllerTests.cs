using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ExchangeServiceApiControllerTests
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
            var dummyInput = A.CollectionOfDummy<string>(1).ToList();
            A.CallTo(() => _productRepo.GetProductEditions(A<List<string>>.Ignored)).Returns(new Products { A.Dummy<ProductsInner>() });
            var response = _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), dummyInput) as ObjectResult;
            response?.StatusCode.Should().Be(200);
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
        public void Calling_PostProductIdentifiers_With_Null_Input_Should_Return_Status_Code_400()
        {
            var response = _exchangeServiceApiController.PostProductIdentifiers(A.Dummy<string>(), null) as ObjectResult;
            response?.StatusCode.Should().Be(400);
        }
    }
}