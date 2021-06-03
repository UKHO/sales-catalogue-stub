using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using PostManRequests.Callers;
using PostManRequests.Models;

namespace PostManTests
{
    public class GetCatalogueTests
    {
        private PostManTestsConfig configuration;
        private GetAccessTokenApiCall _newaccesstoken;

        [SetUp]
        public void Setup()
        {
            configuration = ConfigBuilder.GetApplicationConfiguration(TestContext.CurrentContext.TestDirectory);
            _newaccesstoken = new GetAccessTokenApiCall(configuration.ClientId, configuration.ClientSecret, configuration.Cookie, configuration.Scope, configuration.TokenUrl);
        }

        [Test]
        public void Test_New_Access_Token_Is_Added_to_the_Token_Result_Variable()
        {
            //calling the GetAccesToken Method. Assign the response to var token result
            string tokenresult = _newaccesstoken.GetAcessToken();
            tokenresult.Should().NotBeEmpty();
        }

        [Test]
        public async Task Test_The_Get_Catalogue_With_A_Valid_Token_Returns_A_Valid_Response()
        {   //ToDo Add more Catalogue Tests
            GetCatalogueApiCall newgetcatalogue = new GetCatalogueApiCall();
            string tokenResult = _newaccesstoken.GetAcessToken();
            List<UpdatedCell> catalogueresult = await newgetcatalogue.GetCatalogueHttpClientAsync(tokenResult, configuration.SiteBaseUrl);
            catalogueresult.Should().NotBeNullOrEmpty();
            catalogueresult.Should().Contain(cell =>
                cell.productName == "1U319240" && cell.cellLimitSouthernmostLatitude == 24.0 &&
                cell.cellLimitWesternmostLatitude == 119.0 && (cell.cellLimitNorthernmostLatitude == 25.0) &
                (cell.cellLimitEasternmostLatitude == 120.0));
            catalogueresult.Should().OnlyContain(cell => cell.baseCellUpdateNumber >= 0);
            UpdatedCell returnoneproduct = catalogueresult.Single(cell => cell.productName == "1U421211");
            returnoneproduct.baseCellEditionNumber.Should().Be(2);
            returnoneproduct.baseCellIssueDate.Should().Be(9.March(2021));
            returnoneproduct.baseCellLocation.Should().Be("M1;B2");
            returnoneproduct.baseCellUpdateNumber.Should().Be(0);
            returnoneproduct.cancelledEditionNumber.Should().BeNull();
            returnoneproduct.cellLimitEasternmostLatitude.Should().Be(121.5F);
            returnoneproduct.cancelledCellReplacements.Should().BeEmpty();
            returnoneproduct.lastUpdateNumberPreviousEdition.Should().Be(null);
        }
    }
}