using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostManRequests.Models;

namespace PostManRequests.Callers
{
    public class GetProductApiCall
    {
        //defined Tuple here T1 & T2
        public async Task<(ProductResponse Response, HttpStatusCode StatusCode)> GetProductHttpClientAsync(
            string tokenResult,
            string getProductUri)
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResult}");
                response = await client.GetAsync(getProductUri);
            }

            // 2 Tuple Results passed back
            string responseContent = await response.Content.ReadAsStringAsync();
            ProductResponse result = JsonConvert.DeserializeObject<ProductResponse>(responseContent);
            return (result, response.StatusCode);
        }
    }
}