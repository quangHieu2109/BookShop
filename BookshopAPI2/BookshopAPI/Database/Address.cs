

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{

    [Table("address")]
    public class Address:AddressRequest
    {
        [Key]
        public long id { get; set; }
        public long userId {  get; set; }
       
    }
}
