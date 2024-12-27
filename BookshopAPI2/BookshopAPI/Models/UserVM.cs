using BookshopAPI.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    public class UserLogin

    {
        
        public string? username { get; set; }
        
        public string? password { get; set; }
        

    }
    public class UserRegister : UserLogin
    {
        public string? uid {  get; set; } 
        public string? email { get; set; }
    
        public string? fullName { get; set; }
    }
    
    
    public class GoogleUserVM: GoogleUser
    {
        public string fullName { get; set; }
    }
    public class UserInfor
    {
        public string fullName { get; set; }
        
        public string phoneNumber { get; set; }
        
        public string uid { get; set; }
        public string email { get; set; }

        public int? gender { get; set;}

    }
}
