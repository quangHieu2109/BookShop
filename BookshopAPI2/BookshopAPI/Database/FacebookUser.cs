using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Database
{
    [Table("facebook_user")]
    public class FacebookUser
    {
        [Key]
        public string id { get; set; }
        [ForeignKey("userId")]
        public long userId {  get; set; }
        public DateTime createAt { get; set; }
    }
}
