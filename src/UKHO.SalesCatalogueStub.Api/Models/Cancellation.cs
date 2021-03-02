using System.Runtime.Serialization;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class Cancellation
    {
        /// <summary>
        /// The edition number
        /// </summary>
        /// <value>The edition number</value>
        [DataMember(Name = "editionNumber")]
        public int? EditionNumber { get; set; }

        /// <summary>
        /// The update number, if applicable
        /// </summary>
        /// <value>The update number, if applicable</value>
        [DataMember(Name = "updateNumber")]
        public int? UpdateNumber { get; set; }
    }
}