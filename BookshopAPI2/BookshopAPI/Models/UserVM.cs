using BookshopAPI.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    public class UserLogin

    {
        [Required]
        public string? username { get; set; }
        [Required]
        public string? password { get; set; }
        

    }
    public class UserRegister : UserLogin
    {
        [EmailAddress ]
        public string? email { get; set; }
        [MaxLength(255)]
        public string? fullName { get; set; }
    }
    
    
    public class GoogleUserVM: GoogleUser
    {
        public string fullName { get; set; }
    }
    public class UserInfor
    {
        public string fullName { get; set; }
        [RegularExpression(@"^\d{10}$|^\+\d{2} \d{9}")]
        public string phoneNumber { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [Range(0,1)]
        public int? gender { get; set;}

    }
}
