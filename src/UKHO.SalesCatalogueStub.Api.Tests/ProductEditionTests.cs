using NUnit.Framework;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ProductEditionTests
    {
        [TestCase(1, 0, ExpectedResult = 1)]
        [TestCase(2, 0, ExpectedResult = 2)]
        [TestCase(5, 0, ExpectedResult = 5)]
        [TestCase(10, 0, ExpectedResult = 10)]
        [TestCase(100, 0, ExpectedResult = 100)]
        public int Product_Editions_With_No_Reissue_Return_Correct_Updates_Count(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEdition("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.Count;
        }

        [TestCase(1, 5, ExpectedResult = 4)]
        [TestCase(2, 10, ExpectedResult = 8)]
        [TestCase(5, 8, ExpectedResult = 5)]
        [TestCase(10, 25, ExpectedResult = 16)]
        [TestCase(100, 125, ExpectedResult = 126)]
        public int Product_Editions_With_Reissue_Return_Correct_Updates_Count(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEdition("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.Count;
        }
    }
}