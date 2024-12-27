using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Database
{
    [Table("email")]
    public class Email
    {
        [Key]
        public string uid { get; set; }
        public long userId { get; set; }
        public string email { get; set; }
    }
}
