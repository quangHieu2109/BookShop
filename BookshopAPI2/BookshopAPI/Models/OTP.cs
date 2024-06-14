using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    [Table("otp")]
    public class OTP
    {
        [Key]
        public string email {  get; set; }
        public string otp {  get; set; }
        public int accuracy {  get; set; }
        public DateTime endAt { get; set; }
    }

    public class AccuracyOtp
    {
        public string email { get; set; }
        public string otp { get; set; }
    }

    public class ChangePasswordOtp
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
