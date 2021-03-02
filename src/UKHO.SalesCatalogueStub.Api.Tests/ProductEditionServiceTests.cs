using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.EF.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ProductEditionServiceTests
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

        [TestCase("GB1234", 2, 3, ProductEditionStatusEnum.Updated, ProductTypeNameEnum.Avcs, ExpectedResult = 1)]
        public int Test1(string productName, int editionNumber, int updateNumber, ProductEditionStatusEnum latestStatus, ProductTypeNameEnum productType)
        {
            _dbContext.AddRange(new List<Product>
            {
                new Product
                {
                    ProductEditions = new List<ProductEdition>
                    {
                        new ProductEdition
                        {
                            LatestStatus = latestStatus,
                            EditionIdentifier = productName,
                            EditionNumber = editionNumber.ToString(),
                            UpdateNumber = updateNumber,
                        }
                    },
                    ProductType = new ProductType {Name = productType}
                }
            });

            _dbContext.SaveChanges();

            return _service.GetProductEditions(new List<string> { "GB1234", "FOOBAR" }).Count;
        }

        [Test]
        public void Test_GetProductEditionsSinceDateTime_Given_LatestStatus_Base_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base
                });

            var productEditions = _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.Single();

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().ContainSingle().Which.Should().Be(0);
        }

        [Test]
        public void Test_GetProductEditionsSinceDateTime_Given_LatestStatus_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().HaveCount(2).And.BeEquivalentTo(new List<int>{0, 1});
        }

        private void SimpleProductEditionSetupByStatusCollection(string productName, int editionNumber, ICollection<ProductEditionStatusEnum> statusCollection)
        {
            var lifecycleEvents = new List<LifecycleEvent>();

            for (var i = 0; i < statusCollection.Count; i++)
            {
                lifecycleEvents.Add(new LifecycleEvent
                {
                    EventType = new EventType
                    {
                        Name = statusCollection.ElementAt(i).ToString()
                    },
                    LastUpdated = DateTime.Now.AddDays(-(statusCollection.Count - i))
                });
            }

            _dbContext.AddRange(new List<Product>
            {
                new Product
                {
                    Identifier = productName,
                    ProductEditions = new List<ProductEdition>
                    {
                        new ProductEdition
                        {
                            LatestStatus = statusCollection.Last(),
                            EditionNumber = editionNumber.ToString(),
                            UpdateNumber = null,
                            LifecycleEvents = lifecycleEvents
                        }
                    },
                    ProductType = new ProductType {Name = ProductTypeNameEnum.Avcs}
                }
            });

            _dbContext.SaveChanges();
        }
    }
}