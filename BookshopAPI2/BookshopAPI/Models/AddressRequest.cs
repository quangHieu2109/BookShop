namespace BookshopAPI.Models
{
    public class AddressRequest
    {
        public string? houseNumber { get; set; }
        public string? province { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
    }
}
