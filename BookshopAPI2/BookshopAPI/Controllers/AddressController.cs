using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> getAddress()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address =await myDbContext.Addresses.Where(x => x.userId == userId).ToListAsync();
            
                return Ok(responeMessage.response200(address));
            
           

        }
        [HttpPost("addAddress")]
        [Authorize]
        public async Task<IActionResult> addAddress(AddressRequest addressVM)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address =await myDbContext.Addresses.SingleOrDefaultAsync(x => x.userId == userId && x.houseNumber == addressVM.houseNumber
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
               await myDbContext.Addresses.AddAsync(address);
                int rs =await myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                    address =await myDbContext.Addresses.SingleOrDefaultAsync(x => x.userId == userId && x.houseNumber == addressVM.houseNumber
                    && x.province == addressVM.province && x.district == addressVM.district && x.ward == addressVM.ward);
                    return Ok(responeMessage.response200(address, "Thêm địa chỉ thành công!"));

                    }
                    else
                    {
                        return Ok( responeMessage.response500);
                    }
            }
            
        }

    }
}
