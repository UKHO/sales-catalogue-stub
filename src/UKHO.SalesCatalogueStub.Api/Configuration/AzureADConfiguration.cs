﻿using System.Diagnostics.CodeAnalysis;

namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    /// <summary>
    /// AzureADConfiguration: Getting connection setting for KV and App config
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureADConfiguration
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
