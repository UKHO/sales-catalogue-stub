using System;
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
    public class ProductEditionServiceGetProductEditionsSinceDateTimeTests
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
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Solely_Base_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber,null, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.Single();

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 0 });
            productEdition.FileSize.Should().Be(100);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Solely_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 5;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?>{ expectedUpdateNumber });
            productEdition.FileSize.Should().Be(600);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Solely_Reissued_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 5;
            var expectedReissueNumber = 5;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Reissued
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?>{ expectedReissueNumber });
            productEdition.FileSize.Should().Be(600);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Solely_Cancelled_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Cancelled
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?>{ });
            productEdition.FileSize.Should().Be(200);
            productEdition.Cancellation.Should().BeEquivalentTo(new Cancellation
            {
                EditionNumber = 0,
                UpdateNumber = expectedUpdateNumber + 1
            });
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Solely_Superseded_Then_Empty_Collection_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Superseded
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_With_Multiple_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 5;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?>{ 3, 4, 5 });
            productEdition.FileSize.Should().Be(600);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Of_Base_Followed_By_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?>{0, 1});
            productEdition.FileSize.Should().Be(200);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Of_Reissued_Followed_By_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 6;
            var expectedReissueNumber = 5;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Reissued,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 5, 6 });
            productEdition.FileSize.Should().Be(700);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Of_Updated_Followed_By_Reissued_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 6;
            var expectedReissueNumber = 6;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Reissued
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 6 });
            productEdition.FileSize.Should().Be(700);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Of_Updated_Followed_By_Reissued_Followed_By_Updated_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 7;
            var expectedReissueNumber = 6;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Reissued,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 6, 7 });
            productEdition.FileSize.Should().Be(800);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Events_Of_Updated_Followed_By_Cancelled_Then_Correct_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 1;
            var expectedUpdateNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Cancelled
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 1 });
            productEdition.FileSize.Should().Be(200);
            productEdition.Cancellation.Should().BeEquivalentTo(new Cancellation
            {
                EditionNumber = 0,
                UpdateNumber = 2
            });
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_No_Matching_Events_Then_Empty_Collection_Is_Returned()
        {
            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Ignored_Events_Then_Empty_Collection_Is_Returned()
        {
            SimpleProductEditionSetupByStatusCollection("a", 1, null, 0,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Announced,
                    ProductEditionStatusEnum.Archived,
                    ProductEditionStatusEnum.Edition,
                    ProductEditionStatusEnum.New,
                    ProductEditionStatusEnum.Published,
                    ProductEditionStatusEnum.Unavailable
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_ProductType_Not_Avcs_Then_Empty_Collection_Is_Returned()
        {
            _dbContext.AddRange(new List<Product>
            {
                new Product
                {
                    ProductEditions = new List<ProductEdition>
                    {
                        new ProductEdition
                        {
                            EditionIdentifier = "a",
                            LatestStatus = ProductEditionStatusEnum.Base,
                            EditionNumber = "1",
                            UpdateNumber = null,
                            LastReissueUpdateNumber = 0,
                            LifecycleEvents = new List<LifecycleEvent>{
                                new LifecycleEvent
                                {
                                    EventType = new EventType
                                    {
                                        Name = ProductEditionStatusEnum.Base
                                    },
                                    LastUpdated = DateTime.Now
                                }
                            }
                        }
                    },
                    ProductType = new ProductType {Name = ProductTypeNameEnum.Chart}
                }
            });

            _dbContext.SaveChanges();

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.BeEmpty();
        }

        [TestCase("2021-01-01", "2021-01-01")]
        [TestCase("2021-01-01", "2021-01-02")]
        public async Task Test_GetProductEditionsSinceDateTime_Given_LastUpdated_Older_Or_The_Same_As_SinceDateTime_Then_Empty_Collection_Is_Returned(DateTime lastUpdated, DateTime sinceDateTime)
        {
            _dbContext.AddRange(new List<Product>
            {
                new Product
                {
                    ProductEditions = new List<ProductEdition>
                    {
                        new ProductEdition
                        {
                            EditionIdentifier = "a",
                            LatestStatus = ProductEditionStatusEnum.Base,
                            EditionNumber = "1",
                            UpdateNumber = null,
                            LastReissueUpdateNumber = 0,
                            LifecycleEvents = new List<LifecycleEvent>{
                                new LifecycleEvent
                                {
                                    EventType = new EventType
                                    {
                                        Name = ProductEditionStatusEnum.Base
                                    },
                                    LastUpdated = lastUpdated
                                }
                            }
                        }
                    },
                    ProductType = new ProductType {Name = ProductTypeNameEnum.Avcs}
                }
            });

            _dbContext.SaveChanges();

            var productEditions = await _service.GetProductEditionsSinceDateTime(sinceDateTime);

            productEditions.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Two_ProductEditions_For_Same_Product_Then_Only_The_Newer_ProductEdition_Is_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 2;
            var expectedUpdateNumber = 1;
            var expectedReissueNumber = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, 1, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Cancelled
                });

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.ContainSingle();

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 0, 1 });
            productEdition.FileSize.Should().Be(200);
        }

        [Test]
        public async Task Test_GetProductEditionsSinceDateTime_Given_Two_Products_Then_Two_Correct_ProductEditions_Are_Returned()
        {
            var expectedProductName = "a";
            var expectedEditionNumber = 2;
            var expectedUpdateNumber = 2;
            var expectedReissueNumber = 0;

            var expectedProductName2 = "b";
            var expectedEditionNumber2 = 2;
            var expectedUpdateNumber2 = 1;
            var expectedReissueNumber2 = 0;

            SimpleProductEditionSetupByStatusCollection(expectedProductName, expectedEditionNumber, expectedUpdateNumber, expectedReissueNumber,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base,
                    ProductEditionStatusEnum.Updated,
                    ProductEditionStatusEnum.Updated
                });

            SimpleProductEditionSetupByStatusCollection(expectedProductName2, expectedEditionNumber2, expectedUpdateNumber2, expectedReissueNumber2,
                new List<ProductEditionStatusEnum>
                {
                    ProductEditionStatusEnum.Base,
                    ProductEditionStatusEnum.Updated
                });

            var productEditions = await _service.GetProductEditionsSinceDateTime(DateTime.MinValue);

            productEditions.Should().NotBeNull().And.HaveCount(2);

            var productEdition = productEditions.ElementAt(0);

            productEdition.ProductName.Should().Be(expectedProductName);
            productEdition.EditionNumber.Should().Be(expectedEditionNumber);
            productEdition.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 0, 1, 2 });
            productEdition.FileSize.Should().Be(300);

            var productEdition2 = productEditions.ElementAt(1);

            productEdition2.ProductName.Should().Be(expectedProductName2);
            productEdition2.EditionNumber.Should().Be(expectedEditionNumber2);
            productEdition2.UpdateNumbers.Should().BeEquivalentTo(new List<int?> { 0, 1 });
            productEdition2.FileSize.Should().Be(200);
        }

        private void SimpleProductEditionSetupByStatusCollection(string productName, int editionNumber, int? updateNumber, int? reissueNumber, ICollection<ProductEditionStatusEnum> statusCollection)
        {
            var lifecycleEvents = new List<LifecycleEvent>();

            for (var i = 0; i < statusCollection.Count; i++)
            {
                lifecycleEvents.Add(new LifecycleEvent
                {
                    EventType = new EventType
                    {
                        Name = statusCollection.ElementAt(i)
                    },
                    LastUpdated = DateTime.Now.AddDays(-(statusCollection.Count - i))
                });
            }

            _dbContext.AddRange(new List<Product>
            {
                new Product
                {
                    //Identifier = productName,
                    ProductEditions = new List<ProductEdition>
                    {
                        new ProductEdition
                        {
                            EditionIdentifier = productName,
                            LatestStatus = statusCollection.Last(),
                            EditionNumber = editionNumber.ToString(),
                            UpdateNumber = updateNumber,
                            LastReissueUpdateNumber = reissueNumber,
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