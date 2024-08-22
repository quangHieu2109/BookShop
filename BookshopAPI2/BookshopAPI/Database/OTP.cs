using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("otp")]
    public class OTP
    {
        [Key]
        [EmailAddress]
        public string email {  get; set; }
        public string otp {  get; set; }
        [Range(0,1)]
        public int accuracy {  get; set; }
        public DateTime endAt { get; set; }
    }

    public class AccuracyOtp
    {
        [EmailAddress]
        public string email { get; set; }
        public string otp { get; set; }
    }

    public class ChangePasswordOtp
    {
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
    }
}
