using BookshopAPI.Service;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    
    [Table("product")]
    public class Product: ProductRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public DateTime? updatedAt { get; set; }
        public DateTime? startsAt {  get; set; }
        public DateTime? endsAt {  get; set; }

        public Product convert(Product p)
        {
            return p;
        }
    }
    

        
    
}
