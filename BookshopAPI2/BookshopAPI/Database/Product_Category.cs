using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("product_category")]
    public class Product_Category
    {
        [Key]
        [ForeignKey("productId")]
        public long productId { get; set; }
        [Required]
        [ForeignKey("categoryId")]
        public long categoryId { get; set; }

    }
}
