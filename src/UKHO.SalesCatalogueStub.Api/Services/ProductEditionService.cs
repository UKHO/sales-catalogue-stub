﻿#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.EF.Models;
using UKHO.SalesCatalogueStub.Api.Models;

namespace UKHO.SalesCatalogueStub.Api.Services
{
    public class ProductEditionService : IProductEditionService
    {
        private readonly SalesCatalogueStubDbContext _dbContext;
        private readonly ILogger _logger;

        private readonly List<ProductEditionStatusEnum> _allowedProductStatus = new List<ProductEditionStatusEnum>
        {
            ProductEditionStatusEnum.Base, ProductEditionStatusEnum.Updated, ProductEditionStatusEnum.Reissued,
            ProductEditionStatusEnum.Cancelled
        };

        private readonly List<ProductEditionStatusEnum> _lifecycleEventTypes = new List<ProductEditionStatusEnum>
        {
            ProductEditionStatusEnum.Base, ProductEditionStatusEnum.Updated, ProductEditionStatusEnum.Reissued,
            ProductEditionStatusEnum.Cancelled, ProductEditionStatusEnum.Superseded
        };

        public ProductEditionService(SalesCatalogueStubDbContext dbContext, ILogger<ProductEditionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProductResponse> GetProductIdentifiers(List<string> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var productResponse = new ProductResponse
            {
                Products = new Products(),
                ProductCounts = new ProductCounts
                {
                    ReturnedProductCount = 0,
                    RequestedProductCount = products.Count,
                    RequestedProductsAlreadyUpToDateCount = 0,
                    RequestedProductsNotReturned = new List<RequestedProductsNotReturned>()
                }
            };

            var distinctProducts = products
                .GroupBy(item => item?.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g?.Key)
                .ToList();

            var matchedProducts = new Products();

            foreach (var product in distinctProducts)
            {
                if (string.IsNullOrWhiteSpace(product))
                {
                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(new RequestedProductsNotReturned
                    {
                        ProductName = string.Empty,
                        Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                    });
                    continue;
                }

                var activeEdition = await _dbContext.ProductEditions.AsNoTracking().SingleOrDefaultAsync(a =>
                    a.EditionIdentifier == product &&
                    a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                    _allowedProductStatus.Contains(a.LatestStatus));

                if (activeEdition != null)
                {
                    var activeEditionUpdateNumber = activeEdition.UpdateNumber ?? 0;
                    var updates = GetUpdates(activeEdition.LastReissueUpdateNumber ?? 0, activeEditionUpdateNumber);
                    var productsInner = new ProductsInner
                    {
                        UpdateNumbers = updates,
                        EditionNumber = activeEdition.EditionNumberAsInt,
                        FileSize = GetFileSize(activeEditionUpdateNumber),
                        ProductName = activeEdition.EditionIdentifier
                    };

                    if (activeEdition.LatestStatus == ProductEditionStatusEnum.Cancelled)
                    {
                        if (activeEdition.LastUpdateIssueDate.HasValue)
                        {
                            var productCancelledDate = activeEdition.LastUpdateIssueDate.Value;

                            var days = DateTime.Today.Subtract(productCancelledDate.Date).TotalDays;

                            if (days > 365)
                            {
                                // Old product, do not return with data; just add to the RequestedProductsNotReturned
                                productResponse.ProductCounts.RequestedProductsNotReturned.Add(
                                    new RequestedProductsNotReturned
                                    {
                                        ProductName = product,
                                        Reason = RequestedProductsNotReturned.ReasonEnum
                                            .NoDataAvailableForCancelledProductEnum
                                    });

                                continue;

                            }
                        }

                        productsInner.Cancellation = GetCancellation(activeEditionUpdateNumber);
                    }

                    productResponse.Products.Add(productsInner);
                    productResponse.ProductCounts.ReturnedProductCount++;
                }
                else
                {
                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(new RequestedProductsNotReturned
                    {
                        ProductName = product,
                        Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                    });
                    _logger.LogInformation(
                        $"{nameof(ProductEditionService)} no match found for product {product}");
                }
            }


            return productResponse;
        }

        public async Task<(ProductResponse, GetProductVersionResponseEnum)> GetProductVersions(ProductVersions productVersions)
        {
            if (productVersions == null) throw new ArgumentNullException(nameof(productVersions));

            var distinctProducts = new List<ProductVersionsInner>();
            var productResponse = new ProductResponse
            {
                Products = new Products(),
                ProductCounts = new ProductCounts
                {
                    ReturnedProductCount = 0,
                    RequestedProductCount = 0,
                    RequestedProductsAlreadyUpToDateCount = 0,
                    RequestedProductsNotReturned = new List<RequestedProductsNotReturned>()
                }
            };


            var validProductVersions = productVersions
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.ProductName)).ToList();

