using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("product_review")]
    public class ProductReview
    {
        [Key]
        public long id { get; set; }
        [ForeignKey("productId")]
        [Required]
        public long productId { get; set; }
        [ForeignKey("userId")]
        [Required]
        public long userId {  get; set; }
        public int ratingScore {  get; set; }
        public string content { get; set; }
        public int isShow {  get; set; }
        public DateTime createdAt {  get; set; }    
        public DateTime? updatedAt { get; set; }
    }
}
