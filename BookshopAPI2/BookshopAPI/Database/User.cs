using BookshopAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Mysqlx;

namespace BookshopAPI.Database
{
    [Table("user")]
    public class User : UserRegister
    {
        [Key]
        [MaxLength(20)]
        public long id { get; set; }
        [RegularExpression(@"^\d{10}$|^\+\d{2} \d{9}$", ErrorMessage = "Phone number must be 0xxxxxxxxx or +xx xxxxxxxxxxx")]
        public string? phoneNumber { get; set; }
        [Range(0, 1)]
        public int? gender { get; set; }
        public string? role { get; set; }
        public DateTime? createAt { get; set; }

    }
}
