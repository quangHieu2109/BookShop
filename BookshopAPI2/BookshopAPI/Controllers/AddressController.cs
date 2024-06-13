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
        private ResponeMessage responeMessage = new ResponeMessage();   
        [HttpGet("getAddress")]
        [Authorize]
        public IActionResult getAddress()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var address = myDbContext.Addresses.Where(x => x.userId == userId).ToList();
            if(address.Count > 0 )
            {
                return Ok(responeMessage.response200(address));
            }
            else
            {
                return BadRequest(responeMessage.response404);
            }
           

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
                return Ok(responeMessage.response200(address));

            }
            return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);

        }

    }
}
