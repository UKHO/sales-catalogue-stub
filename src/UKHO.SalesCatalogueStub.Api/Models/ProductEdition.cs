#pragma warning disable 1591

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    public class ProductEdition
    {
        public ProductEdition(string productName, int editionNumber, int reissueUpdateNumber, int latestUpdateNumber, ProductEditionStatusEnum latestStatus)
        {
            ProductName = productName;
            ReissueUpdateNumber = reissueUpdateNumber;
            LatestUpdateNumber = latestUpdateNumber;
            Status = latestStatus;
            EditionNumber = Status == ProductEditionStatusEnum.Cancelled ? 0 : editionNumber;
        }

        [JsonProperty(PropertyName = "productName")]

        public string ProductName { get; }

        [JsonProperty(PropertyName = "editionNumber")]
        public int EditionNumber { get; }

        [JsonProperty(PropertyName = "updateNumbers")]
        public List<int> UpdateNumbers => CalculateProductUpdates();

        private int ReissueUpdateNumber { get; }

        private int LatestUpdateNumber { get; }

        private ProductEditionStatusEnum Status { get; }


        private List<int> CalculateProductUpdates()
        {
            var lowerLimit = ReissueUpdateNumber == 0 ? ReissueUpdateNumber + 1 : ReissueUpdateNumber;
            return Enumerable.Range(lowerLimit, LatestUpdateNumber - lowerLimit + 1).ToList();
        }
    }
}
