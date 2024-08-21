using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

namespace BookshopAPI.Models
{
    [Table("cart_item")]
    public class CartItem
    {
        [Key]
        public long id {  get; set; }  
        public long cartId { get; set; }
        public long productId {  get; set; }
        public int quantity {  get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set;}

    }
    
}
