using System.Collections.Generic;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.EF.Repositories
{
    public interface IProductEditionRepository
    {
        List<ProductEdition> GetProductEditions(List<string> products);
    }
}