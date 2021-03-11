using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
            Calls_To_GetCatalogue_Should_Return_Expected_Model_Types()
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Should().AllBeOfType<EssDataInner>();
            serviceResponse.Should().BeOfType(typeof(EssData));
        }
        [TestCase("EG3GOA01")]
        [TestCase("AU220120")]
        [TestCase("1U420222")]
        [TestCase("JP54QNMK")]
        [TestCase("GB340060")]
        [TestCase("DE521860")]
        [TestCase("JP44MON8")]
        [TestCase("MX300511")]
        public void Calls_To_GetCatalogue_Should_Return_Expected_ProductName_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).ProductName.Should().Be(productName);
        }

        private static readonly object[] GetCatalogueBaseCellIssueDateCases =
        {
            new object[] { "EG3GOA01", new DateTime(2020, 5, 13) },
            new object[] { "AU220120", new DateTime(2020, 9, 7) },
            new object[] { "1U420222", new DateTime(2020, 10, 12) },
            new object[] { "JP54QNMK", new DateTime(2020, 11, 13) },
            new object[] { "GB340060", new DateTime(2021, 12, 09) },
            new object[] { "DE521860", new DateTime(2021, 12, 22) },
            new object[] { "JP44MON8", new DateTime(2022, 5, 11) },
            new object[] { "MX300511", new DateTime(2022, 6, 1) },
        };

        [Test, TestCaseSource(nameof(GetCatalogueBaseCellIssueDateCases))]
        public void Calls_To_GetCatalogue_Should_Return_Expected_BaseCellIssueDate_For_Matched_Edition(string productName, DateTime expected)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).BaseCellIssueDate.Should().Be(expected);
        }

        [TestCase("EG3GOA01", ExpectedResult = 2)]
        [TestCase("AU220120", ExpectedResult = 5)]
        [TestCase("1U420222", ExpectedResult = 8)]
        [TestCase("JP54QNMK", ExpectedResult = 1)]
        [TestCase("GB340060", ExpectedResult = 14)]
        [TestCase("DE521860", ExpectedResult = 23)]
        [TestCase("JP44MON8", ExpectedResult = 0)]
        [TestCase("MX300511", ExpectedResult = 0)]
        public int? Calls_To_GetCatalogue_Should_Return_Expected_BaseCellEditionNumber_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).BaseCellEditionNumber;
        }

        private static readonly object[] GetCatalogueIssueDateLatestUpdateCases =
        {
            new object[] { "EG3GOA01", new DateTime(2020, 6, 24) },
            new object[] { "AU220120", new DateTime(2020, 10, 28) },
            new object[] { "1U420222", new DateTime(2020, 10, 13) },
            new object[] { "JP54QNMK", new DateTime(2020, 12, 28) },
            new object[] { "GB340060", new DateTime(2021, 1, 15) },
            new object[] { "DE521860", new DateTime(2021, 12, 30) },
            new object[] { "JP44MON8", new DateTime(2022, 11, 16) },
            new object[] { "MX300511", new DateTime(2022, 6, 16) },
        };

        [Test, TestCaseSource(nameof(GetCatalogueIssueDateLatestUpdateCases))]
        public void Calls_To_GetCatalogue_Should_Return_Expected_IssueDateLatestUpdate_For_Matched_Edition(string productName, DateTime expected)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).IssueDateLatestUpdate.Should().Be(expected);
        }

        [TestCase("EG3GOA01", ExpectedResult = null)]
        [TestCase("AU220120", ExpectedResult = null)]
        [TestCase("1U420222", ExpectedResult = 11)]
        [TestCase("JP54QNMK", ExpectedResult = 5)]
        [TestCase("GB340060", ExpectedResult = 7)]
        [TestCase("DE521860", ExpectedResult = 23)]
        [TestCase("JP44MON8", ExpectedResult = 2)]
        [TestCase("MX300511", ExpectedResult = 8)]
        public int? Calls_To_GetCatalogue_Should_Return_Expected_LatestUpdateNumber_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).LatestUpdateNumber;
        }

        [TestCase("EG3GOA01", ExpectedResult = 100)]
        [TestCase("AU220120", ExpectedResult = 100)]
        [TestCase("1U420222", ExpectedResult = 1200)]
        [TestCase("JP54QNMK", ExpectedResult = 600)]
        [TestCase("GB340060", ExpectedResult = 800)]
        [TestCase("DE521860", ExpectedResult = 2400)]
        [TestCase("JP44MON8", ExpectedResult = 300)]
        [TestCase("MX300511", ExpectedResult = 900)]
        public int? Calls_To_GetCatalogue_Should_Return_Expected_FileSize_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).FileSize;
        }

        private static readonly object[] GetCatalogueLatitudeCases =
        {
            new object[] { "EG3GOA01", 24.5666667, 118.558333, 24.266667, 118.091667 },
            new object[] { "AU220120", 36.522, 250.355555, 36.233, 250.3444 },
            new object[] { "1U420222", 49.234255, 24.23444, 48.2424, 24.2222 },
            new object[] { "JP54QNMK", 65.23666, 23.23555, 65.23444, 23.11333 },
            new object[] { "GB340060", -100.55533, 92.26555, -101.24245, 92.2455 },
            new object[] { "DE521860", 357.242424, 66.53336, 356.2355, 66.35555 },
            new object[] { "JP44MON8", 56.235, -31.23556, 55.4435, -32.0941667 },
            new object[] { "MX300511", 86.2355555, 143.235555, 86.23444, 143.21112 },
        };

        [Test, TestCaseSource(nameof(GetCatalogueLatitudeCases))]
        public void Calls_To_GetCatalogue_Should_Return_Expected_Latitudes_For_Matched_Edition(string productName, double expectedNorthLatitude, double expectedEastLatitude, double expectedSouthLatitude, double expectedWestLatitude)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            var productEdition = serviceResponse.Single(a => a.ProductName == productName);

            productEdition.CellLimitNorthernmostLatitude.Should().Be((decimal?)expectedNorthLatitude);
            productEdition.CellLimitEasternmostLatitude.Should().Be((decimal?)expectedEastLatitude);
            productEdition.CellLimitSouthernmostLatitude.Should().Be((decimal?)expectedSouthLatitude);
            productEdition.CellLimitWesternmostLatitude.Should().Be((decimal?)expectedWestLatitude);
        }

        private static readonly object[] GetCatalogueCoverageCoordinatesCases =
        {
            new object[] { "EG3GOA01", 0, 0 },
            new object[] { "AU220120",  0, 0 },
            new object[] { "1U420222", 0, 0 },
            new object[] { "JP54QNMK", 0, 0 },
            new object[] { "GB340060", 0, 0 },
            new object[] { "DE521860", 0, 0 },
            new object[] { "JP44MON8", 0, 0 },
            new object[] { "MX300511", 0, 0 }
        };

        [Test, TestCaseSource(nameof(GetCatalogueCoverageCoordinatesCases))]
        public void Calls_To_GetCatalogue_Should_Return_Expected_DataCoverageCoordinates_For_Matched_Edition(string productName,
            double expectedLatitude, double expectedLongitude)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            var productEdition = serviceResponse.Single(a => a.ProductName == productName);

            productEdition.DataCoverageCoordinates.Single().Longitude.Should().Be((decimal?)expectedLongitude);
            productEdition.DataCoverageCoordinates.Single().Latitude.Should().Be((decimal?)expectedLongitude);

        }

        [TestCase("EG3GOA01")]
        [TestCase("AU220120")]
        [TestCase("1U420222")]
        [TestCase("JP54QNMK")]
        [TestCase("GB340060")]
        [TestCase("DE521860")]
        [TestCase("JP44MON8")]
        [TestCase("MX300511")]
        public void Calls_To_GetCatalogue_Should_Return_True_As_Compression_Value_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).Compression.Should().BeTrue();
        }

        [TestCase("EG3GOA01")]
        [TestCase("AU220120")]
        [TestCase("1U420222")]
        [TestCase("JP54QNMK")]
        [TestCase("GB340060")]
        [TestCase("DE521860")]
        [TestCase("JP44MON8")]
        [TestCase("MX300511")]
        public void Calls_To_GetCatalogue_Should_Return_True_As_Encryption_Value_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).Encryption.Should().BeTrue();
        }

        [TestCase("EG3GOA01", ExpectedResult = 0)]
        [TestCase("AU220120", ExpectedResult = 0)]
        [TestCase("1U420222", ExpectedResult = 0)]
        [TestCase("JP54QNMK", ExpectedResult = 0)]
        [TestCase("GB340060", ExpectedResult = 7)]
        [TestCase("DE521860", ExpectedResult = 15)]
        [TestCase("JP44MON8", ExpectedResult = 0)]
        [TestCase("MX300511", ExpectedResult = 0)]
        public int? Calls_To_GetCatalogue_Should_Return_Expected_BaseCellUpdateNumber_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).BaseCellUpdateNumber;
        }

        [TestCase("EG3GOA01")]
        [TestCase("AU220120")]
        [TestCase("1U420222")]
        [TestCase("JP54QNMK")]
        [TestCase("GB340060")]
        [TestCase("DE521860")]
        [TestCase("JP44MON8")]
        [TestCase("MX300511")]
        public void Calls_To_GetCatalogue_Should_Return_Zero_As_LastUpdateNumberPreviousEdition_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).LastUpdateNumberPreviousEdition.Should().Be(0);
        }

        [TestCase("EG3GOA01")]
        [TestCase("AU220120")]
        [TestCase("1U420222")]
        [TestCase("JP54QNMK")]
        [TestCase("GB340060")]
        [TestCase("DE521860")]
        [TestCase("JP44MON8")]
        [TestCase("MX300511")]
        public void Calls_To_GetCatalogue_Should_Return_An_Empty_Array_As_CancelledReplacements_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).CancelledCellReplacements.Should().BeEquivalentTo(new List<string>());
        }

        private static readonly object[] GetCatalogueIssueDatePreviousUpdateCases =
        {
            new object[] { "EG3GOA01", new DateTime(2020, 5, 13) },
            new object[] { "AU220120", new DateTime(2020, 9, 7) },
            new object[] { "1U420222", new DateTime(2020, 10, 12) },
            new object[] { "JP54QNMK", new DateTime(2020, 11, 13) },
            new object[] { "GB340060", new DateTime(2021, 12, 09) },
            new object[] { "DE521860", new DateTime(2021, 12, 22) },
            new object[] { "JP44MON8", new DateTime(2022, 5, 11) },
            new object[] { "MX300511", new DateTime(2022, 6, 1) },
        };
        [Test, TestCaseSource(nameof(GetCatalogueIssueDatePreviousUpdateCases))]
        public void Calls_To_GetCatalogue_Should_Return_Expected_IssueDatePreviousUpdate_For_Matched_Edition(string productName, DateTime expected)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            serviceResponse.Single(a => a.ProductName == productName).IssueDatePreviousUpdate.Should().Be(expected);
        }

        [TestCase("EG3GOA01", ExpectedResult = 0)]
        [TestCase("AU220120", ExpectedResult = 0)]
        [TestCase("1U420222", ExpectedResult = 0)]
        [TestCase("JP54QNMK", ExpectedResult = 0)]
        [TestCase("GB340060", ExpectedResult = 0)]
        [TestCase("DE521860", ExpectedResult = 0)]
        [TestCase("JP44MON8", ExpectedResult = 9)]
        [TestCase("MX300511", ExpectedResult = 16)]
        public int? Calls_To_GetCatalogue_Should_Return_Expected_CancelledEditionNumber_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).CancelledEditionNumber;
        }

        [TestCase("EG3GOA01", ExpectedResult = "M1;B1")]
        [TestCase("AU220120", ExpectedResult = "M1;B2")]
        [TestCase("1U420222", ExpectedResult = "M1;B4")]
        [TestCase("JP54QNMK", ExpectedResult = "M2;B6")]
        [TestCase("GB340060", ExpectedResult = "M2;B7")]
        [TestCase("DE521860", ExpectedResult = "M2;B9")]
        [TestCase("JP44MON8", ExpectedResult = "M1;B1")]
        [TestCase("MX300511", ExpectedResult = "M2;B9")]
        public string Calls_To_GetCatalogue_Should_Return_Expected_BaseCellLocation_For_Matched_Edition(string productName)
        {

            var serviceResponse = _service.GetCatalogue(A.Dummy<DateTime>());

            return serviceResponse.Single(a => a.ProductName == productName).BaseCellLocation;
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
            double northLimit, double eastLimit, double southLimit, double westLimit, int? baseCdNumber,
            ProductTypeNameEnum productType = ProductTypeNameEnum.Avcs)
        {
            PidTombstone pidTombstone = null;
            ICollection<PidGeometry> pidGeometry = null;
            var isCancelled = latestStatus == ProductEditionStatusEnum.Cancelled;

            if (isCancelled)
            {
                pidTombstone = new PidTombstone
                {
                    Id = new Guid(),
                    XmlData = new XElement("ENC", new XElement("ShortName", productName),
                        new XElement("Metadata", new XElement("DatasetTitle", "Test"),
                            new XElement("Scale", "1"),
                            new XElement("GeographicLimit",
                                new XElement("BoundingBox", new XElement("NorthLimit", northLimit),
                                    new XElement("SouthLimit", southLimit), new XElement("EastLimit", eastLimit),
                                    new XElement("WestLimit", westLimit)),
                                new XElement("Polygon",
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "07")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")),
                                    new XElement("Position", new XAttribute("latitude", "0"),
                                        new XAttribute("longitude", "0")))),
                            new XElement("Folio", new XElement("ID", "PAYSF")), new XElement("SAP_IPN", "0"),
                            new XElement("CatalogueNumber", productName),
                            new XElement("Status",
                                new XElement("ChartStatus", new XAttribute("date", "2021-01-01"), "Cancelled"),
                                new XElement("ReplacedByList", new XElement("ReplacedBy", ""))),
                            new XElement("Unit", new XElement("ID", productName)), new XElement("DSNM", productName),
                            new XElement("Usage", "1"), new XElement("Edtn", "1"),
                            new XElement("Base_isdt", "2021-01-01"), new XElement("UPDN", "1"),
                            new XElement("Last_update_isdt", "2021-01-01"), new XElement("Last_reissue_UPDN", "0"),
                            new XElement("CD", new XElement("Base", baseCdNumber), new XElement("Update", "0")))).ToString()
                };

            }
            else
            {
                //Geometries come from product editions table
                pidGeometry = new List<PidGeometry>
                {
                    new PidGeometry
                    {
                        IsBoundingBox = true,
                        Geom = new MultiPoint(new[]
                        {
                            new Point(new Coordinate(northLimit, eastLimit)),
                            new Point(new Coordinate(southLimit, eastLimit)),
                            new Point(new Coordinate(southLimit, westLimit)),
                            new Point(new Coordinate(northLimit, westLimit))
                        })
                    }
                };
            }

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
                        PidTombstone = pidTombstone,
                        PidGeometry = pidGeometry,
                        BaseCd = isCancelled ? null : baseCdNumber
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
                new DateTime(2020, 6, 24), 24.5666667, 118.558333, 24.266667, 118.091667, 1);
            CreateProduct("AU220120", 5, null, 0, ProductEditionStatusEnum.Base, new DateTime(2020, 9, 7),
                new DateTime(2020, 10, 28), 36.522, 250.355555, 36.233, 250.3444, 2);
            CreateProduct("1U420222", 8, 11, 0, ProductEditionStatusEnum.Updated, new DateTime(2020, 10, 12),
                new DateTime(2020, 10, 13), 49.234255, 24.23444, 48.2424, 24.2222, 4);
            CreateProduct("JP54QNMK", 1, 5, 0, ProductEditionStatusEnum.Updated, new DateTime(2020, 11, 13),
                new DateTime(2020, 12, 28), 65.23666, 23.23555, 65.23444, 23.11333, 6);
            CreateProduct("GB340060", 14, 7, 7, ProductEditionStatusEnum.Reissued, new DateTime(2021, 12, 09),
                new DateTime(2021, 1, 15), -100.55533, 92.26555, -101.24245, 92.2455, 7);
            CreateProduct("DE521860", 23, 23, 15, ProductEditionStatusEnum.Reissued, new DateTime(2021, 12, 22),
                new DateTime(2021, 12, 30), 357.242424, 66.53336, 356.2355, 66.35555, 9);
            CreateProduct("JP44MON8", 9, 2, 0, ProductEditionStatusEnum.Cancelled, new DateTime(2022, 5, 11),
                new DateTime(2022, 11, 16), 56.235, -31.23556, 55.4435, -32.0941667, 1);
            CreateProduct("MX300511", 16, 8, 0, ProductEditionStatusEnum.Cancelled, new DateTime(2022, 6, 1),
                new DateTime(2022, 6, 16), 86.2355555, 143.235555, 86.23444, 143.21112, 9);
        }
    }
}
