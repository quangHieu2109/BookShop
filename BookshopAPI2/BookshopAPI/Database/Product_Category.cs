using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("product_category")]
    public class Product_Category
    {
        [Key]
        public long productId { get; set; }
       
        public long categoryId { get; set; }

    }
}
