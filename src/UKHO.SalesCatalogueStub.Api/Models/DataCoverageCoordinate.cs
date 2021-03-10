using System.Runtime.Serialization;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class DataCoverageCoordinate
    {
        /// <summary>
        /// The edition number
        /// </summary>
        /// <value>The edition number</value>
        [DataMember(Name = "latitude")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// The update number, if applicable
        /// </summary>
        /// <value>The update number, if applicable</value>
        [DataMember(Name = "longitude")]
        public decimal? Longitude { get; set; }
    }
}
