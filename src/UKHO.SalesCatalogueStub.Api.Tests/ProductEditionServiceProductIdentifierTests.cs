﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
        public async Task Calls_To_GetProductEditions_With_An_Empty_Product_List_Returns_Zero_Product_Edition_Matches()
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { "" });

            serviceResponse.Products.Should().NotBeNull().And.BeEmpty();
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
        }

        [Test]
        public async Task Calls_To_GetProductEditions_With_An_Invalid_Product_And_1_Valid_Returns_1_Product_Edition_Matches_And_1_Count_Reason()
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { "", "AU220120" });

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(2);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(1);
        }

        [Test]
        public async Task Calls_To_GetProductEditions_With_A_Single_Null_Product_Returns_Zero_Product_Edition_Matches()
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { null });

            serviceResponse.Products.Should().NotBeNull().And.BeEmpty();
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
        }

        [Test]
        public void Calls_To_GetProductEditions_With_Null_Throws_ArgumentNullException()
        {
            PopulateTestProductData();
            _service.Invoking(a => a.GetProductIdentifiers(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Calls_To_GetProductEditions_With_Duplicate_Match_Throws_ArgumentNullExceptions()
        {
            PopulateTestProductData();
            // Add duplicate
            CreateProduct("EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Base);

            _service.Invoking(a => a.GetProductIdentifiers(new List<string> { "EG3GOA01" })).Should()
                .Throw<InvalidOperationException>();
        }

        [Test]
        public async Task Calls_To_GetProductEditions_Should_Remove_Duplicates_And_Return_Single_Match()
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { "EG3GOA01", "EG3GOA01 ", "Eg3goa01", "EG3GOA01  " });

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(4);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(0);
        }


        [TestCase("EG3GOA01", 2, null, 0, ProductEditionStatusEnum.Base, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("AU220120", 8, 11, 0, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, ExpectedResult = 12)]
        [TestCase("1U420222", 1, 5, 3, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, ExpectedResult = 3)]
        [TestCase("JP54QNMK", 12, 7, 7, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("JP44MON8", 12, 5, 5, ProductEditionStatusEnum.Reissued, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        [TestCase("GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, ExpectedResult = 2)]
        [TestCase("DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, ExpectedResult = 2)]
        public async Task<int> Calls_To_GetProductEditions_With_A_Matching_Product_Return_A_Product_Edition_With_Expected_Number_Of_Updates(
                string productName, int editionNumber, int updateNumber, int lastReissueUpdateNumber,
                ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType)
        {
            CreateProduct(productName, editionNumber, updateNumber, lastReissueUpdateNumber, latestStatus, productType);

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { productName });

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(0);

            return serviceResponse.Products.First().UpdateNumbers.Count;
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
        public async Task
            Calls_To_GetProductEditions_With_A_Matching_Product_Returns_A_Product_Edition_With_Expected_Update_List(
                string productName, int editionNumber, int updateNumber, int lastReissueUpdateNumber,
                ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType, List<int?> expected)
        {
            CreateProduct(productName, editionNumber, updateNumber, lastReissueUpdateNumber, latestStatus, productType);

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { productName });

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(0);

            serviceResponse.Products.First().UpdateNumbers.Should().ContainInOrder(expected);
        }

        private static readonly object[] ProductEditionCancellationCases =
        {
            new object[] { "GB340060", 0, 2 },
            new object[] { "DE521860", 0, 2 },
            new object[] { "AU5143P1", 0, 11 },
            new object[] { "AU5164P1", 0, 15 },
        };

        [Test, TestCaseSource(nameof(ProductEditionCancellationCases))]
        public async Task
            Calls_To_GetProductEditions_With_A_Matching_Cancelled_Product_Cancelled_In_Last_12Months_Returns_A_Product_Edition_With_Cancellation_Details(
                string productName, int expectedEditionNumber, int expectedUpdateNumber)
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { productName });

            var cancellation = serviceResponse.Products.Single().Cancellation;

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(0);

            cancellation.EditionNumber.Should().Be(expectedEditionNumber);
            cancellation.UpdateNumber.Should().Be(expectedUpdateNumber);
        }

        [TestCase("GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled, -366, 0, 2)]
        [TestCase("DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled, -366, 0, 2)]
        [TestCase("AU5143P1", 5, 10, 0, ProductEditionStatusEnum.Cancelled, -366, 0, 11)]
        [TestCase("AU5164P1", 5, 14, 5, ProductEditionStatusEnum.Cancelled, -366, 0, 15)]
        public async Task
            Calls_To_GetProductEditions_With_A_Matching_Cancelled_Product_Cancelled_Greater_Than_12Months_Ago_Returns_Entry_In_Reasons(
                string productName, int editionNumber, int updateNumber, int lastReissueNumber,
                ProductEditionStatusEnum latestStatus, int daysSinceCancelled, int expectedEditionNumber,
                int expectedUpdateNumber)
        {
            CreateProduct(productName, editionNumber, updateNumber, lastReissueNumber, latestStatus, ProductTypeNameEnum.Avcs, daysSinceCancelled);

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { productName });

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(0);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(1);

            var reasons = serviceResponse.ProductCounts.RequestedProductsNotReturned.First();
            reasons.ProductName.Should().Be(productName);
            reasons.Reason.Should().Be(RequestedProductsNotReturned.ReasonEnum.NoDataAvailableForCancelledProductEnum);
        }

        private static readonly object[] ProductEditionWithoutCancellationCases =
        {
            new object[] { "EG3GOA01"},
            new object[] { "AU220120"},
            new object[] { "1U420222"},
            new object[] { "JP54QNMK"},
        };

        [Test, TestCaseSource(nameof(ProductEditionWithoutCancellationCases))]
        public async Task
            Calls_To_GetProductEditions_With_A_Matching_Product_Without_Cancellation_Returns_A_Product_Edition_With_No_Cancellation_Details(
                string productName)
        {
            PopulateTestProductData();

            var serviceResponse = await _service.GetProductIdentifiers(new List<string> { productName });

            var cancellation = serviceResponse.Products.Single().Cancellation;

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().BeEmpty();

            cancellation.Should().Be(null);
        }

        [Test]
        public async Task Calls_To_GetProductEditions_Should_Return_Expected_Model_Types()
        {
            PopulateTestProductData();
            var productList = new List<string> { "JP54QNMK" };

            var serviceResponse = await _service.GetProductIdentifiers(productList);

            serviceResponse.Products.Should().AllBeOfType<ProductsInner>();
            serviceResponse.Should().BeOfType(typeof(ProductResponse));
        }

        [Test]
        public async Task
            Calls_To_GetProductEditions_With_A_Single_Matching_Product_Returns_Expected_Product_Edition_Details()
        {
            PopulateTestProductData();
            var productList = new List<string> { "JP54QNMK" };

            var serviceResponse = await _service.GetProductIdentifiers(productList);

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(1);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(1);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().BeEmpty();

            serviceResponse.Products.Should().BeEquivalentTo(new List<ProductsInner>
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
        public async Task
            Calls_To_GetProductEditions_With_A_Multiple_Matching_Products_Returns_Expected_Product_Edition_Details()
        {
            PopulateTestProductData();

            var productList = new List<string> { "JP54QNMK", "AU5143P1" };

            var serviceResponse = await _service.GetProductIdentifiers(productList);

            var jpProduct = serviceResponse.Products.Single(a => a.ProductName == "JP54QNMK");
            var auProduct = serviceResponse.Products.Single(a => a.ProductName == "AU5143P1");

            serviceResponse.Products.Should().NotBeNull().And.HaveCount(2);
            serviceResponse.ProductCounts.ReturnedProductCount.Should().Be(2);
            serviceResponse.ProductCounts.RequestedProductCount.Should().Be(2);
            serviceResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            serviceResponse.ProductCounts.RequestedProductsNotReturned.Should().BeEmpty();

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
            ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType = ProductTypeNameEnum.Avcs, int daysSinceUpdate = 0)
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
                        LastReissueUpdateNumber = lastReissueUpdateNumber,
                        LastUpdateIssueDate = DateTime.Now.AddDays(daysSinceUpdate)
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
            CreateProduct("GB340060", 21, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, -300);
            CreateProduct("DE521860", 6, 1, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, -300);
            CreateProduct("AU5143P1", 5, 10, 0, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, -300);
            CreateProduct("AU5164P1", 5, 14, 5, ProductEditionStatusEnum.Cancelled, ProductTypeNameEnum.Avcs, -300);
        }
    }
}
