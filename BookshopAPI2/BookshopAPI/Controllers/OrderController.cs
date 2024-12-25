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
    public class OrderController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();
        [HttpGet("getAllOrders")]
        [Authorize(Roles ="ADMIN, EMPLOYEE")]
        public async Task<IActionResult> getAll()
        {
            return Ok(responeMessage.response200(await myDbContext.Orders.ToListAsync()));
        }

        [HttpGet("getOrderByStatus/status:{status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public async Task<IActionResult> getOrderByStatus(int status)
        {
            return Ok(responeMessage.response200(await myDbContext.Orders.Where(x => x.status == status).ToListAsync()));
        }

        [HttpGet("getAllOrdersDetail")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public async Task<IActionResult> getAllDetail()
        {
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders =await myDbContext.Orders.ToListAsync();
            foreach (var order in orders)
            {
                var orderItems =await (from o in myDbContext.OrderItems
                                  where o.orderId == order.id
                                  select new OrderItemResponse
                                  {
                                      id = o.id,
                                      orderId = o.orderId,
                                      product = myDbContext.Products.SingleOrDefault(x => x.id == o.productId),
                                      price = o.price,
                                      discount = o.discount,
                                      quantity = o.quantity,
                                      createdAt = o.createdAt,
                                      updatedAt = o.updatedAt

                                  }).ToListAsync();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user =await myDbContext.Users.SingleOrDefaultAsync(x => x.id == order.userId),
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

        [HttpGet("getAllOrdersDetailByUser")]
        [Authorize]
        public async Task<IActionResult> getAllDetailByUser()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders =await myDbContext.Orders.Where(x => x.userId == userId).ToListAsync();
            foreach (var order in orders)
            {
                var orderItems =await (from o in myDbContext.OrderItems
                                  where o.orderId == order.id
                                  select new OrderItemResponse
                                  {
                                      id = o.id,
                                      orderId = o.orderId,
                                      product = myDbContext.Products.SingleOrDefault(x => x.id == o.productId),
                                      price = o.price,
                                      discount = o.discount,
                                      quantity = o.quantity,
                                      createdAt = o.createdAt,
                                      updatedAt = o.updatedAt

                                  }).ToListAsync();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user =await myDbContext.Users.SingleOrDefaultAsync(x => x.id == order.userId),
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
        [HttpGet("getOrderDetailById/orderId:{orderId}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public async Task<IActionResult> getOrderDetailByIdl(long orderId)
        {
           
            var order =await myDbContext.Orders.SingleOrDefaultAsync(x => x.id == orderId);
            var orderResponse=new OrderResponse();


                if (order == null)
            {
                return Ok(responeMessage.response400(null, "OrderId không chính xác"));
            }
            else
            {
                var orderItems =await (from o in myDbContext.OrderItems
                                  where o.orderId == order.id
                                  select new OrderItemResponse
                                  {
                                      id = o.id,
                                      orderId = o.orderId,
                                      product = myDbContext.Products.SingleOrDefault(x => x.id == o.productId),
                                      price = o.price,
                                      discount = o.discount,
                                      quantity = o.quantity,
                                      createdAt = o.createdAt,
                                      updatedAt = o.updatedAt

                                  }).ToListAsync();
                 orderResponse = new OrderResponse
                {
                    id = order.id,
                    user =await myDbContext.Users.SingleOrDefaultAsync(x => x.id == order.userId),
                    status = order.status,
                    deliveryMethod = order.deliveryMethod,
                    deliveryPrice = order.deliveryPrice,
                    createdAt = order.createdAt,
                    updatedAt = order.updatedAt,
                    Items = orderItems
                };

            }
            
            return Ok(responeMessage.response200(orderResponse));
        }


        [HttpGet("getOrderDetailByStatus/status:{status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public async Task<IActionResult> getOrderDetailByStatus(int status)
        {
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders =await myDbContext.Orders.Where(x => x.status == status).ToListAsync();
            foreach (var order in orders)
            {
                var orderItems =await (from o in myDbContext.OrderItems
                                  where o.orderId == order.id
                                  select new OrderItemResponse
                                  {
                                      id = o.id,
                                      orderId = o.orderId,
                                      product = myDbContext.Products.SingleOrDefault(x => x.id == o.productId),
                                      price = o.price,
                                      discount = o.discount,
                                      quantity = o.quantity,
                                      createdAt = o.createdAt,
                                      updatedAt = o.updatedAt

                                  }).ToListAsync();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user =await myDbContext.Users.SingleOrDefaultAsync(x => x.id == order.userId),
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
        public async Task<IActionResult> createOrder(OrderRequest orderRequest)
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
            int rs =await myDbContext.SaveChangesAsync();
            if (rs <= 0)
            {
                return Ok( responeMessage.response500);
            }
            List<long> cartItemId = orderRequest.cartItemIds;
            foreach(var id  in cartItemId)
            {
                var cartItem =await myDbContext.CartItems.SingleOrDefaultAsync(x => x.id == id);
                if (cartItem != null)
                {
                    var product =await myDbContext.Products.SingleOrDefaultAsync(y => y.id == cartItem.productId);
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
                        await myDbContext.OrderItems.AddAsync(orderItem);
                        await myDbContext.SaveChangesAsync();
                    }
                }
            }
            return Ok(responeMessage.response200(order, "Đặt hàng thành công"));


        }

        [HttpPut("updateStatus/orderId:{orderId}&status:{status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public async Task<IActionResult> updateStatus(long orderId, int status)
        {
            var order = await myDbContext.Orders.SingleOrDefaultAsync(x => x.id == orderId);
            if(order != null)
            {
                if(order.status == 3)
                {
                    return Ok(responeMessage.response400(null, "Không thể thay đổi trạng thái của đơn hàng đã hủy!"));
                }
                if(status >5 || status < 0)
                {
                    return Ok(responeMessage.response400(null, "Status mới không hợp lệ!"));
                }
                if(order.status == status) {
                    return Ok(responeMessage.response400(null, "Status mới bị trùng với status cũ!"));

                }
                order.status = status;
                int rs =await myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return Ok(responeMessage.response200(null, "Cập nhật trạng thái đơn hàng thành công"));
                }
                else
                {
                    return Ok(responeMessage.response500);
                }
            }
            return Ok(responeMessage.response400(null, "OrderId không chính xác!"));
        }
    }
}
