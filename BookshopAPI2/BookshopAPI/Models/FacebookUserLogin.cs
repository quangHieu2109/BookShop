namespace BookshopAPI.Models
{
    public class FacebookUserLogin
    {
        public string inputToken { get; set; }
        public string accessToken { get; set; }
        public string name { get; set; }
        public string uid { get; set; }
        public string email { get; set; }

        public string phoneNumber { get; set; }

    }
}