namespace UKHO.SalesCatalogueStub.Api.Configuration
{
    /// <summary>
    /// Get PID Database config from key vault
    /// </summary>
    public class PidDatabaseConfig
    {
        /// <summary>
        /// Server name
        /// </summary>
        public string ServerInstance { get; set; }
        
        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }    
    }
}
