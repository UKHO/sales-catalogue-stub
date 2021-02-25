#pragma warning disable 1591

using System.Collections.Generic;
using Newtonsoft.Json;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary />
    public class ProductEdition
    {
        private readonly int _editionNumber;

        public ProductEdition(string productName, int editionNumber, int reissueUpdateNumber, int latestUpdateNumber, ProductEditionStatusEnum latestStatus)
        {
            ProductName = productName;
            _editionNumber = editionNumber;
            ReissueUpdateNumber = reissueUpdateNumber;
            LatestUpdateNumber = latestUpdateNumber;
            Status = latestStatus;

        }

        [JsonProperty(PropertyName = "productName")]

        public string ProductName { get; }

        [JsonProperty(PropertyName = "editionNumber")]
        public int EditionNumber => Status == ProductEditionStatusEnum.Cancelled ? 0 : _editionNumber;

        [JsonProperty(PropertyName = "updateNumber")]
        public List<int> UpdateNumber => CalculateProductUpdates();

        private int ReissueUpdateNumber { get; }

        private int LatestUpdateNumber { get; }

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
