using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("category")]
    public class Category
    {
        [Key]
        public long id {  get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageName {  get; set; }
    }
}
