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
    public class ProductEditionServiceGetVersionsTests
    {
        private SalesCatalogueStubDbContext _dbContext;
        private IProductEditionService _service;

        [SetUp]
        public void Test_Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SalesCatalogueStubDbContext>()
                .UseInMemoryDatabase(databaseName: "inmemory")
                .Options;

            _dbContext = new SalesCatalogueStubDbContext(dbContextOptions);
            _service = new ProductEditionService(new SalesCatalogueStubDbContext(dbContextOptions), A.Fake<ILogger<ProductEditionService>>());
        }

        [TearDown]
        public void Test_TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task Test_GetProductVersions_When_MultipleProductsProvided_And_OneHasNullProductName_Return_RequestedProductsNotReturned_With_Empty_ProductName_And_ReasonInvalid()
        {
            // Arrange
            var (_, productVersionOne, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

            productVersionOne.UpdateNumber--;

            var productVersionTwo = new ProductVersionsInner
            {
                EditionNumber = 2,
                UpdateNumber = 2,
                ProductName = null
            };

            // Act
            var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersionOne, productVersionTwo });

            // Assert
            actualProducts.Products.Should().NotBeNull().And.ContainSingle();
            actualProducts.ProductCounts.ReturnedProductCount.Should().Be(1);
            actualProducts.ProductCounts.RequestedProductCount.Should().Be(1);
            actualProducts.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
            actualProducts.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(1);
            actualProducts.ProductCounts.RequestedProductsNotReturned.Single().Should().BeEquivalentTo(
                new RequestedProductsNotReturned
                {
                    ProductName = string.Empty,
                    Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                });

            actualProducts.Products.First().UpdateNumbers.Should().Equal(new[] { 1, 2, 3, 4 });
            response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        }

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionProvidedIsCurrent_UpdateNumberNotProvided__NoReissues_Returns_AllUpdatesForCurrentEdition()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

        //    productVersion.UpdateNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Products.Should().NotBeNull().And.ContainSingle();
        //    actualProducts.ProductCounts.ReturnedProductCount.Should().Be(1);
        //    actualProducts.ProductCounts.RequestedProductCount.Should().Be(1);
        //    actualProducts.ProductCounts.RequestedProductsAlreadyUpToDateCount.Should().Be(0);
        //    actualProducts.ProductCounts.RequestedProductsNotReturned.Should().HaveCount(0);

        //    actualProducts.Products.First().UpdateNumbers.Should().Equal(new[] { 1, 2, 3, 4 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NoEditionOrUpdateNumbersProvided_And_NoReIssue_And_LatestStatusUpdated_Returns_AllUpdatesForCurrentEditionFromBase()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

        //    productVersion.UpdateNumber = null;
        //    productVersion.EditionNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 0, 1, 2, 3, 4 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NoEditionOrUpdateProvided_And_LatestStatusBase_Returns_ZeroAsOnlyUpdate()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, null, ProductEditionStatusEnum.Base);

        //    productVersion.UpdateNumber = null;
        //    productVersion.EditionNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 0 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NoEditionOrUpdateProvided_And_LatestStatusReIssue_Returns_AllUpdatesForCurrentEditionFromReIssue()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Reissued, LastReissueUpdateNumber: 2);

        //    productVersion.UpdateNumber = null;
        //    productVersion.EditionNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equal(new[] { 2, 3, 4 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_ReIssued_And_ReIssuedUpdateProvided_Returns_AllUpdatesForCurrentEditionFromUpdateAfterThatProvided()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("JP54NC8S", 12, 7, ProductEditionStatusEnum.Updated, LastReissueUpdateNumber: 6);

        //    productVersion.UpdateNumber = 6;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equal(new[] { 7 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_UpdateProvidedButNoEdition_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

        //    productVersion.EditionNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoProductsFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionNumberProvidedIsCurrent_And_UpdateNumberBeforeCurrentProvided_And_NoReissues_Returns_AllUpdatesForCurrentEdition()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated);

        //    productVersion.UpdateNumber = 1;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 2, 3, 4 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[TestCase(2, null, TestName = "No update number provided")]
        //[TestCase(1, 3, TestName = "Edition before the current edition provided")]
        //[TestCase(2, 1, TestName = "Update number before the current update number provided")]
        //public async Task Test_GetProductVersions_When_EditionHasReissue_Returns_AllUpdatesForCurrentEditionFromReissueOnwards(int callerEdition, int callerUpdate)
        //{
        //    // Arrange
        //    var (dbProduct, productVersionOne, _) = CreateProduct("GB1", 2, 4, ProductEditionStatusEnum.Updated, LastReissueUpdateNumber: 2);

        //    productVersionOne.UpdateNumber = callerUpdate;
        //    productVersionOne.EditionNumber = callerEdition;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersionOne });

        //    // Assert
        //    var currentEditionNumber = Convert.ToInt32(dbProduct.ProductEditions.First().EditionNumber);

        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 2, 3, 4 });
        //    actualProducts.First().EditionNumber.Should().Be(currentEditionNumber);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NoUpdateNumberProvided_Return_AllUpdatesForCurrentEdition()
        //{
        //    // Arrange
        //    var (_, productVersionOne, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);

        //    productVersionOne.UpdateNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersionOne });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 1, 2, 3 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NullVersionProvidedInProductVersions_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, _, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { null });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoProductsFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_NullVersion_And_ValidVersion_Provided_Returns_UpdatesForValidVersion()
        //{
        //    // Arrange
        //    var (_, _, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);

        //    var productVersionTwo = new ProductVersionsInner
        //    {
        //        ProductName = "GB1",
        //        EditionNumber = 2,
        //        UpdateNumber = 1
        //    };

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { null, productVersionTwo });

        //    // Assert
        //    actualProducts.Should().HaveCount(1);
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 2, 3 });
        //    actualProducts.First().EditionNumber.Should().Be(2);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionLowerThanCurrentIsProvided_And_LatestStatusBase_Returns_CurrentEditionNumber_And_UpdateNumberZero()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, null, ProductEditionStatusEnum.Base);
        //    productVersion.EditionNumber = 1;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().EditionNumber.Should().Be(2);
        //    actualProducts.First().UpdateNumbers.Single().Should().Be(0);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionProvidedButNoUpdate_And_LatestStatusBase_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, null, ProductEditionStatusEnum.Base);
        //    productVersion.EditionNumber = 2;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionAndUpdateZeroProvided_And_LatestStatusBase_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, null, ProductEditionStatusEnum.Base);
        //    productVersion.EditionNumber = 2;
        //    productVersion.UpdateNumber = 0;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_LatestStatusSuperseded_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Superseded);

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoProductsFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_ThereAreNoUpdatesSinceThatProvided_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_ThereAreNoUpdatesSinceThatProvided_And_CurrentStatusReIssue_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Reissued);

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionProvidedIsGreaterThanCurrent_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);
        //    productVersion.EditionNumber++;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_UpdateNumberProvidedIsGreaterThanCurrent_AndEditionProvidedIsCurrent_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated);
        //    productVersion.UpdateNumber++;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_UpdateNumberProvidedIsGreaterZero__And_StatusIsBase_And_EditionProvidedIsCurrent_Returns_NoProducts()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, null, ProductEditionStatusEnum.Base);
        //    productVersion.UpdateNumber = 100;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.NoUpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_EditionProvidedIsLowerThanCurrent_And_CurrentEditionHasNoReissue_Returns_CurrentEdition_And_AnyUpdates_IncludingZero()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("GB1234", 2, 3, ProductEditionStatusEnum.Updated, LastReissueUpdateNumber: null);
        //    productVersion.EditionNumber = 1;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 0, 1, 2, 3 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_CancelledCellIsOnlyUpdateSinceEdition_And_UpdateRequestedIsThatCancellation_Returns_Expected()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("CA570179", 1, null, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().EditionNumber.Should().Be(1);
        //    actualProducts.First().Cancellation.UpdateNumber.Should().Be(1);
        //    actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
        //    actualProducts.First().UpdateNumbers.Should().HaveCount(0);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_CancelledCellHasPreviousUpdatesForEdition_And_EditionRequestedIsBeforeEditionAndUpdateRequested_Returns_Expected()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("US4WI21M", 16, 3, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);
        //    productVersion.EditionNumber = 1;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 0, 1, 2, 3 });
        //    actualProducts.First().EditionNumber.Should().Be(16);
        //    actualProducts.First().Cancellation.UpdateNumber.Should().Be(4);
        //    actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_CancelledCellHasPreviousUpdatesForEdition_And_NoUpdateProvided_Returns_Expected()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("US4WI21M", 16, 3, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);
        //    productVersion.UpdateNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().Equals(new[] { 1, 2, 3 });
        //    actualProducts.First().EditionNumber.Should().Be(16);
        //    actualProducts.First().Cancellation.UpdateNumber.Should().Be(4);
        //    actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_When_CancelledCellIsOnlyUpdateSinceEdition_And_NoUpdateOrEditionProvided_Returns_Expected()
        //{
        //    // Arrange
        //    var (_, productVersion, _) = CreateProduct("CA570179", 1, null, ProductEditionStatusEnum.Cancelled, LastReissueUpdateNumber: null);
        //    productVersion.UpdateNumber = null;
        //    productVersion.EditionNumber = null;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersion });

        //    // Assert
        //    actualProducts.First().UpdateNumbers.Should().HaveCount(0);
        //    actualProducts.First().EditionNumber.Should().Be(1);
        //    actualProducts.First().Cancellation.UpdateNumber.Should().Be(1);
        //    actualProducts.First().Cancellation.EditionNumber.Should().Be(0);
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public async Task Test_GetProductVersions_OnlyProductsRequestedAreReturned()
        //{
        //    // Arrange
        //    var (_, productVersionOne, _) = CreateProduct("GB1", 2, 3, ProductEditionStatusEnum.Updated);
        //    var (_, productVersionTwo, _) = CreateProduct("GB2", 3, 4, ProductEditionStatusEnum.Updated);
        //    _ = CreateProduct("GB3", 2, 3, ProductEditionStatusEnum.Updated);

        //    productVersionOne.UpdateNumber = 2;
        //    productVersionTwo.UpdateNumber = 1;

        //    // Act
        //    var (actualProducts, response) = await _service.GetProductVersions(new ProductVersions { productVersionOne, productVersionTwo });

        //    // Assert
        //    actualProducts.Should().HaveCount(2);
        //    actualProducts.Single(p => p.ProductName == productVersionOne.ProductName).UpdateNumbers.Should().Equals(new[] { 3 });
        //    actualProducts.Single(p => p.ProductName == productVersionTwo.ProductName).UpdateNumbers.Should().Equals(new[] { 2, 3, 4 });
        //    response.Should().Be(GetProductVersionResponseEnum.UpdatesFound);
        //}

        //[Test]
        //public void Test_Calls_To_GetProductEditions_With_Null_Throws_ArgumentNullException()
        //{
        //    _service.Invoking(async a => await a.GetProductVersions(null)).Should().Throw<ArgumentNullException>();
        //}

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