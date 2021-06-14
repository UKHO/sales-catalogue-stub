using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PostManRequests.Callers;
using PostManRequests.Models;

namespace PostManTests
{
    public class GetProductsTests

    {
        private PostManTestsConfig configuration;
        private GetAccessTokenApiCall _newaccesstoken;

        [SetUp]
        public void Setup()

        {
            configuration = ConfigBuilder.GetApplicationConfiguration(TestContext.CurrentContext.TestDirectory);
            _newaccesstoken = new GetAccessTokenApiCall(configuration.ClientId, configuration.ClientSecret,
                configuration.Cookie, configuration.Scope, configuration.TokenUrl);
        }

        [Test]
        public async Task Test_The_Get_Product_With_An_Invalid_Token_Returns_Unauthorised()
        {
            GetProductApiCall newgetproduct = new GetProductApiCall();

            var productUri =
                newgetproduct.GetProductUri(configuration.SiteBaseUrl, DateTime.Now - TimeSpan.FromDays(14));

            (ProductResponse ProductResponse, HttpStatusCode StatusCode) result =
                await newgetproduct.GetProductHttpClientAsync("InvalidToken", productUri);
            result.ProductResponse.Should().BeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Test_The_Get_Product_With_An_Old_Token_Return_Unauthorised()
        {
            GetProductApiCall newgetproduct = new GetProductApiCall();
            var productUri =
                newgetproduct.GetProductUri(configuration.SiteBaseUrl, DateTime.Now - TimeSpan.FromDays(14));
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) result =
                await newgetproduct.GetProductHttpClientAsync(
                    "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiJmZGFhNDc0Zi00OWE4LTQyNTYtODQ1Zi1kYzJjMTY0NTM1ZWMiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjIxMjQ3NTY5LCJuYmYiOjE2MjEyNDc1NjksImV4cCI6MTYyMTI1MTQ2OSwiYWlvIjoiRTJaZ1lBaFZxWmdsS2xLNC9mUFpxYnJWdXJzMkFRQT0iLCJhcHBpZCI6ImIyYjQ4YWI5LTYyYWItNGI3YS05YzU2LTczZGRiMjE1ZDViMyIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzkxMzRjYTQ4LTY2M2QtNGEwNS05NjhhLTMxYTQyZjBhZWQzZS8iLCJvaWQiOiI4ZDQ1ZWNjYi05ODk3LTQ2NGMtYmJlNS1mMDExMDc2MzVhZDAiLCJyaCI6IjAuQVFJQVNNbzBrVDFtQlVxV2lqR2tMd3J0UHJtS3RMS3JZbnBMbkZaejNiSVYxYk1DQUFBLiIsInJvbGVzIjpbIkNhdGFsb2d1ZVJlYWRlciIsIldlYmhvb2tDYWxsZXIiLCJFeGNoYW5nZVNlcnZpY2VSZWFkZXIiXSwic3ViIjoiOGQ0NWVjY2ItOTg5Ny00NjRjLWJiZTUtZjAxMTA3NjM1YWQwIiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidXRpIjoiV25TMEgyLWNHa0cyQjRzZGtBOHlBQSIsInZlciI6IjEuMCJ9.acIUiTiqgxplb_GuTUo73Ched1aHXzHPc8VZ2J9bBxJD-NKhZeLoLTCMx1W9EHRQXlGrjMPwBA9keq6RTbg38DcSL3Oq3xFqOV734_FMaGrQl5gpSWbFs7wNiZEetMuDsmP1uFlLlVCugY1YiSbucm-uZz5753BryAzfu6dIzdRiKf-ifWHatjU5NgSD5G4yEYYFALbjZdAuZGMee-KtameKZ2HGUiMhXPhMGeZMiZZRazk22iTNxqwYrapr2yvfU8ikf3wRFr0vYxwcDTrhEdmSF-QBn5aYPJBqO7PwBF_P89Liis7lM4byzLYQqWdXoOMJAvHccxN8ElJsJ0H3AA",
                    productUri);
            result.ProductResponse.Should().BeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Test_The_Get_Product_Returns_Status_Code_OK()
        {
            string tokenresult = _newaccesstoken.GetAcessToken();
            GetProductApiCall newuri = new GetProductApiCall();
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetProductHttpClientAsync(tokenresult, productUri);
            uriresult.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Test_The_Get_Product_Returns_Same_Amount_Of_Products_In_Product_Count()
        {
            string tokenresult = _newaccesstoken.GetAcessToken();
            GetProductApiCall newuri = new GetProductApiCall();
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetProductHttpClientAsync(tokenresult, productUri);
            uriresult.StatusCode.Should().Be(HttpStatusCode.OK);
            int count = uriresult.ProductResponse.products.Count;
            uriresult.ProductResponse.productCounts.returnedProductCount.Should().Be(count);
        }

        [Test]
        public async Task Test_The_Get_Product_Request_With_No_Dates_Returns_Bad_Request()
        {
            string tokenresult = _newaccesstoken.GetAcessToken();
            GetProductApiCall newuri = new GetProductApiCall();
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetProductHttpClientAsync(tokenresult,
                    $"{configuration.SiteBaseUrl}/v1/productData/AVCS/products?sinceDateTime=");
            uriresult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_The_Get_Product_Request_With_Incorrect_Date_Format_Returns_Bad_Request()
        {
            string tokenresult = _newaccesstoken.GetAcessToken();
            GetProductApiCall newuri = new GetProductApiCall();
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetProductHttpClientAsync(tokenresult,
                    $"{configuration.SiteBaseUrl}/v1/productData/AVCS/products?sinceDateTime=BLAH");
            uriresult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_The_Get_Product_Request_With_Bad_Token_Returns_Unauthorised()
        {
            GetProductApiCall newuri = new GetProductApiCall();

            (ProductResponse ProductResponse, HttpStatusCode StatusCode) uriresult =
                await newuri.GetProductHttpClientAsync("InvalidToken", productUri);
            uriresult.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Test_The_Get_Product_Product_Counts_Returns_A_200OK_And_Least_One_Product()
        {
            string tokenResult = _newaccesstoken.GetAcessToken();
            GetProductApiCall newgetproduct = new GetProductApiCall();
            (ProductResponse ProductResponse, HttpStatusCode StatusCode) result =
                await newgetproduct.GetProductHttpClientAsync(tokenResult, productUri);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.ProductResponse.productCounts.returnedProductCount.Should().BeGreaterThan(1);
            result.ProductResponse.products.Should().NotBeNullOrEmpty();
            result.ProductResponse.products.Should().OnlyContain(cell => cell.fileSize >= 0);
            result.ProductResponse.products.Should().OnlyContain(cell => cell.fileSize <= 10000);
        }
    }
}