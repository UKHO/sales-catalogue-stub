using System.Diagnostics.CodeAnalysis;

namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EventHubLoggingConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string EntityPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EventHubConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MinimumLoggingLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string System { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UkhoMinimumLoggingLevel { get; set; }

    }
}
