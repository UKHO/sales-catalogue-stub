namespace PostManRequests.Models
{
    public class TokenResponse

    { //This is our model for Post Response Properties etc
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }


    }
}