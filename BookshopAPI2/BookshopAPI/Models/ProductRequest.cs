namespace BookshopAPI.Models
{
    public class ProductRequest
    {
        public string name { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public int quantity { get; set; }
        public string author { get; set; }
        public int pages { get; set; }
        public string publisher { get; set; }
        public int yearPublishing { get; set; }
        public string description { get; set; }
        public string imageName { get; set; }
        public int shop { get; set; }
        public DateTime createdAt { get; set; }
    }
}
