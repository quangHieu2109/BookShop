using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();   
        [HttpGet("getAddress")]
        [Authorize]
        public IActionResult getAddress()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address = myDbContext.Addresses.Where(x => x.userId == userId).ToList();
            
                return Ok(responeMessage.response200(address));
            
           

        }
        [HttpPost("addAddress")]
        [Authorize]
        public IActionResult addAddress(AddressVM addressVM)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address = myDbContext.Addresses.SingleOrDefault(x => x.userId == userId && x.houseNumber == addressVM.houseNumber
                && x.province == addressVM.province && x.district == addressVM.district && x.ward == addressVM.ward);
            if(address != null)
            {
                return Ok(responeMessage.response400(address, "Địa chỉ này đã được tạo trước đó!"));
            }
            else 
            { 
                 address = new Address
                {
                    userId = userId,
                    houseNumber = addressVM.houseNumber,
                    province = addressVM.province,
                    district = addressVM.district,
                    ward = addressVM.ward
                };
                myDbContext.Addresses.Add(address);
                int rs =myDbContext.SaveChanges();
                    if (rs > 0)
                    {
                    address = myDbContext.Addresses.SingleOrDefault(x => x.userId == userId && x.houseNumber == addressVM.houseNumber
                    && x.province == addressVM.province && x.district == addressVM.district && x.ward == addressVM.ward);
                    return Ok(responeMessage.response200(address, "Thêm địa chỉ thành công!"));

                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
                    }
            }
            
        }

    }
}
