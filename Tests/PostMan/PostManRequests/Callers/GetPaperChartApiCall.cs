using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostManRequests.Models;

namespace PostManRequests.Callers
{
    public class GetPaperChartApiCall

    {
        public async Task<(GetPaperChartResponse Response, HttpStatusCode StatusCode)> GetChartHttpClientAsync(
            string getPaperChartUri,
            string chartNum
        )
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {chartNum}");
                response = await client.GetAsync(getPaperChartUri);
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            GetPaperChartResponse result = JsonConvert.DeserializeObject<GetPaperChartResponse>(responseContent);
            return (result, response.StatusCode);
        }
    }
}