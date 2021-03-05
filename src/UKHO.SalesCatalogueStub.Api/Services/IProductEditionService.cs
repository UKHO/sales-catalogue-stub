#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.SalesCatalogueStub.Api.Models;

namespace UKHO.SalesCatalogueStub.Api.Services
{
    public interface IProductEditionService
    {
        Products GetProductEditions(List<string> products);
        Task<Products> GetProductEditionsSinceDateTime(DateTime sinceDateTime);
    }
}