            var numberOfNull = productVersions.Count - validProductVersions.Count;

            for (var i = 0; i < numberOfNull; i++)
            {
                productResponse.ProductCounts.RequestedProductsNotReturned.Add(new RequestedProductsNotReturned
                { ProductName = string.Empty, Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum });
            }

            if (validProductVersions.Any())
            {
                distinctProducts = validProductVersions
                           .GroupBy(item => item.ProductName.Trim(),
                               StringComparer.OrdinalIgnoreCase)
                           .Select(g => g.First())
                           .ToList();
            }

            var productsInDatabase = false;


            productResponse.ProductCounts.RequestedProductCount = productVersions.Count;

            foreach (var requestProduct in distinctProducts)
            {

                if (requestProduct.UpdateNumber.HasValue && !requestProduct.EditionNumber.HasValue)
                {
                    // Product requested not in DB
                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(
                        new RequestedProductsNotReturned
                        {
                            ProductName = requestProduct.ProductName,
                            Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                        });

                    continue;
                }

                var productDbMatch = await _dbContext.ProductEditions.AsNoTracking().SingleOrDefaultAsync(a =>
                    a.EditionIdentifier == requestProduct.ProductName &&
                    a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                    _allowedProductStatus.Contains(a.LatestStatus));

                if (productDbMatch == null)
                {
                    // Product requested not in DB
                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(
                        new RequestedProductsNotReturned
                        {
                            ProductName = requestProduct.ProductName,
                            Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                        });

                    continue;
                }

                productsInDatabase = true;

                var activeEditionUpdateNumber = productDbMatch.UpdateNumber ?? 0;
                var requestUpdateNumber = requestProduct.UpdateNumber ?? 0;
                var requestEditionNumber = requestProduct.EditionNumber ?? 0;

                var matchedProduct = new ProductsInner
                {
                    EditionNumber = Convert.ToInt32(productDbMatch.EditionNumber),
                    FileSize = GetFileSize(activeEditionUpdateNumber),
                    ProductName = productDbMatch.EditionIdentifier
                };

                // Reject where status is Base and update zero is requested for current edition
                if (requestUpdateNumber == 0 &&
                    matchedProduct.EditionNumber == requestEditionNumber &&
                    productDbMatch.LatestStatus == ProductEditionStatusEnum.Base)
                {
                    productResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount++;

                    continue;
                }

                // Reject where the provided and current are the same
                if (productDbMatch.UpdateNumber == requestUpdateNumber &&
                    matchedProduct.EditionNumber == requestEditionNumber &&
                    productDbMatch.LatestStatus != ProductEditionStatusEnum.Cancelled)
                {
                    productResponse.ProductCounts.RequestedProductsAlreadyUpToDateCount++;

                    continue;
                }

                // Reject where edition or update numbers are provided that are higher than current
                if ((requestEditionNumber > matchedProduct.EditionNumber) ||
                    (requestEditionNumber == matchedProduct.EditionNumber &&
                     requestUpdateNumber > (activeEditionUpdateNumber)))
                {
                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(
                        new RequestedProductsNotReturned
                        {
                            ProductName = requestProduct.ProductName,
                            Reason = RequestedProductsNotReturned.ReasonEnum.InvalidProductEnum
                        });

                    continue;
                }

                int? start = 0;
                if (requestUpdateNumber == productDbMatch.LastReissueUpdateNumber)
                {
                    start = productDbMatch.LastReissueUpdateNumber + 1;
                }
                else
                {
                    start = (productDbMatch.LastReissueUpdateNumber > 0)
                    ? productDbMatch.LastReissueUpdateNumber
                    : !requestProduct.EditionNumber.HasValue || (requestProduct.EditionNumber < matchedProduct.EditionNumber)
                        ? 0 : requestProduct.UpdateNumber + 1 ?? 1;
                }

                var end = activeEditionUpdateNumber;

                switch (productDbMatch.LatestStatus)
                {
                    case ProductEditionStatusEnum.Cancelled:
                        {
                            if (productDbMatch.LastUpdateIssueDate.HasValue)
                            {
                                var productCancelledDate = productDbMatch.LastUpdateIssueDate.Value;

                                var days = DateTime.Today.Subtract(productCancelledDate.Date).TotalDays;

                                if (days > 365)
                                {
                                    // Old product, do not return with data; just add to the RequestedProductsNotReturned
                                    productResponse.ProductCounts.RequestedProductsNotReturned.Add(
                                        new RequestedProductsNotReturned
                                        {
                                            ProductName = requestProduct.ProductName,
                                            Reason = RequestedProductsNotReturned.ReasonEnum
                                                .NoDataAvailableForCancelledProductEnum
                                        });

                                    continue;

                                }
                            }

                            matchedProduct.Cancellation = new Cancellation
                            {
                                EditionNumber = 0,
                                UpdateNumber = activeEditionUpdateNumber + 1
                            };

                            if (requestEditionNumber == matchedProduct.EditionNumber &&
                                requestUpdateNumber == activeEditionUpdateNumber)
                            {
                                matchedProduct.UpdateNumbers = GetUpdates(start.Value, end);
                            }
                            else if (requestEditionNumber < matchedProduct.EditionNumber ||
                                     requestUpdateNumber < activeEditionUpdateNumber)
                            {
                                matchedProduct.UpdateNumbers = activeEditionUpdateNumber == 0
                                    ? new List<int?>()
                                    : GetUpdates(start.Value, end);
                            }

                            break;
                        }
                    case ProductEditionStatusEnum.Base:
                        matchedProduct.UpdateNumbers = new List<int?> { 0 };
                        break;
                    default:
                        matchedProduct.UpdateNumbers = GetUpdates(start.Value, end);
                        break;
                }

                productResponse.ProductCounts.ReturnedProductCount++;
                productResponse.Products.Add(matchedProduct);
            }

