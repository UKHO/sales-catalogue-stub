using Newtonsoft.Json;
using System.Collections.Generic;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductEditionDto
    {
        private readonly string _editionNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="editionNumber"></param>
        /// <param name="reissueUpdateNumber"></param>
        /// <param name="latestUpdateNumber"></param>
        /// <param name="latestStatus"></param>
        public ProductEditionDto(string productName, string editionNumber, int reissueUpdateNumber, int latestUpdateNumber, ProductEditionStatusEnum latestStatus)
        {
            ProductName = productName;
            _editionNumber = editionNumber;
            ReissueUpdateNumber = reissueUpdateNumber;
            LatestUpdateNumber = latestUpdateNumber;
            Status = latestStatus;

        }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "productName")]

        public string ProductName { get; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "editionNumber")]
        public string EditionNumber => Status == ProductEditionStatusEnum.Cancelled ? "0" : _editionNumber;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "updateNumber")]
        public List<int> UpdateNumber => CalculateProductUpdates();

        /// <summary>
        /// 
        /// </summary>
        private int ReissueUpdateNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        private int LatestUpdateNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        private ProductEditionStatusEnum Status { get; }


        private List<int> CalculateProductUpdates()
        {
            var productUpdates = new List<int>();

            var upperLimit = LatestUpdateNumber;
            var lowerLimit = ReissueUpdateNumber == 0 ? ReissueUpdateNumber + 1 : ReissueUpdateNumber;

            for (var i = lowerLimit; i <= upperLimit; i++)
            {
                productUpdates.Add(i);
            }

            return productUpdates;
        }
    }

}
