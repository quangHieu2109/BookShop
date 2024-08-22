using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookshopAPI.Database
{
    [Table("google_user")]
    public class GoogleUser
    {
        [Key]
        public long googleId { get; set; }
        [EmailAddress]
        public string email { get; set; }
    }
}
