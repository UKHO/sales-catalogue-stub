#pragma warning disable 1591

using System.Diagnostics.CodeAnalysis;

namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EventHubLoggingConfig
    {
        public string EntityPath { get; set; }

        public string Environment { get; set; }

        public string EventHubConnectionString { get; set; }

        public string MinimumLoggingLevel { get; set; }

        public string NodeName { get; set; }

        public string Service { get; set; }

        public string System { get; set; }

        public string UkhoMinimumLoggingLevel { get; set; }
    }
}
