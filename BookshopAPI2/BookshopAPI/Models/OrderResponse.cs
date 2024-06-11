namespace BookshopAPI.Models
{
    public class OrderResponse: Order
    {
        public List<OrderItem> Items { get; set; }
    }
}