            if (!productResponse.Products.Any())
            {
                return productsInDatabase ? (productResponse, GetProductVersionResponseEnum.NoUpdatesFound) : (productResponse, GetProductVersionResponseEnum.NoProductsFound);
            }


            return (productResponse, GetProductVersionResponseEnum.UpdatesFound);
        }

        public async Task<(bool isModified, DateTime? dateEntered)> CheckIfCatalogueModified(DateTime? ifModifiedSince)
        {
            var latestLoaderStatus = await _dbContext.LoaderStatus.OrderByDescending(a => a.DateEntered).FirstAsync(a => a.AreaName == AreaNameEnum.Main);

            var dateEntered = latestLoaderStatus.DateEntered;
            return (ifModifiedSince == null || ifModifiedSince < dateEntered, dateEntered);
        }

        public async Task<EssData> GetCatalogue()
        {
            var editions = await _dbContext.ProductEditions.Include(a => a.PidGeometry).Include(a => a.PidTombstone).AsNoTracking().Where(a =>
                a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                _allowedProductStatus.Contains(a.LatestStatus)).ToListAsync();

            var catalogue = new List<EssDataInner>();

            foreach (var edition in editions)
            {
                var isCancelled = edition.LatestStatus == ProductEditionStatusEnum.Cancelled;

                var updateNumber = edition.UpdateNumber ?? 0;

                var reissueNumber = edition.LastReissueUpdateNumber ?? 0;

                Tuple<double?, double?, double?, double?> latitudes;

                int? baseCdNumber;

                if (isCancelled && edition.PidTombstone?.XmlData != null)
                {
                    var pidTombstone = DeserializePidTombstone(edition.PidTombstone.XmlData);
                    latitudes = new Tuple<double?, double?, double?, double?>(
                        pidTombstone.Metadata?.GeographicLimit?.BoundingBox?.NorthLimit,
                        pidTombstone.Metadata?.GeographicLimit?.BoundingBox?.EastLimit,
                        pidTombstone.Metadata?.GeographicLimit?.BoundingBox?.SouthLimit,
                        pidTombstone.Metadata?.GeographicLimit?.BoundingBox?.WestLimit);

                    baseCdNumber = pidTombstone.Metadata?.Cd?.Base;
                }
                else
                {
                    var geometry = edition.PidGeometry.SingleOrDefault(a => a.IsBoundingBox)?.Geom;

                    latitudes = new Tuple<double?, double?, double?, double?>(
                       geometry?.EnvelopeInternal?.MaxX,
                       geometry?.EnvelopeInternal?.MaxY,
                       geometry?.EnvelopeInternal?.MinX,
                       geometry?.EnvelopeInternal?.MinY);

                    baseCdNumber = edition.BaseCd;
                }

                catalogue.Add(
                    new EssDataInner()
                    {
                        ProductName = edition.EditionIdentifier,
                        BaseCellIssueDate = edition.BaseIssueDate,
                        BaseCellEditionNumber = isCancelled ? 0 : edition.EditionNumberAsInt,
                        IssueDateLatestUpdate = edition.LastUpdateIssueDate,
                        LatestUpdateNumber = edition.UpdateNumber,
                        FileSize = GetFileSize(updateNumber),
                        CellLimitNorthernmostLatitude = Convert.ToDecimal(latitudes.Item1),
                        CellLimitEasternmostLatitude = Convert.ToDecimal(latitudes.Item2),
                        CellLimitSouthernmostLatitude = Convert.ToDecimal(latitudes.Item3),
                        CellLimitWesternmostLatitude = Convert.ToDecimal(latitudes.Item4),
                        DataCoverageCoordinates = new List<DataCoverageCoordinate> { new DataCoverageCoordinate { Latitude = null, Longitude = null } },
                        Compression = true,
                        Encryption = true,
                        BaseCellUpdateNumber = reissueNumber,
                        LastUpdateNumberPreviousEdition = null,
                        CancelledCellReplacements = new List<string>(),
                        IssueDatePreviousUpdate = edition.BaseIssueDate,
                        CancelledEditionNumber = isCancelled ? edition.EditionNumberAsInt : (int?)null,
                        BaseCellLocation = GetBaseCellLocation(baseCdNumber)

                    });
            };

            var essData = new EssData();
            essData.AddRange(catalogue.OrderBy(a => a.ProductName));
            return essData;
        }

