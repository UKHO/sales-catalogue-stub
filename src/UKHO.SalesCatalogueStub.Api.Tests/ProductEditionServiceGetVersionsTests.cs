using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductEditionServiceGetVersionsTests
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
        public void GetProductVersions_WhenEditionNumberProvidedIsCurrent_AndNoReissues_AndUpdateNumberNotProvided_Returns_AllUpdatesForCurrentEdition()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

            productVersion.UpdateNumber = null;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 1, 2, 3, 4 });
        }

        [Test]
        public void GetProductVersions_WhenEditionNumberProvidedIsCurrent_AndNoReissues_AndUpdateNumberBeforeCurrentProvided_Returns_AllUpdatesForCurrentEdition()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

            productVersion.UpdateNumber = 1;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 2, 3, 4 });
        }

        [TestCase(2, null, TestName = "No update number provided")]
        [TestCase(1, 3, TestName = "Edition before the current edition provided")]
        [TestCase(2, 1, TestName = "Update number before the current update number provided")]
        public void GetProductVersions_WhenEditionHasReissue_Returns_AllUpdatesForCurrentEdition_FromReissueOnwards(int callerEdition, int callerUpdate)
        {
            // Arrange
            var (dbProduct, productVersionOne, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated, LastReissueUpdateNumber: 2);

            productVersionOne.UpdateNumber = callerUpdate;
            productVersionOne.EditionNumber = callerEdition;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersionOne });

            // Assert
            var currentEditionNumber = Convert.ToInt32(dbProduct.ProductEditions.First().EditionNumber);

            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 2, 3, 4 });
            actualProducts.First().EditionNumber.Should().Be(currentEditionNumber);
        }

        [Test]
        public void GetProductVersions_WhenNoUpdateNumberProvided_Return_AllUpdatesForCurrentEdition()
        {
            // Arrange
            var (_, productVersionOne, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);

            productVersionOne.UpdateNumber = null;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersionOne });

            // Assert
            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 1, 2, 3 });
        }

        [Test]
        public void GetProductVersions_CurrentStatusBase_EditionNumberLowerThanCurrentIsProvided_Returns_CurrentEditionNumber_And_UpdateNumberZero()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, null, ProductEditionStatusEnum.Base);
            productVersion.EditionNumber = 1;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().EditionNumber.Should().Be(2);
            actualProducts.First().UpdateNumbers.Single().Should().Be(0);
        }

        [Test]
        public void GetProductVersions_SupersededStatus_Returns_NoProducts()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Superseded);

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.Should().HaveCount(0);
        }

        [Test]
        public void GetProductVersions_WhenThereAreNoUpdatesSinceThatProvided_Returns_NoProducts()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.Should().HaveCount(0);
        }

        [Test]
        public void GetProductVersions_WhenEditionProvidedIsGreaterThanCurrent_Returns_NoProducts()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);
            productVersion.EditionNumber++;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.Should().HaveCount(0);
        }

        [Test]
        public void GetProductVersions_WhenUpdateNumberProvidedIsGreaterThanCurrent_AndEditionProvidedIsCurrent_Returns_NoProducts()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);
            productVersion.UpdateNumber++;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.Should().HaveCount(0);
        }


        [Test]
        public void GetProductVersions_WhenEditionNumberIsHigherThatProvided_And_CurrentEditionHasNoReissue_Returns_CurrentEdition_And_AnyUpdates_IncludingZero()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated, LastReissueUpdateNumber: null);
            productVersion.EditionNumber = 1;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 0, 1, 2, 3 });
        }

        //this can be updates up to one befor4e cancel but also need an updates from reissue
        [Test]
        public void GetProductVersions_WhenCancelledCellIsOnlyUpdateSinceEdition_And_UpdateRequestedIsThatCancellation_Returns_Expected()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 1, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().EditionNumber.Should().Be(0);
            actualProducts.First().Cancellation.UpdateNumber.Should().Be(1);
            actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
            actualProducts.First().UpdateNumbers.Should().HaveCount(0);
        }

        [Test]
        public void GetProductVersions_WhenCancelledCellHasPreviousUpdatesForEdition_And_UpdateRequestedIsBeforeEditionAndUpdateRquested_Returns_Expected()
        {
            // Arrange
            var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);
            productVersion.UpdateNumber -= 2;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersion });

            // Assert
            actualProducts.First().UpdateNumbers.Should().ContainInOrder(new[] { 2 });
            actualProducts.First().EditionNumber.Should().Be(2);
            actualProducts.First().Cancellation.UpdateNumber.Should().Be(3);
            actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
        }

        [Test]
        public void GetProductVersions_OnlyProductsRequestedAreReturned()
        {
            // Arrange
            var (_, productVersionOne, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);
            var (_, productVersionTwo, _) = CreateProduct("GB2", 3, 4, ProductEditionStatusEnum.Updated);
            _ = CreateProduct("GB3", 2, 3, ProductEditionStatusEnum.Updated);

            productVersionOne.UpdateNumber = 2;
            productVersionTwo.UpdateNumber = 1;

            // Act
            var (actualProducts, _) = _service.GetProductVersions(new ProductVersions { productVersionOne, productVersionTwo });

            // Assert
            actualProducts.Should().HaveCount(2);
            actualProducts.Single(p => p.ProductName == productVersionOne.ProductName).UpdateNumbers.Should().ContainInOrder(new[] { 3 });
            actualProducts.Single(p => p.ProductName == productVersionTwo.ProductName).UpdateNumbers.Should().ContainInOrder(new[] { 2, 3, 4 });
        }

        private (Product, ProductVersionsInner, ProductsInner) CreateProduct(string productName, int editionNumber, int? updateNumber,
            ProductEditionStatusEnum latestStatus, int? LastReissueUpdateNumber = null, ProductTypeNameEnum productType = ProductTypeNameEnum.Avcs)
        {
            var newDbProduct = new Product
            {
                ProductEditions = new List<ProductEdition>
                {
                    new ProductEdition
                    {
                        LatestStatus = latestStatus,
                        EditionIdentifier = productName,
                        EditionNumber = editionNumber.ToString(),
                        UpdateNumber = updateNumber,
                        LastReissueUpdateNumber = LastReissueUpdateNumber
                    }
                },
                ProductType = new ProductType { Name = productType }
            };

            _dbContext.Add(newDbProduct);
            _dbContext.SaveChanges();

            var productVersion = new ProductVersionsInner
            {
                EditionNumber = editionNumber,
                ProductName = productName,
                UpdateNumber = updateNumber
            };

            var productsInner = new ProductsInner
            {
                EditionNumber = editionNumber,
                ProductName = productName
            };

            return (newDbProduct, productVersion, productsInner);
        }
    }
}