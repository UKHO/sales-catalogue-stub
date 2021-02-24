using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.Tests
{
    public class ProductEditionTests
    {
        [Test]
        public void Product_Edition_With_Supplied_Params_Should_Initialize_Correctly()
        {
            var productEdition = new ProductEditionDto("TEST", "1", 0, 5, ProductEditionStatusEnum.Updated);
            productEdition.EditionNumber.Should().Be("1");
            productEdition.ProductName.Should().Be("TEST");
            productEdition.UpdateNumber.Should().ContainInOrder(new List<int> { 1, 2, 3, 4, 5 });
        }
        [TestCase(1, 0, ExpectedResult = 1)]
        [TestCase(2, 0, ExpectedResult = 2)]
        [TestCase(5, 0, ExpectedResult = 5)]
        [TestCase(10, 0, ExpectedResult = 10)]
        [TestCase(100, 0, ExpectedResult = 100)]
        public int Product_Editions_With_No_Reissue_Return_Correct_Updates_Count(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEditionDto("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.Count;
        }

        [TestCase(5, 2, ExpectedResult = 4)]
        [TestCase(15, 10, ExpectedResult = 6)]
        [TestCase(12, 8, ExpectedResult = 5)]
        [TestCase(35, 25, ExpectedResult = 11)]
        [TestCase(150, 125, ExpectedResult = 26)]
        public int Product_Editions_With_Reissue_Return_Correct_Updates_Count(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEditionDto("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.Count;
        }

        [TestCase(1, 0, ExpectedResult = new[] { 1 })]
        [TestCase(2, 0, ExpectedResult = new[] { 1, 2 })]
        [TestCase(5, 0, ExpectedResult = new[] { 1, 2, 3, 4, 5 })]
        [TestCase(10, 0, ExpectedResult = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [TestCase(20, 0, ExpectedResult = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 })]
        public int[] Product_Editions_With_No_Reissue_Return_Correct_Updates_Array(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEditionDto("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.ToArray();
        }

        [TestCase(2, 1, ExpectedResult = new[] { 1, 2 })]
        [TestCase(5, 3, ExpectedResult = new[] { 3, 4, 5 })]
        [TestCase(10, 6, ExpectedResult = new[] { 6, 7, 8, 9, 10 })]
        [TestCase(15, 11, ExpectedResult = new[] { 11, 12, 13, 14, 15 })]
        [TestCase(20, 15, ExpectedResult = new[] { 15, 16, 17, 18, 19, 20 })]
        public int[] Product_Editions_With_Reissue_Return_Correct_Updates_Array(int latestUpdateNumber, int reissueNumber)
        {
            var productEdition = new ProductEditionDto("TEST", "1", reissueNumber, latestUpdateNumber, ProductEditionStatusEnum.Updated);
            return productEdition.UpdateNumber.ToArray();
        }

        [Test]
        public void Product_Edition_With_Status_Cancelled_Should_Have_Edition_Zero()
        {
            var productEdition = new ProductEditionDto("TEST", "1", 0, 1, ProductEditionStatusEnum.Cancelled);
            productEdition.EditionNumber.Should().Be("0");
        }


    }
}