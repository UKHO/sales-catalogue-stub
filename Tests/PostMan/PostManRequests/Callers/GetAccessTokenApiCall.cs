using PostManRequests.Models;
using RestSharp;

namespace PostManRequests.Callers
{
    public class GetAccessTokenApiCall
    {
        private readonly string _configurationClientId;
        private readonly string _configurationClientSecret;
        private readonly string _configurationCookie;
        private readonly string _configurationScope;
        private readonly string _configurationTokenUrl;

        public GetAccessTokenApiCall(string configurationClientId, string configurationClientSecret, string configurationCookie, string configurationScope, string configurationTokenUrl)
        {
            _configurationClientId = configurationClientId;
            _configurationClientSecret = configurationClientSecret;
            _configurationCookie = configurationCookie;
            _configurationScope = configurationScope;
            _configurationTokenUrl = configurationTokenUrl;
        }

        public string GetAcessToken()
        {
            var client =
                new RestClient(
                    _configurationTokenUrl);
            client.Timeout = -1;
            var request = new RestRequest();
            request.AddHeader("Cookie",
                _configurationCookie);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _configurationClientId);
            request.AddParameter("client_secret", _configurationClientSecret);
            request.AddParameter("scope", _configurationScope);
            //IRestResponse response = client.Execute(request);
            //var deserialized = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
            ////returns the deserialized string that contains the access_token property 
            var response = client.Post<TokenResponse>(request);
            return response.Data.access_token;
            //return deserialized.access_token;
        }
    }
}