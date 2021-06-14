using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostManRequests.Models;

namespace PostManRequests.Callers
{
    public class GetPaperChartApiCall

    {
        public async Task<(PaperChartResponse Response, HttpStatusCode StatusCode)> GetChartHttpClientAsync(
            string siteBaseUrl,
            string productNum)

        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                response = await client.GetAsync(
                    $"{siteBaseUrl}http://dev1.api.engineering.ukho.gov.uk:80/pins-state-management/paperchart/getmay/{productNum}");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            PaperChartResponse result = JsonConvert.DeserializeObject<PaperChartResponse>(responseContent);
            return (result, response.StatusCode);
        }
    }
}