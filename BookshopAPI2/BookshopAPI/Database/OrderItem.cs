using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("order_item")]
    public class OrderItem
    {
        [Key]
        public long id {  get; set; }
        public long orderId {  get; set; }
        public long productId {  get; set; }
        public double price { get; set; }
        public double discount {  get; set; }
        public int quantity {  get; set; }
        public DateTime createdAt {  get; set; }
        public DateTime? updatedAt { get; set; }
    }
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
