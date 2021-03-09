#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Products GetProductEditions(List<string> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            var distinctProducts = products
                .GroupBy(item => item?.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g?.Key)
                .ToList();

            var matchedProducts = new Products();

            foreach (var product in distinctProducts)
            {
                if (product == null)
                {
                    continue;
                }

                var activeEdition = GetActiveEdition(product);

                if (activeEdition != null)
                {
                    var activeEditionUpdateNumber = activeEdition.UpdateNumber ?? 0;
                    var updates = GetUpdates(activeEdition.LastReissueUpdateNumber ?? 0, activeEditionUpdateNumber);
                    var productsInner = new ProductsInner
                    {
                        UpdateNumbers = updates,
                        EditionNumber = Convert.ToInt32(activeEdition.EditionNumber),
                        FileSize = GetFileSize(activeEditionUpdateNumber),
                        ProductName = activeEdition.EditionIdentifier
                    };

                    if (activeEdition.LatestStatus == ProductEditionStatusEnum.Cancelled)
                    {
                        productsInner.Cancellation = GetCancellation(activeEditionUpdateNumber);
                    }

                    matchedProducts.Add(productsInner);
                }
                else
                {
                    _logger.LogInformation(
                        $"{nameof(ProductEditionService)} no match found for product {product}");
                }
            }

            return matchedProducts;
        }

        public async Task<(Products, GetProductVersionResponseEnum)> GetProductVersions(ProductVersions productVersions)
        {
            if (productVersions == null) throw new ArgumentNullException(nameof(productVersions));

            var distinctProducts = new List<ProductVersionsInner>();

            var validProductVersions = productVersions
                .Where(x => x != null && x.ProductName != null).ToList();

            if (validProductVersions.Any())
            {
                distinctProducts = validProductVersions
                           .GroupBy(item => item.ProductName.Trim(),
                               StringComparer.OrdinalIgnoreCase)
                           .Select(g => g.First())
                           .ToList();
            }

            var productsInDatabase = false;
            var matchedProducts = new Products();

            foreach (var requestProduct in distinctProducts)
            {
                if (requestProduct.UpdateNumber.HasValue && !requestProduct.EditionNumber.HasValue) continue;

                var productDbMatch = await _dbContext.ProductEditions.AsNoTracking().SingleOrDefaultAsync(a =>
                    a.EditionIdentifier == requestProduct.ProductName &&
                    a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                    _allowedProductStatus.Contains(a.LatestStatus));

                if (productDbMatch == null) continue;

                productsInDatabase = true;

                // Reject where update number is provided without an edition number
                if ((productDbMatch.EditionNumber == null) &&
                    (productDbMatch.UpdateNumber != null))
                    continue;

                var activeEditionUpdateNumber = productDbMatch.UpdateNumber ?? 0;

                var matchedProduct = new ProductsInner
                {
                    EditionNumber = Convert.ToInt32(productDbMatch.EditionNumber),
                    FileSize = GetFileSize(activeEditionUpdateNumber),
                    ProductName = productDbMatch.EditionIdentifier
                };

                // Reject where status is Base and update zero is requested for current edition
                if (requestProduct.UpdateNumber == 0 &&
                    matchedProduct.EditionNumber == requestProduct.EditionNumber &&
                    productDbMatch.LatestStatus == ProductEditionStatusEnum.Base)
                    continue;

                // Reject where the provided and current are the same
                if (productDbMatch.UpdateNumber == requestProduct.UpdateNumber &&
                    matchedProduct.EditionNumber == requestProduct.EditionNumber &&
                    productDbMatch.LatestStatus != ProductEditionStatusEnum.Cancelled)
                    continue;

                // Reject where edition or update numbers are provided that are higher than current
                if ((requestProduct.EditionNumber > matchedProduct.EditionNumber) ||
                    (requestProduct.EditionNumber == matchedProduct.EditionNumber &&
                     requestProduct.UpdateNumber > (productDbMatch.UpdateNumber ?? 0)))
                    continue;

                var start = (productDbMatch.LastReissueUpdateNumber > 0)
                    ? productDbMatch.LastReissueUpdateNumber
                    : !requestProduct.EditionNumber.HasValue || (requestProduct.EditionNumber < matchedProduct.EditionNumber)
                        ? 0 : requestProduct.UpdateNumber + 1 ?? 1;

                var end = activeEditionUpdateNumber;

                switch (productDbMatch.LatestStatus)
                {
                    case ProductEditionStatusEnum.Cancelled:
                        {
                            var updateNumber = requestProduct.UpdateNumber ?? 0;

                            matchedProduct.Cancellation = new Cancellation
                            {
                                EditionNumber = 0
                            };

                            if (requestProduct.EditionNumber == matchedProduct.EditionNumber && updateNumber == activeEditionUpdateNumber)
                            {
                                matchedProduct.UpdateNumbers = GetUpdates(start.Value, end);
                                matchedProduct.Cancellation.UpdateNumber = activeEditionUpdateNumber + 1;
                                matchedProduct.EditionNumber = null;
                            }
                            else if (requestProduct.EditionNumber < matchedProduct.EditionNumber || requestProduct.UpdateNumber < activeEditionUpdateNumber)
                            {
                                matchedProduct.UpdateNumbers = GetUpdates(start.Value, end);
                                matchedProduct.Cancellation.UpdateNumber = activeEditionUpdateNumber + 1;
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

                matchedProducts.Add(matchedProduct);
            }

            if (matchedProducts.Count == 0)
            {
                return productsInDatabase ? (matchedProducts, GetProductVersionResponseEnum.NoUpdatesFound) : (matchedProducts, GetProductVersionResponseEnum.NoProductsFound);
            }

            return (matchedProducts, GetProductVersionResponseEnum.UpdatesFound);
        }

        public async Task<Products> GetProductEditionsSinceDateTime(DateTime sinceDateTime)
        {
            var productsSinceDatetime = new Products();

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

                productsSinceDatetime.Add(productEdition);

            }

            return productsSinceDatetime;
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

        private ProductEdition GetActiveEdition(string productName)
        {
            var productMatch = _dbContext.ProductEditions.AsNoTracking().SingleOrDefault(a =>
                a.EditionIdentifier == productName &&
                a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                _allowedProductStatus.Contains(a.LatestStatus));

            return productMatch;
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
    }
}

