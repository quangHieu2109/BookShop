using BookshopAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace BookshopAPI.Database
{
    [Table("user")]
    public class User 
    {
        [Key]
        
        public long id { get; set; }
        public string? username { get; set; }

        public string? password { get; set; }
        public string? fullName { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [RegularExpression(@"^\d{10}$|^\+\d{2} \d{9}", ErrorMessage = "Phone number must be 0xxxxxxxxx or +84 xxxxxxxx")]
        public string? phoneNumber { get; set; }
        [Range(0, 1)]
        public int? gender { get; set; }


        public string? role { get; set; }
        public DateTime? createAt { get; set; }

    }
}
