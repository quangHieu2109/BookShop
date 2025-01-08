using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController: ControllerBase
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();   
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getCart()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart =await myDbContext.Carts.SingleOrDefaultAsync(x => x.userId == userId);
            if (cart == null)
            {
                 cart = new Cart
                {
                    id = DateTime.Now.ToFileTimeUtc(),
                    userId = userId,
                    createdAt = DateTime.Now

                };
                await myDbContext.Carts.AddAsync(cart);
                await myDbContext.SaveChangesAsync();
            }
            var cartItems =await (from ci in myDbContext.CartItems
                            where ci.cartId == cart.id
                            select new CartItemResponse{
                            id = ci.id,
                            cartId = ci.cartId,
                            product = myDbContext.Products.SingleOrDefault(x => x.id == ci.productId),
                            quantity= ci.quantity,
                            createdAt = ci.createdAt,
                            updatedAt = ci.updatedAt
                            }).ToListAsync();
            var cartResponse = new CartResponse
            {
                id = cart.id,
                userId = cart.userId,
                createdAt = cart.createdAt,
                updatedAt = cart.updatedAt,
                CartItems = cartItems

            };
            return Ok(responeMessage.response200(cartResponse)) ;
        }
        [HttpPost("addCartItemPId:{productId}")]
        [Authorize]
        public async Task<IActionResult> addCartItemByPId(long productId, int quantity=1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart =await myDbContext.Carts.SingleOrDefaultAsync(x => x.userId == userId);
            var product =await myDbContext.Products.SingleOrDefaultAsync(x => x.id == productId);
            if (product != null)
            {
                var cartItem =await myDbContext.CartItems.SingleOrDefaultAsync(x => x.productId == productId && x.cartId == cart.id) ;
                if(cartItem != null) 
                {
                    cartItem.quantity += quantity;
                    await myDbContext.SaveChangesAsync();
                }
                else
                {
                    cartItem= new CartItem
                    {
                        cartId = cart.id,
                        productId = product.id,
                        quantity = quantity,
                        createdAt = DateTime.Now
                    };
                    await myDbContext.CartItems.AddAsync(cartItem);
                    await myDbContext.SaveChangesAsync();
                }
                return Ok(responeMessage.response200(cartItem));
            }
            return Ok(responeMessage.response400);
        }
        [HttpPost("addCartItemPName:{productName}")]
        [Authorize]
        public async Task<IActionResult> addCartItemByPName(String productName, int quantity = 1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = await myDbContext.Carts.SingleOrDefaultAsync(x => x.userId == userId);
            var product =await myDbContext.Products.SingleOrDefaultAsync(x => x.name == productName);
            if (product != null)
            {
                var cartItem =await myDbContext.CartItems.SingleOrDefaultAsync(x => x.productId == product.id && x.cartId == cart.id);
                if (cartItem != null)
                {
                    cartItem.quantity += quantity;
                    await myDbContext.SaveChangesAsync();
                }
                else
                {
                    cartItem = new CartItem
                    {
                        cartId = cart.id,
                        productId = product.id,
                        quantity = quantity,
                        createdAt = DateTime.Now
                    };
                   await myDbContext.CartItems.AddAsync(cartItem);
                   await myDbContext.SaveChangesAsync();
                }
                return Ok(responeMessage.response200(cartItem));
            }
            return Ok(responeMessage.response400);
        }

        [HttpPut("updateCartItemId:{cartItemId}")]
        [Authorize]
        public async Task<IActionResult> updateCartItem(long cartItemId, int quantity = 1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart =await myDbContext.Carts.SingleOrDefaultAsync(x => x.userId == userId);
            var cartItem = await myDbContext.CartItems.SingleOrDefaultAsync(x => x.id == cartItemId);
            if (cartItem != null)
            {                          
                cartItem.quantity = quantity;
                int rs =await myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(cartItem));
                }
                return Ok(responeMessage.response500);
            }
            return Ok(responeMessage.response400);
        }

        [HttpDelete("deleteCartItemId:{id}")]
        [Authorize]
        public async Task<IActionResult> deleteCartItem(long id)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = await myDbContext.Carts.SingleOrDefaultAsync(x => x.userId == userId);
            var cartItem = await myDbContext.CartItems.SingleOrDefaultAsync(x => x.id == id);
            if (cartItem != null)
            {
                if(cartItem.cartId != cart.id)
                {
                    return Ok(responeMessage.response400(null, "CartItem muốn xóa không phải của người dùng hiện tại"));
                }
                myDbContext.CartItems.Remove(cartItem);
                int rs = await myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(null, "Xóa cartItem thành công"));
                }
                return Ok(responeMessage.response500(null, "Có lỗi từ server, vui lòng thử lại sau"));
                
            }
            
            return Ok(responeMessage.response400(null, "CartItemId không chính xác"));
        }

    }
    public class CartResponse : Cart
    {
        public List<CartItemResponse> CartItems { get; set; }
    }
}
