using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookshopAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagementController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();
        [HttpGet("getShopInfo")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> getGetShopInfo()
        {
            int countUser = myDbContext.Users.Count();
            int countCategory = myDbContext.Categories.Count();
            int countProduct = myDbContext.Products.Count();
            int countOrder = myDbContext.Orders.Count();
            var ShopInfo = new ShopInfo
            {
                countUser = countUser,
                countCategory = countCategory,
                countOrder = countOrder,
                countproduct = countProduct
            };
            return Ok(responeMessage.response200(ShopInfo));
        }
    }
}