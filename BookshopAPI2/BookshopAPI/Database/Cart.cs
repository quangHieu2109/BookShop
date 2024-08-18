using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("cart")]
    public class Cart
    {
        [Key]
        public long id { get; set; }
        public long userId {  get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt {  get; set; }
    }
}
