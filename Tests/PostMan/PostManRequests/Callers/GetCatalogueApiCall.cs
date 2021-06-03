using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostManRequests.Models;

namespace PostManRequests.Callers
{
    public class GetCatalogueApiCall
    {
        public async Task<List<UpdatedCell>> GetCatalogueHttpClientAsync(string tokenResult, string siteBaseUrl)
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("IfModifiedSince", "3/10/2021 12:00:00 AM");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResult}");
                response = await client.GetAsync(
                    $"{siteBaseUrl}/v1/productData/AVCS/catalogue/EssData");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            List<UpdatedCell> result = JsonConvert.DeserializeObject<List<UpdatedCell>>(responseContent);
            return result;
        }
    }
}