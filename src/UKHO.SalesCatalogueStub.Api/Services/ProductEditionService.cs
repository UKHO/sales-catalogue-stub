﻿#pragma warning disable 1591

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

        public ProductEditionService(SalesCatalogueStubDbContext dbContext, ILogger<ProductEditionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Products GetProductEditions(List<string> products)
        {
            if (products == null) throw new ArgumentNullException(nameof(products));

            var distinctProducts = products
                .GroupBy(item => item.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key)
                .ToList();

            var matchedProducts = new Products();

            foreach (var product in distinctProducts)
            {
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
                        $"{nameof(ProductEditionService)} no match, or duplicate entries found for product {product}");
                }
            }

            return matchedProducts;
        }

        private static List<int> GetUpdates(int lastReissueUpdateNumber, int latestUpdateNumber)
        {
            return Enumerable.Range(lastReissueUpdateNumber, latestUpdateNumber - lastReissueUpdateNumber + 1).ToList();
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
