namespace BookshopAPI.Models
{
    public class JsonResponse
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string app_id { get; set; }
        public string type { get; set; }
        public string application { get; set; }
        public long data_access_expires_at { get; set; }
        public long expires_at { get; set; }
        public bool is_valid { get; set; }
        public long issued_at { get; set; }
        public Metadata metadata { get; set; }
        public string[] scopes { get; set; }
        public string user_id { get; set; }
    }

    public class Metadata
    {
        public string auth_type { get; set; }
        public string sso { get; set; }
    }
}
