

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    public class AddressVM
    {
        public string? houseNumber { get; set; }
        public string? province { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
    }
    [Table("address")]
    public class Address:AddressVM
    {
        [Key]
        public long id { get; set; }
        public long userId {  get; set; }
       
    }
}
