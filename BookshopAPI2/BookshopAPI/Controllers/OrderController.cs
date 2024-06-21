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
        [HttpGet("getAllOrders")]
        [Authorize(Roles ="ADMIN, EMPLOYEE")]
        public IActionResult getAll()
        {
            return Ok(responeMessage.response200(myDbContext.Orders));
        }

        [HttpGet("getOrderByStatus/status={status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult getOrderByStatus(int status)
        {
            return Ok(responeMessage.response200(myDbContext.Orders.Where(x => x.status == status).ToList()));
        }

        [HttpGet("getAllOrdersDetail")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult getAllDetail()
        {
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders = myDbContext.Orders.ToList();
            foreach (var order in orders)
            {
                var orderItems = (from o in myDbContext.OrderItems
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

                                  }).ToList();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user = myDbContext.Users.SingleOrDefault(x => x.id == order.userId),
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
        public IActionResult getAllDetailByUser()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders = myDbContext.Orders.Where(x => x.userId == userId).ToList();
            foreach (var order in orders)
            {
                var orderItems = (from o in myDbContext.OrderItems
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

                                  }).ToList();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user = myDbContext.Users.SingleOrDefault(x => x.id == order.userId),
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
        [HttpGet("getOrderDetailById/orderId={orderId}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult getOrderDetailByIdl(long orderId)
        {
           
            var order = myDbContext.Orders.SingleOrDefault(x => x.id == orderId);
            var orderResponse=new OrderResponse();


                if (order == null)
            {
                return Ok(responeMessage.response400(null, "OrderId không chính xác"));
            }
            else
            {
                var orderItems = (from o in myDbContext.OrderItems
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

                                  }).ToList();
                 orderResponse = new OrderResponse
                {
                    id = order.id,
                    user = myDbContext.Users.SingleOrDefault(x => x.id == order.userId),
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


        [HttpGet("getOrderDetailByStatus/status={status}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        public IActionResult getOrderDetailByStatus(int status)
        {
            List<OrderResponse> OrderResponse = new List<OrderResponse>();
            var orders = myDbContext.Orders.Where(x => x.status == status).ToList();
            foreach (var order in orders)
            {
                var orderItems = (from o in myDbContext.OrderItems
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

                                  }).ToList();
                var orderResponse = new OrderResponse
                {
                    id = order.id,
                    user = myDbContext.Users.SingleOrDefault(x => x.id == order.userId),
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
                    return Ok(responeMessage.response400("Không thể thay đổi trạng thái của đơn hàng đã hủy!"));
                }
                if(status >5 || status < 0)
                {
                    return Ok(responeMessage.response400("Status mới không hợp lệ!"));
                }
                if(order.status == status) {
                    return Ok(responeMessage.response400("Status mới bị trùng với status cũ!"));

                }
                order.status = status;
                int rs =myDbContext.SaveChanges();
                if(rs > 0)
                {
                    return Ok(responeMessage.response200(order, "Cập nhật trạng thái đơn hàng thành công"));
                }
                else
                {
                    return Ok(responeMessage.response500);
                }
            }
            return Ok(responeMessage.response400("OrderId không chính xác!"));
        }
    }
}
