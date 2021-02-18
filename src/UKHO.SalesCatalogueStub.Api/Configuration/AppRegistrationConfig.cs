using System.Diagnostics.CodeAnalysis;

namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AppRegistrationConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string MicrosoftOnlineLoginUrl { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string ClientId { get; set; }
    }
}
