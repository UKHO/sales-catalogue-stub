#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<ProductEditionStatusEnum> _allowedProductStatus = new List<ProductEditionStatusEnum> { ProductEditionStatusEnum.Base, ProductEditionStatusEnum.Updated, ProductEditionStatusEnum.Reissued, ProductEditionStatusEnum.Cancelled };

        public ProductEditionService(SalesCatalogueStubDbContext dbContext, ILogger<ProductEditionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<ProductEdition> GetProductEditions(List<string> products)
        {
            if (products == null) throw new ArgumentNullException(nameof(products));

            var distinctProducts = products
                .GroupBy(item => item.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key)
                .ToList();

            var matchedProducts = new List<ProductEdition>();

            foreach (var product in distinctProducts)
            {
                var productMatch = _dbContext.ProductEditions.AsNoTracking().SingleOrDefault(a =>
                    a.EditionIdentifier == product &&
                    a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                    _allowedProductStatus.Contains(a.LatestStatus));

                if (productMatch != null)
                {
                    matchedProducts.Add(new ProductEdition(productMatch.EditionIdentifier, productMatch.EditionNumber,
                        productMatch.LastReissueUpdateNumber ?? 0, productMatch.UpdateNumber ?? 0,
                        productMatch.LatestStatus));
                }
                else
                {
                    _logger.LogInformation($"{nameof(ProductEditionService)} no match, or duplicate entries found for product {product}");
                }
            }

            return matchedProducts;
        }
    }
}
