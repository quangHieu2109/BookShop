using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    public class UserLogin

    {
        public string? username {  get; set; } 
        public string? password { get; set; }
        

    }
    public class UserRegister : UserLogin
    {
        public string? email { get; set; }
        public string? fullName { get; set; }
    }
    [Table("user")]
    public class User : UserRegister
    {
        [Key]
        public long id { get; set; }
        public string? phoneNumber { get; set; }
        public int? gender {  get; set; }
        public string? role {  get; set; }
        public DateTime? createAt { get; set; }

    }
    [Table("google_user")]
    public class GoogleUser
    {
        [Key]
        public long googleId { get; set; }
        public string email { get; set; }
    }
    public class GoogleUserVM: GoogleUser
    {
        public string fullName { get; set; }
    }
}
