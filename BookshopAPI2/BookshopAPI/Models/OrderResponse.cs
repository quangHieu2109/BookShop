using BookshopAPI.Database;

namespace BookshopAPI.Models
{
    public class OrderResponse

    {

        public long id { get; set; }
        public User user { get; set; }
        public int status { get; set; }
        public int deliveryMethod { get; set; }
        public double deliveryPrice { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; }
    }
}
