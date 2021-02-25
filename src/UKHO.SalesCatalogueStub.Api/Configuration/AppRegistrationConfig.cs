#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;

namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppRegistrationConfig
    {
        public string MicrosoftOnlineLoginUrl { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }
    }
}
