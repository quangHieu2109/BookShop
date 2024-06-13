using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController2 : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();
        [HttpGet("getAllOrders")]
        [Authorize(Roles ="ADMIN, EMPLOYEE")]
        public IActionResult getAll()
        {
            return Ok(responeMessage.response200(myDbContext.Orders));
        }
        [HttpGet("getAllOrdersDetail")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult getAllDetail()
        {
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders = myDbContext.Orders;
            foreach (var order in orders)
            {
                var orderItems = (from o in myDbContext.OrderItems
                                 where o.orderId == order.id
                                 select new OrderItem
                                 {
                                     id = o.id,
                                     orderId = o.orderId,
                                     productId = o.productId,
                                     price = o.price,
                                     discount = o.discount,
                                     quantity = o.quantity,
                                     createdAt = o.createdAt,
                                     updatedAt = o.updatedAt

                                 }).ToList();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    userId = order.userId,
                    status = order.status,
                    deliveryMethod = order.deliveryMethod,
                    deliveryPrice = order.deliveryPrice,
                    createdAt = order.createdAt,
                    updatedAt = order.updatedAt,
                    Items = orderItems
                };
                OrderResponse.Add(orderResponse);
            }
            return Ok(responeMessage.response200(OrderResponse));
        }
        [HttpPost("createOrder")]
        [Authorize]
        public IActionResult createOrder(OrderRequest orderRequest)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var order = new Order
            {
                id = DateTime.Now.ToFileTimeUtc(),
                userId = userId,
                status = 0,
                deliveryMethod = orderRequest.deliveryMethod,
                deliveryPrice = orderRequest.deliveryPrice,
                createdAt = DateTime.Now

            };
            myDbContext.Orders.Add(order);
            int rs = myDbContext.SaveChanges();
            if (rs <= 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            List<long> cartItemId = orderRequest.cartItemIds;
            foreach(var id  in cartItemId)
            {
                var cartItem = myDbContext.CartItems.SingleOrDefault(x => x.id == id);
                if (cartItem != null)
                {
                    var product = myDbContext.Products.SingleOrDefault(y => y.id == cartItem.productId);
                    if (product != null)
                    {
                        var orderItem = new OrderItem
                        {
                            orderId = order.id,
                            productId = product.id,
                            price = product.price,
                            discount = product.discount,
                            quantity = cartItem.quantity,
                            createdAt = DateTime.Now

                        };
                        myDbContext.OrderItems.Add(orderItem);
                        myDbContext.SaveChanges();
                    }
                }
            }
            return Ok(responeMessage.response200(order, "Đặt hàng thành công"));


        }

        [HttpPut("updateStatus/orderId={orderId}&status={status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult updateStatus(long orderId, int status)
        {
            var order = myDbContext.Orders.SingleOrDefault(x => x.id == orderId);
            if(order != null)
            {
                if(order.status == 3)
                {
                    return BadRequest(responeMessage.response400("Không thể thay đổi trạng thái của đơn hàng đã hủy!"));
                }
                if(status >5 || status < 0)
                {
                    return BadRequest(responeMessage.response400("Status mới không hợp lệ!"));
                }
                if(order.status == status) {
                    return BadRequest(responeMessage.response400("Status mới bị trùng với status cũ!"));

                }
                order.status = status;
                int rs =myDbContext.SaveChanges();
                if(rs > 0)
                {
                    return Ok(responeMessage.response200(order, "Cập nhật trạng thái đơn hàng thành công"));
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
                }
            }
            return BadRequest(responeMessage.response400("OrderId không chính xác!"));
        }
    }
}
