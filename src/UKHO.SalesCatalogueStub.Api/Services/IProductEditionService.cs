#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.SalesCatalogueStub.Api.Models;

namespace UKHO.SalesCatalogueStub.Api.Services
{
    public interface IProductEditionService
    {
        Task<ProductResponse> GetProductIdentifiers(List<string> products);
        Task<ProductResponse> GetProductEditionsSinceDateTime(DateTime sinceDateTime);
        Task<(ProductResponse, GetProductVersionResponseEnum)> GetProductVersions(ProductVersions productVersions);
        Task<EssData> GetCatalogue();
        Task<(bool isModified, DateTime? dateEntered)> CheckIfCatalogueModified(DateTime? ifModifiedSince);
    }
}