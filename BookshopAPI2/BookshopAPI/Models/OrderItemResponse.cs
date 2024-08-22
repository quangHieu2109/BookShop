namespace BookshopAPI.Models
{
    public class OrderItemResponse
    {

        public long id { get; set; }
        public long orderId { get; set; }
        public Product product { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public int quantity { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
