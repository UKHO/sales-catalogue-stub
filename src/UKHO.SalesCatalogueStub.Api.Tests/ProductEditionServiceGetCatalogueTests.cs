using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.EF.Models;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{

    class ProductEditionServiceGetCatalogueTests
    {
        private SalesCatalogueStubDbContext _dbContext;
        private IProductEditionService _service;

        [Test]
        public void
            Calls_To_GetProductEditions_Should_Return_Correct_Model_Type()
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Should().AllBeOfType<EssDataInner>();
            serviceResponse.Should().BeOfType(typeof(EssData));
        }

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SalesCatalogueStubDbContext>()
                .UseInMemoryDatabase(databaseName: "inmemory")
                .Options;

            _dbContext = new SalesCatalogueStubDbContext(dbContextOptions);
            _service = new ProductEditionService(new SalesCatalogueStubDbContext(dbContextOptions), A.Fake<ILogger<ProductEditionService>>());

            CreateCatalogue();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        private void CreateProduct(string productName, int? editionNumber, int? updateNumber, int? lastReissueUpdateNumber,
            ProductEditionStatusEnum latestStatus, DateTime? baseIssueDate, DateTime? lastUpdateIssueDate,
            double northLimit, double eastLimit, double southLimit, double westLimit,
            ProductTypeNameEnum productType = ProductTypeNameEnum.Avcs)
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
                        BaseIssueDate = baseIssueDate,
                        LastUpdateIssueDate = lastUpdateIssueDate,
                        PidGeometry = new List<PidGeometry>
                        {
                            new PidGeometry
                            {
                                IsBoundingBox = true,
                                Geom = new MultiPoint(new[]
                                {
                                    new Point(new Coordinate(northLimit, eastLimit)), new Point(new Coordinate(southLimit, eastLimit)),
                                    new Point(new Coordinate(southLimit, westLimit)), new Point(new Coordinate(northLimit, westLimit))
                                })
                            }
                        }
                    }
                },
                ProductType = new ProductType { Name = productType }
            };

            _dbContext.Add(newProduct);
            _dbContext.SaveChanges();
        }

        private void CreateCatalogue()
        {
            CreateProduct("EG3GOA01", 2, null, null, ProductEditionStatusEnum.Base, new DateTime(2020, 5, 13),
                new DateTime(2020, 6, 24), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("AU220120", 5, null, 0, ProductEditionStatusEnum.Base, new DateTime(2020, 9, 7),
                new DateTime(2020, 10, 28), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("1U420222", 8, 11, 0, ProductEditionStatusEnum.Updated, new DateTime(2020, 10, 12),
                new DateTime(2020, 10, 13), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("JP54QNMK", 1, 5, 0, ProductEditionStatusEnum.Updated, new DateTime(2020, 11, 13),
                new DateTime(2020, 12, 28), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("GB340060", 14, 7, 7, ProductEditionStatusEnum.Reissued, new DateTime(2021, 12, 09),
                new DateTime(2021, 1, 15), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("DE521860", 23, 23, 15, ProductEditionStatusEnum.Reissued, new DateTime(2021, 12, 22),
                new DateTime(2021, 12, 30), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("JP44MON8", 9, 2, 0, ProductEditionStatusEnum.Cancelled, new DateTime(2022, 5, 11),
                new DateTime(2022, 11, 16), 24.5666667, 24.266667, 118.558333, 118.091667);
            CreateProduct("MX300511", 16, 8, 0, ProductEditionStatusEnum.Cancelled, new DateTime(2022, 6, 1),
                new DateTime(2022, 6, 16), 24.5666667, 24.266667, 118.558333, 118.091667);
        }
    }
}
