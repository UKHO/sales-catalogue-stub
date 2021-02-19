using System.Collections.Generic;
using UKHO.SalesCatalogueStub.EF.Models;

namespace UKHO.SalesCatalogueStub.EF.Repositories
{
    public interface IProductEditionRepository
    {
        List<ProductEdition> GetProductEditions(List<string> products);
    }
}