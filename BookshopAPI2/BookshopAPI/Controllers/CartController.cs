using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
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
        public IActionResult getCart()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = myDbContext.Carts.SingleOrDefault(x => x.userId == userId);
            if (cart == null)
            {
                 cart = new Cart
                {
                    id = DateTime.Now.ToFileTimeUtc(),
                    userId = userId,
                    createdAt = DateTime.Now

                };
                myDbContext.Carts.Add(cart);
                myDbContext.SaveChanges();
            }
            var cartItems = (from ci in myDbContext.CartItems
                            where ci.cartId == cart.id
                            select new CartItemResponse{
                            id = ci.id,
                            cartId = ci.cartId,
                            product = myDbContext.Products.SingleOrDefault(x => x.id == ci.productId),
                            quantity= ci.quantity,
                            createdAt = ci.createdAt,
                            updatedAt = ci.updatedAt
                            }).ToList();
            
            return Ok(new CartResponse
            {
                id = cart.id,
                userId = cart.userId,
                createdAt = cart.createdAt,
                updatedAt = cart.updatedAt,
                CartItems = cartItems

            }) ;
        }
        [HttpPost("addCartItemPId={productId}")]
        [Authorize]
        public IActionResult addCartItemByPId(long productId, int quantity=1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = myDbContext.Carts.SingleOrDefault(x => x.userId == userId);
            var product = myDbContext.Products.SingleOrDefault(x => x.id == productId);
            if (product != null)
            {
                var cartItem = myDbContext.CartItems.SingleOrDefault(x => x.productId == productId && x.cartId == cart.id) ;
                if(cartItem != null) 
                {
                    cartItem.quantity += quantity;
                    myDbContext.SaveChanges();
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
                    myDbContext.CartItems.Add(cartItem);
                    myDbContext.SaveChanges();
                }
                return Ok(responeMessage.response200(cartItem));
            }
            return BadRequest(responeMessage.response400);
        }
        [HttpPost("addCartItemPName={productName}")]
        [Authorize]
        public IActionResult addCartItemByPName(String productName, int quantity = 1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = myDbContext.Carts.SingleOrDefault(x => x.userId == userId);
            var product = myDbContext.Products.SingleOrDefault(x => x.name == productName);
            if (product != null)
            {
                var cartItem = myDbContext.CartItems.SingleOrDefault(x => x.productId == product.id && x.cartId == cart.id);
                if (cartItem != null)
                {
                    cartItem.quantity += quantity;
                    myDbContext.SaveChanges();
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
                    myDbContext.CartItems.Add(cartItem);
                    myDbContext.SaveChanges();
                }
                return Ok(responeMessage.response200(cartItem));
            }
            return BadRequest(responeMessage.response400);
        }

        [HttpPut("updateCartItemId={cartItemId}")]
        [Authorize]
        public IActionResult updateCartItem(long cartItemId, int quantity = 1)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = myDbContext.Carts.SingleOrDefault(x => x.userId == userId);
            var cartItem = myDbContext.CartItems.SingleOrDefault(x => x.id == cartItemId);
            if (cartItem != null)
            {                          
                cartItem.quantity = quantity;
                int rs = myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(cartItem));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            return BadRequest(responeMessage.response400);
        }

        [HttpDelete("deleteCartItemId={id}")]
        [Authorize]
        public IActionResult deleteCartItem(long id)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var cart = myDbContext.Carts.SingleOrDefault(x => x.userId == userId);
            var cartItem = myDbContext.CartItems.SingleOrDefault(x => x.id == id);
            if (cartItem != null)
            {
                myDbContext.CartItems.Remove(cartItem);
                int rs = myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            return BadRequest(responeMessage.response400);
        }

    }
    public class CartResponse : Cart
    {
        public List<CartItemResponse> CartItems { get; set; }
    }
}
