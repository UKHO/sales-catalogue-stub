using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.Controllers;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class CatalogueApiControllerTests
    {
        private IProductEditionService _productRepo;
        private CatalogueApiController _catalogueApiController;

        [SetUp]
        public void Setup()
        {
            _productRepo = A.Fake<IProductEditionService>();
            _catalogueApiController = new CatalogueApiController(_productRepo);
            CreateCatalogue();
        }
        [Test]
        public async Task Calling_GetCatalogue_With_No_IsModifiedSince_Should_Return_Status_Code_200()
        {
            CreateLoaderStatus(new DateTime(2020, 03, 12), true);
            var response = await _catalogueApiController.GetCatalogue("AVCS", "ESS", null) as ObjectResult;
            response?.StatusCode.Should().Be(200);
            response.Should().NotBeNull();
        }
        [Test]
        public async Task Calling_GetCatalogue_With_IsModifiedSince_Earlier_Than_Latest_DateEntered_Should_Return_Status_Code_200_Ok()
        {
            CreateLoaderStatus(new DateTime(2020, 03, 13), true);
            var response = await _catalogueApiController.GetCatalogue("AVCS", "ESS", new DateTime(2020, 03, 12)) as ObjectResult;
            response?.StatusCode.Should().Be(200);
            response.Should().NotBeNull();
        }
        [Test]
        public async Task Calling_GetCatalogue_With_IsModifiedSince_Later_Than_Latest_DateEntered_Should_Return_Status_Code_304_Not_Modified()
        {
            CreateLoaderStatus(new DateTime(2020, 03, 12), false);
            var response = await _catalogueApiController.GetCatalogue("AVCS", "ESS", new DateTime(2020, 03, 13)) as ObjectResult;
            response?.StatusCode.Should().Be(304);
            response.Should().NotBeNull();
        }
        [Test]
        public async Task Calling_GetCatalogue_With_No_IsModifiedSince_Should_Return_Expected_Object_Response()
        {
            CreateLoaderStatus(new DateTime(2020, 03, 12), true);
            var response = await _catalogueApiController.GetCatalogue("AVCS", "ESS", null) as ObjectResult;
            response?.Value.Should().BeOfType<EssData>();
            response.Should().NotBeNull();
        }

        private void CreateLoaderStatus(DateTime? dateEntered, bool isModified)
        {
            A.CallTo(() => _productRepo.CheckIfCatalogueModified(A<DateTime?>.Ignored))
                .Returns(Task.FromResult((isModified, dateEntered)));
        }

        private void CreateCatalogue()
        {
            A.CallTo(() => _productRepo.GetCatalogue()).Returns(new EssData
            {
                new EssDataInner
                {
                    ProductName = "IT400115",
                    BaseCellIssueDate = new DateTime(2019, 1, 7),
                    BaseCellEditionNumber = 4,
                    IssueDateLatestUpdate = new DateTime(2019, 1, 19),
                    LatestUpdateNumber = 18,
                    FileSize = 1900,
                    CellLimitSouthernmostLatitude = (decimal?) 43.924,
                    CellLimitWesternmostLatitude = (decimal?) 9.699,
                    CellLimitNorthernmostLatitude = (decimal?) 44.1264675,
                    CellLimitEasternmostLatitude = (decimal?) 10.1105547,
                    DataCoverageCoordinates = new List<DataCoverageCoordinate>(),
                    Compression = true,
                    Encryption = true,
                    BaseCellUpdateNumber = 0,
                    LastUpdateNumberPreviousEdition = null,
                    BaseCellLocation = "M1;B4",
                    CancelledCellReplacements = new List<string>(),
                    IssueDatePreviousUpdate = new DateTime(2019, 1, 7),
                    CancelledEditionNumber = 0
                },
                new EssDataInner
                {
                    ProductName = "RU4PFQN0",
                    BaseCellIssueDate = new DateTime(2020, 5, 24),
                    BaseCellEditionNumber = 4,
                    IssueDateLatestUpdate = new DateTime(2020, 6, 2),
                    LatestUpdateNumber = 5,
                    FileSize = 600,
                    CellLimitSouthernmostLatitude = (decimal?) 53.2345,
                    CellLimitWesternmostLatitude = (decimal?) 23.23,
                    CellLimitNorthernmostLatitude = (decimal?) 54.2334,
                    CellLimitEasternmostLatitude = (decimal?) 24.234,
                    DataCoverageCoordinates = new List<DataCoverageCoordinate>(),
                    Compression = true,
                    Encryption = true,
                    BaseCellUpdateNumber = 0,
                    LastUpdateNumberPreviousEdition = null,
                    BaseCellLocation = "M2;B6",
                    CancelledCellReplacements = new List<string>(),
                    IssueDatePreviousUpdate = new DateTime(2020, 6, 2),
                    CancelledEditionNumber = 0
                },
                new EssDataInner
                {
                    ProductName = "VE400102",
                    BaseCellIssueDate = new DateTime(2021, 2, 12),
                    BaseCellEditionNumber = 4,
                    IssueDateLatestUpdate = new DateTime(2021, 3, 8),
                    LatestUpdateNumber = 2,
                    FileSize = 300,
                    CellLimitSouthernmostLatitude = (decimal?) 45.1111,
                    CellLimitWesternmostLatitude = (decimal?) 39.5343,
                    CellLimitNorthernmostLatitude = (decimal?) 46.2344,
                    CellLimitEasternmostLatitude = (decimal?) 39.5655,
                    DataCoverageCoordinates = new List<DataCoverageCoordinate>(),
                    Compression = true,
                    Encryption = true,
                    BaseCellUpdateNumber = 0,
                    LastUpdateNumberPreviousEdition = null,
                    BaseCellLocation = "M2;B9",
                    CancelledCellReplacements = new List<string>(),
                    IssueDatePreviousUpdate = new DateTime(2020, 3, 8),
                    CancelledEditionNumber = 0
                },
                new EssDataInner
                {
                    ProductName = "DE521900",
                    BaseCellIssueDate = new DateTime(2017, 1, 13),
                    BaseCellEditionNumber = 0,
                    IssueDateLatestUpdate = new DateTime(2021, 1, 6),
                    LatestUpdateNumber = 3,
                    FileSize = 400,
                    CellLimitSouthernmostLatitude = (decimal?) 53.3259999,
                    CellLimitWesternmostLatitude = (decimal?) 7.1703318,
                    CellLimitNorthernmostLatitude = (decimal?) 53.3701674,
                    CellLimitEasternmostLatitude = (decimal?) 7.2286667,
                    DataCoverageCoordinates = new List<DataCoverageCoordinate>(),
                    Compression = true,
                    Encryption = true,
                    BaseCellUpdateNumber = 0,
                    LastUpdateNumberPreviousEdition = null,
                    BaseCellLocation = "M2;B8",
                    CancelledCellReplacements = new List<string>(),
                    IssueDatePreviousUpdate = new DateTime(2017, 1, 13),
                    CancelledEditionNumber = 6
                }
            });
        }


    }
}