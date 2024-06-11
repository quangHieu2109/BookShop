using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        [HttpGet("getAddress")]
        [Authorize]
        public IActionResult getAddress()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address = myDbContext.Addresses.All(x => x.userId == userId);
            return Ok(address);

        }
        [HttpPost("addAddress")]
        [Authorize]
        public IActionResult addAddress(AddressVM addressVM)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address = new Address
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
                return Ok(address);

            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi từ server, vui lòng thử lại sau!");

        }

    }
}
