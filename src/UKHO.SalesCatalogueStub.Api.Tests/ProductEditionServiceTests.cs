using System.Collections.Generic;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
            _dbContext.AddRange(new List<Products>
            {
                new Products
                {
                    ProductEditions = new List<ProductEditions>
                    {
                        new ProductEditions
                        {
                            LatestStatus = latestStatus,
                            EditionIdentifier = productName,
                            EditionNumber = editionNumber.ToString(),
                            UpdateNumber = updateNumber,
                        }
                    },
                    ProductType = new ProductTypes {Name = productType}
                }
            });

            _dbContext.SaveChanges();

            return _service.GetProductEditions(new List<string> { "GB1234", "FOOBAR" }).Count;
        }
    }
}