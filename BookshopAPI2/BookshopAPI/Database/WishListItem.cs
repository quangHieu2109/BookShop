using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("wishlist_item")]
    public class WishListItem
    {
        [Key]
        public long id {  get; set; }
        public long userId {  get; set; }
        public long productId {  get; set; }
        public DateTime createdAt {  get; set; }

    }
}
