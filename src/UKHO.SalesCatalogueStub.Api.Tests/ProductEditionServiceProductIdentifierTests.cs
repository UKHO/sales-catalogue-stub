using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.EF.Models;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ProductEditionServiceProductIdentifierTests
    {
        private SalesCatalogueStubDbContext _dbContext;
        private IProductEditionService _service;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SalesCatalogueStubDbContext>()
                .UseInMemoryDatabase(databaseName: "inmemory")
                .Options;

            _dbContext = new SalesCatalogueStubDbContext(dbContextOptions);
            _service = new ProductEditionService(new SalesCatalogueStubDbContext(dbContextOptions), A.Fake<ILogger<ProductEditionService>>());
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public void Calls_To_GetProductEditions_With_An_Empty_Product_List_Returns_Zero_Product_Edition_Matches()
        {
            PopulateTestProductData();

            var serviceResponse = _service.GetProductEditions(new List<string> { "" });

            serviceResponse.Count.Should().Be(0);
        }

        [Test]
        public void Calls_To_GetProductEditions_With_A_Single_Null_Product_Returns_Zero_Product_Edition_Matches()
        {
            PopulateTestProductData();

            var serviceResponse = _service.GetProductEditions(new List<string> { null });

            serviceResponse.Count.Should().Be(0);
        }

        [Test]
        public void Calls_To_GetProductEditions_With_Null_Throws_ArgumentNullException()
        {
            PopulateTestProductData();
            _service.Invoking(a => a.GetProductEditions(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Calls_To_GetProductEditions_With_Duplicate_Match_Throws_ArgumentNullExceptions()
        {
            PopulateTestProductData();
            // Add duplicate
            CreateProduct("EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Base);

            _service.Invoking(a => a.GetProductEditions(new List<string> { "EG3GOA01" })).Should()
                .Throw<InvalidOperationException>();
        }

        [Test]
        public void Calls_To_GetProductEditions_Should_Remove_Duplicates_And_Return_Single_Match()
        {
            PopulateTestProductData();

            var serviceResponse = _service.GetProductEditions(new List<string> { "EG3GOA01", "EG3GOA01 ", "Eg3goa01", "EG3GOA01  " });
            serviceResponse.Count.Should().Be(1);
        }


        [TestCase("EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Base, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("AU220120", 8, 11, 0, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, ExpectedResult = 12)]
        [TestCase("1U420222", 1, 5, 3, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, ExpectedResult = 3)]
        [TestCase("JP54QNMK", 12, 7, 7, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("JP44MON8", 12, 5, 5, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, ExpectedResult = 2)]
        [TestCase("DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, ExpectedResult = 2)]
        public int
            Calls_To_GetProductEditions_With_A_Matching_Product_Return_A_Product_Edition_With_Correct_Number_Of_Updates(
                string productName, int editionNumber, int updateNumber, int lastReissueUpdateNumber,
                ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType)
        {
            CreateProduct(productName, editionNumber, updateNumber, lastReissueUpdateNumber, latestStatus, productType);

            var serviceResponse = _service.GetProductEditions(new List<string> { productName });

            return serviceResponse.First().UpdateNumbers.Count;
        }

        private static readonly object[] ProductEditionUpdateListCases =
        {
            new object[] { "EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, new List<int?>{ 0 } },
            new object[] { "AU220120", 8, 11, 0, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, new List<int?>{ 0,1,2,3,4,5,6,8,9,10,11 } },
            new object[] { "1U420222", 1, 5, 3, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, new List<int?>{ 3,4,5 } },
            new object[] { "JP54QNMK", 12, 7, 7, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, new List<int?>{ 7 } },
            new object[] { "JP44MON8", 12, 5, 5, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, new List<int?>{ 5 } },
            new object[] { "GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, new List<int?>{ 0,1 } },
            new object[] { "DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, new List<int?>{ 0,1 } },
        };

        [Test, TestCaseSource(nameof(ProductEditionUpdateListCases))]
        public void
            Calls_To_GetProductEditions_With_A_Matching_Product_Returns_A_Product_Edition_With_Correct_Update_List(
                string productName, int editionNumber, int updateNumber, int lastReissueUpdateNumber,
                ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType, List<int?> expected)
        {
            CreateProduct(productName, editionNumber, updateNumber, lastReissueUpdateNumber, latestStatus, productType);

            var serviceResponse = _service.GetProductEditions(new List<string> { productName });

            serviceResponse.First().UpdateNumbers.Should().ContainInOrder(expected);
        }

        private static readonly object[] ProductEditionCancellationCases =
        {
            new object[] { "GB340060", 0, 2 },
            new object[] { "DE521860", 0, 2 },
            new object[] { "AU5143P1", 0, 11 },
            new object[] { "AU5164P1", 0, 15 },
        };

        [Test, TestCaseSource(nameof(ProductEditionCancellationCases))]
        public void
            Calls_To_GetProductEditions_With_A_Matching_Cancelled_Product_Returns_A_Product_Edition_With_Cancellation_Details(
                string productName, int expectedEditionNumber, int expectedUpdateNumber)
        {
            PopulateTestProductData();

            var serviceResponse = _service.GetProductEditions(new List<string> { productName });

            var cancellation = serviceResponse.Single().Cancellation;

            cancellation.EditionNumber.Should().Be(expectedEditionNumber);
            cancellation.UpdateNumber.Should().Be(expectedUpdateNumber);
        }

        private static readonly object[] ProductEditionWithoutCancellationCases =
        {
            new object[] { "EG3GOA01"},
            new object[] { "AU220120"},
            new object[] { "1U420222"},
            new object[] { "JP54QNMK"},
        };

        [Test, TestCaseSource(nameof(ProductEditionWithoutCancellationCases))]
        public void
            Calls_To_GetProductEditions_With_A_Matching_Product_Without_Cancellation_Returns_A_Product_Edition_With_No_Cancellation_Details(
                string productName)
        {
            PopulateTestProductData();

            var serviceResponse = _service.GetProductEditions(new List<string> { productName });

            var cancellation = serviceResponse.Single().Cancellation;

            cancellation.Should().Be(null);
        }

        [Test]
        public void
            Calls_To_GetProductEditions_Should_Return_Correct_Model_Type()
        {
            PopulateTestProductData();
            var productList = new List<string> { "JP54QNMK" };

            var serviceResponse = _service.GetProductEditions(productList);

            serviceResponse.Should().AllBeOfType<ProductsInner>();
            serviceResponse.Should().BeOfType(typeof(Products));
        }

        [Test]
        public void
            Calls_To_GetProductEditions_With_A_Single_Matching_Product_Returns_Expected_Product_Edition_Details()
        {
            PopulateTestProductData();
            var productList = new List<string> { "JP54QNMK" };

            var serviceResponse = _service.GetProductEditions(productList);

            serviceResponse.Should().BeEquivalentTo(new List<ProductsInner>
            {
                new ProductsInner()
                {
                    EditionNumber = 12,
                    Cancellation = null,
                    FileSize = 800,
                    ProductName = "JP54QNMK",
                    UpdateNumbers = new List<int?> {7}
                }
            });
        }

        [Test]
        public void
            Calls_To_GetProductEditions_With_A_Multiple_Matching_Products_Returns_Expected_Product_Edition_Details()
        {
            PopulateTestProductData();

            var productList = new List<string> { "JP54QNMK", "AU5143P1" };

            var serviceResponse = _service.GetProductEditions(productList);

            var jpProduct = serviceResponse.Single(a => a.ProductName == "JP54QNMK");
            var auProduct = serviceResponse.Single(a => a.ProductName == "AU5143P1");

            jpProduct.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 7 });
            jpProduct.Cancellation.Should().BeNull();
            jpProduct.ProductName.Should().Be("JP54QNMK");
            jpProduct.EditionNumber.Should().Be(12);
            jpProduct.FileSize.Should().Be(800);

            auProduct.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            auProduct.Cancellation.Should().BeEquivalentTo(new Cancellation { EditionNumber = 0, UpdateNumber = 11 });
            auProduct.ProductName.Should().Be("AU5143P1");
            auProduct.EditionNumber.Should().Be(5);
            auProduct.FileSize.Should().Be(1100);

        }


        private void CreateProduct(string productName, int? editionNumber, int? updateNumber, int? lastReissueUpdateNumber,
            ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType = ProductTypeNameEnum.Avcs)
        {
            var newProduct = new Product
            {
                ProductEditions = new List<ProductEdition>
                {
                    new ProductEdition
                    {
                        LatestStatus = latestStatus,
                        EditionIdentifier = productName,
                        EditionNumber = editionNumber.ToString(),
                        UpdateNumber = updateNumber,
                        LastReissueUpdateNumber = lastReissueUpdateNumber
                    }
                },
                ProductType = new ProductType { Name = productType }
            };

            _dbContext.Add(newProduct);
            _dbContext.SaveChanges();
        }

        private void PopulateTestProductData()
        {
            CreateProduct("EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Base);
            CreateProduct("AU220120", 8, 11, 0, ProductEditionStatusEnum.Updated);
            CreateProduct("1U420222", 1, 5, 3, ProductEditionStatusEnum.Updated);
            CreateProduct("JP54QNMK", 12, 7, 7, ProductEditionStatusEnum.Reissued);
            CreateProduct("JP44MON8", 12, 5, 5, ProductEditionStatusEnum.Reissued);
            CreateProduct("GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled);
            CreateProduct("DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled);
            CreateProduct("AU5143P1", 5, 10, 0, ProductEditionStatusEnum.Cancelled);
            CreateProduct("AU5164P1", 5, 14, 5, ProductEditionStatusEnum.Cancelled);
        }
    }
}
