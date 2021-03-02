#pragma warning disable 1591

using System.Collections.Generic;
using UKHO.SalesCatalogueStub.Api.Models;

namespace UKHO.SalesCatalogueStub.Api.Services
{
    public interface IProductEditionService
    {
        Products GetProductEditions(List<string> products);
    }
}