        public async Task<ProductResponse> GetProductEditionsSinceDateTime(DateTime sinceDateTime)
        {
            var productResponse = new ProductResponse
            {
                Products = new Products(),
                ProductCounts = new ProductCounts
                {
                    ReturnedProductCount = 0,
                    RequestedProductCount = 0,
                    RequestedProductsAlreadyUpToDateCount = 0,
                    RequestedProductsNotReturned = new List<RequestedProductsNotReturned>()
                }
            };

            var lifecycleEvents = await _dbContext.LifecycleEvents
                .Include(le => le.ProductEdition)
                .Include(le => le.EventType)
                .Where(le => le.LastUpdated > sinceDateTime &&
                             le.ProductEdition.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                             _lifecycleEventTypes.Contains(le.EventType.Name)
                )
                .AsNoTracking()
                .ToListAsync();

            var products = lifecycleEvents.Select(le => le.ProductEdition.EditionIdentifier).Distinct().ToList();

            foreach (var product in products)
            {
                var productEdition = new ProductsInner();

                var editionNumberAsInt = lifecycleEvents
                    .Where(le => le.ProductEdition.EditionIdentifier == product)
                    .Select(le => le.ProductEdition.EditionNumberAsInt)
                    .OrderByDescending(le => le)
                    .First();

                var relevantLifecycleEvents = lifecycleEvents
                    .Where(le => le.ProductEdition.EditionIdentifier == product && le.ProductEdition.EditionNumberAsInt == editionNumberAsInt)
                    .OrderByDescending(le => le.LastUpdated)
                    .ToList();

                if (relevantLifecycleEvents.First().EventType.Name == ProductEditionStatusEnum.Superseded)
                {
                    // this product will not be listed in the output
                    continue;
                }

                var activeEditionUpdateNumber = relevantLifecycleEvents.First().ProductEdition.UpdateNumber ?? 0;
                var activeEditionReissueNumber = relevantLifecycleEvents.First().ProductEdition.LastReissueUpdateNumber ?? 0;

                var updatedNumbers = new List<int?>();

                var updatedCount = activeEditionUpdateNumber;

                foreach (var lifecycleEvent in relevantLifecycleEvents)
                {
                    if (lifecycleEvent.EventType.Name == ProductEditionStatusEnum.Base)
                    {
                        updatedNumbers.Add(0);
                        break;
                    }

                    if (lifecycleEvent.EventType.Name == ProductEditionStatusEnum.Updated)
                    {
                        updatedNumbers.Add(updatedCount);
                        updatedCount--;
                        continue;
                    }

                    if (lifecycleEvent.EventType.Name == ProductEditionStatusEnum.Reissued)
                    {
                        updatedNumbers.Add(activeEditionReissueNumber);
                        break;
                    }
                }

                if (relevantLifecycleEvents.Any(le => le.EventType.Name == ProductEditionStatusEnum.Cancelled))
                {

                    productEdition.Cancellation = GetCancellation(activeEditionUpdateNumber);
                }

                productEdition.EditionNumber = editionNumberAsInt;
                productEdition.UpdateNumbers = updatedNumbers.OrderBy(gt => gt.Value).ToList();
                productEdition.ProductName = product;
                productEdition.FileSize = GetFileSize(activeEditionUpdateNumber);

                productResponse.Products.Add(productEdition);
                productResponse.ProductCounts.ReturnedProductCount++;

            }

            return productResponse;
        }

        private static List<int?> GetUpdates(int lastReissueUpdateNumber, int latestUpdateNumber)
        {
            var productUpdates = new List<int?>();

            for (var i = lastReissueUpdateNumber; i <= latestUpdateNumber; i++)
            {
                productUpdates.Add(i);
            }

            return productUpdates;
        }

        private static int GetFileSize(int latestUpdateNumber)
        {
            return 100 * (latestUpdateNumber + 1);
        }

        private static Cancellation GetCancellation(int latestUpdateNumber)
        {
            return new Cancellation()
            {
                EditionNumber = 0,
                UpdateNumber = latestUpdateNumber + 1
            };
        }

        private static string GetBaseCellLocation(int? baseCdNumber)
        {
            if (baseCdNumber == null)
            {
                return null;
            }

            return baseCdNumber <= 5 ? $"M1;B{baseCdNumber}" : $"M2;B{baseCdNumber}";
        }

        private static Enc DeserializePidTombstone(string xmlString)
        {
            var serializer =
                new XmlSerializer(typeof(Enc));

            Enc enc;

            using var stringReader = new StringReader(xmlString);
            {
                enc = (Enc)serializer.Deserialize(stringReader);
            }

            return enc;
        }
    }
}

