using BookshopAPI.Models;
using BookshopAPI.Service;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Security.Claims;
using System.Xml;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();
        private ProductRating productRating = new ProductRating();

        [HttpGet("getAllProduct")]
        public IActionResult getAllProduct()
        {
            long userId=-1;
            if(this.User.FindFirstValue("Id") != null)
            {
                userId  = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = myDbContext.Products.ToList();
            List<Object> ProductRatings = new List<Object>();
            foreach (var product in products)
            {
                ProductRatings.Add(productRating.GetProductRating(product, userId));
            }
            return Ok(responeMessage.response200(ProductRatings));
        }
        [HttpGet("getWishList")]
        [Authorize]
        public IActionResult getWishList()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist = (from w in myDbContext.WishListItems
                           where w.userId == userId
                           select w).ToList();
            List<Object> products = new List<Object>();
            foreach(var wish in wishlist)
            {
                products.Add(productRating.GetProductRating(myDbContext.Products.SingleOrDefault(x => x.id == wish.productId), userId));

            }
            if(products.Count > 0)
            {
                
                return Ok(responeMessage.response200(products));
            }
            return NotFound(responeMessage.response404);

        }
        [HttpGet("getProductByName")]
        public IActionResult getByName(string name)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = myDbContext.Products.Where(x =>
            x.name.Contains(name)).ToList();
            if (products != null)
            {
                List<Object> result = new List<Object>();
                foreach(var product in products)
                {
                    result.Add(productRating.GetProductRating(product, userId));
                }
                return Ok(responeMessage.response200(result));
            }
            else
            {
                return NotFound(responeMessage.response404);
            }

        }
        [HttpGet("getSimilarProduct")]
        public IActionResult getSimilarProduct(long productId)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var category_product = myDbContext.Product_Categories.SingleOrDefault(x => x.productId == productId);
            if (category_product != null)
            {
                var products = (from p in myDbContext.Products
                               join c in myDbContext.Product_Categories on p.id equals c.productId
                               where c.categoryId == category_product.categoryId
                               select  new Product().convert(p)).ToList() ;
                
                if (products != null)
                {
                    List<Object> result = new List<Object>();
                    foreach (var product in products)
                    {
                        result.Add(productRating.GetProductRating(product, userId));
                    }
                    return Ok(responeMessage.response200(result));
                }
            }
            return NotFound(responeMessage.response404);
        }


        [HttpGet("getProduct/categoryId={categoryId}")]
        public IActionResult getProductByCategoryId(long categoryId)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = from p in myDbContext.Products
                            join c in myDbContext.Product_Categories on p.id equals c.productId
                            where c.categoryId == categoryId
                            select  p ;
            if (products != null)
            {
                List<Object> result = new List<Object>();
                foreach (var product in products)
                {
                    result.Add(productRating.GetProductRating(product, userId));
                }
                return Ok(responeMessage.response200(result));
            }
            
            return NotFound(responeMessage.response404);
        }
        [HttpGet("getProduct/categoryName={categoryName}")]
        public IActionResult getProductByCategoryName(String categoryName)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var category = myDbContext.Categories.FirstOrDefault(x => x.name.Contains(categoryName));
            if(category != null)
            {
                var products = from p in myDbContext.Products
                               join c in myDbContext.Product_Categories on p.id equals c.productId
                               where c.categoryId == category.id
                               select p ;
                if (products != null)
                {
                    List<Object> result = new List<Object>();
                    foreach (var product in products)
                    {
                        result.Add(productRating.GetProductRating(product, userId));
                    }
                    return Ok(responeMessage.response200(result));
                }
            }

            return NotFound(responeMessage.response404);
        }


        [HttpPut("updateProduct")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult update(Product product)
        {
            var _product = myDbContext.Products.SingleOrDefault(x => (x.id) == product.id);
            if (_product != null)
            {
                _product = product;
                var rs = myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(_product));
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
                }
            }
            else
            {
                return NotFound(responeMessage.response404);
            }


        }

        [HttpPost("addWishList/productId={productId}")]
        [Authorize]
        public IActionResult AddWishList(long productId)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist = myDbContext.WishListItems.SingleOrDefault(x => x.productId == productId && x.userId == userId);
            var product = myDbContext.Products.SingleOrDefault(x => x.id == productId);
            if (product == null)
            {
                return BadRequest(responeMessage.response400("Mã sản phẩm không chính xác"));
            }
            
            if (wishlist != null)
            {
                return BadRequest(responeMessage.response400("Sản phẩm đã có trong danh sách yêu thích"));
            }
            else
            {
                wishlist = new WishListItem
                {
                    userId = userId,
                    productId = productId,
                    createdAt = DateTime.Now
                };
                myDbContext.WishListItems.Add(wishlist);
                int rs =  myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(wishlist));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            

        }

        [HttpDelete("deleteWishList/productId={productId}")]
        [Authorize]
        public IActionResult DeleteWishList(long productId)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            
            var product = myDbContext.Products.SingleOrDefault(x => x.id == productId);
            if (product == null)
            {
                return BadRequest(responeMessage.response400("Mã sản phẩm không chính xác"));
            }
            else
            {
                var wishlist = myDbContext.WishListItems.SingleOrDefault(x => x.productId == productId && x.userId == userId);
                if (wishlist != null)
                {
                    myDbContext.WishListItems.Remove(wishlist);
                    myDbContext.SaveChanges();
                    return Ok(responeMessage.response200("Xóa sản phẩm khỏi danh sách yêu thích thành công"));
                }
                else
                {
                    return BadRequest(responeMessage.response400("Sản phẩm không có trong danh sách yêu thích!"));
                }

               
            }
            

        }
            [HttpGet("getByOrderRating")]
        public IActionResult getRecommend()
        {
            var productRatings = myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     .OrderByDescending(x => x.rating)
                     .Take(10)
                     .ToList();
            return Ok(responeMessage.response200(productRatings));
        }
        [HttpGet("getRecommendByOrderRating/productId={productId}")]
        public IActionResult getRecommendByOrderRating(long productId)
        {
            var category_product = myDbContext.Product_Categories.SingleOrDefault(x => x.productId == productId);
            var productRatings = myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     
                     .OrderByDescending(x => x.rating)
                     .Take(10)
                     .ToList();
            var productRatings2 = from pr in productRatings
                                  where (myDbContext.Product_Categories.Single(x => x.productId == pr.Product.id).categoryId == category_product.categoryId)
                                  select pr;
            return Ok(responeMessage.response200(productRatings2));
        }

        [HttpGet("getTopSell")]
        public IActionResult getTopSell()
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = myDbContext.OrderItems
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         Quantity = g.Sum(pr => pr.quantity),
                         
                     })
                     .OrderByDescending(x => x.Quantity)
                     .Take(10)
                     .ToList();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(new
                {
                    product= product,
                    quantity=product.Quantity,
                    rating = productRating.GetProductRating(product.Product, userId).rating,
                    wishlist = productRating.GetProductRating(product.Product, userId).wishlist
                });
            }
            return Ok(responeMessage.response200(result));
        }

        [HttpGet("getReleases")]
        public IActionResult getReleases()
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = myDbContext.Products
                        
                     .OrderByDescending(x => x.createdAt)
                     .Take(10)
                     .ToList();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(productRating.GetProductRating(product, userId ));
            }
            return Ok(responeMessage.response200(result));
        }

        [HttpGet("getPerchased")]
        [Authorize]
        public IActionResult getPerchased()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var orderItemss = (from o in myDbContext.Orders
                               join oi in myDbContext.OrderItems on o.id equals oi.orderId

                               where o.userId == userId
                               select oi).ToList();
            var products = (from oi in orderItemss
                            join p in myDbContext.Products on oi.productId equals p.id
                            select new Product().convert(p))
                            .GroupBy(x => x.id)
                            .Select(g => g.First())
                            .ToList();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(productRating.GetProductRating(product, userId ));
            }
            return Ok(responeMessage.response200(result));
           


        }
        
    }
}
