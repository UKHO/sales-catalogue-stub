#pragma warning disable 1591

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.EF.Models;
using UKHO.SalesCatalogueStub.Api.Models;
using Products = UKHO.SalesCatalogueStub.Api.Models.Products;

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

        public Products GetProductEditionsSinceDateTime(DateTime sinceDateTime)
        {
            var productsSinceDatetime = new Products();

            var lifecycleEvents = _dbContext.LifecycleEvents
                .Include(le => le.ProductEdition)
                .Include(le => le.EventType)
                .Where(le => le.LastUpdated > sinceDateTime &&
                             le.ProductEdition.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                             _lifecycleEventTypes.Contains(le.EventType.Name)
                //&& le.ProductEdition.EditionIdentifier == "jp34b5jk" //lastupdated: 2011-11-08 13:50:35.680
                )
                .AsNoTracking()
                .ToList();

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
                    // TODO: confirm with Kev that this product will not be listed in the output
                    continue;
                }

                var activeEditionUpdateNumber = relevantLifecycleEvents.First().ProductEdition.UpdateNumber ?? 0;
                var activeEditionReissueNumber = relevantLifecycleEvents.First().ProductEdition.LastReissueUpdateNumber ?? 0;

                var updatedNumbers = new List<int?>();

                var updatedCount = activeEditionUpdateNumber;

                for (int i = 0; i < relevantLifecycleEvents.Count; i++)
                {
                    var lifecycleEvent = relevantLifecycleEvents.ElementAt(i);

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

                    if (lifecycleEvent.EventType.Name == ProductEditionStatusEnum.Cancelled)
                    {
                        continue;
                    }
                }

                if (relevantLifecycleEvents.Any(le => le.EventType.Name == ProductEditionStatusEnum.Cancelled))
                {
                    var currentUpdateNumber = updatedNumbers.Count > 0 ? updatedNumbers.Max(l => l).Value : 0;
                    productEdition.Cancellation = GetCancellation(currentUpdateNumber);
                }

                productEdition.EditionNumber = editionNumberAsInt;
                productEdition.UpdateNumbers = updatedNumbers.OrderBy(gt => gt.Value).ToList();
                productEdition.ProductName = product;

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

