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
        Task<ProductResponse> GetProductEditionsSinceDateTime(DateTime sinceDateTime);
        Task<(Products, GetProductVersionResponseEnum)> GetProductVersions(ProductVersions productVersions);
        EssData GetCatalogue(DateTime? ifModifiedSince);
    }
}