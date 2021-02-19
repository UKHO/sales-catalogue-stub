using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.SalesCatalogueStub.EF.Models;

namespace UKHO.SalesCatalogueStub.EF.Repositories
{
    public class ProductEditionRepository : IProductEditionRepository
    {
        private readonly SalesCatalogueStubDbContext _dbContext;
        private readonly List<ProductEditionStatusEnum> _allowedProductStatus = new List<ProductEditionStatusEnum> { ProductEditionStatusEnum.Base, ProductEditionStatusEnum.Updated, ProductEditionStatusEnum.Reissued, ProductEditionStatusEnum.Cancelled };

        public ProductEditionRepository(SalesCatalogueStubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProductEdition> GetProductEditions(List<string> products)
        {
            if (products == null) throw new ArgumentNullException(nameof(products));

            var distinctProducts = products
                .GroupBy(item => item.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key)
                .ToList();

            return distinctProducts
                .Select(product => _dbContext.ProductEditions.Single(a =>
                    a.EditionIdentifier == product &&
                    a.Product.ProductType.Name == ProductTypeNameEnum.Avcs &&
                    _allowedProductStatus.Contains(a.LatestStatus))).Select(productEdition =>
                    new ProductEdition(productEdition.EditionIdentifier, productEdition.EditionNumber,
                        productEdition.LastReissueUpdateNumber ?? 0, productEdition.UpdateNumber ?? 0,
                        productEdition.LatestStatus)).ToList();
        }
    }
}
