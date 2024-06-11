using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models

{
    [Table("orders")]
    public class Order
    {
        [Key]
        public long id {  get; set; }
        public long userId {  get; set; }
        public int status { get; set; }
        public int deliveryMethod {  get; set; }
        public double deliveryPrice {  get; set; }
        public DateTime createdAt {  get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
