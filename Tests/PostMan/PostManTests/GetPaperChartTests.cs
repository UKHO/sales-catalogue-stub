using FluentAssertions;
using NUnit.Framework;
using PostManRequests.Callers;
using PostManRequests.Models;
using System.Net;
using System.Threading.Tasks;

namespace PostManTests
{
    public class GetPaperChartTests

    {
        [SetUp]
        public void Setup()

        {
        }

        [Test]
        public async Task Test_The_Get_PaperChart_Returns_Status_Code_OK()
        {
            GetPaperChartApiCall newuri = new GetPaperChartApiCall();
            (PaperChartResponse paperChartResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetChartHttpClientAsync(
                    "http://dev1.api.engineering.ukho.gov.uk:80/pins-state-management/paperchart/getmay", "1552");
            uriresult.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}