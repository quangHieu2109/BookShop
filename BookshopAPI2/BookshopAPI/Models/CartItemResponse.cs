namespace BookshopAPI.Models
{
    public class CartItemResponse
    {
        public long id { get; set; }
        public long cartId { get; set; }
        public Product product { get; set; }
        public int quantity { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
