using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("refresh_token")]
    public class RefreshToken
    {
        [Key]
        [ForeignKey("UserId")]
        public long userId { get; set; }
        public string refreshToken { get; set; }
        public DateTime endAt { get; set; }
    }
}
