using BookshopAPI.Models;
using BookshopAPI.Service;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> getAllProduct()
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = await myDbContext.Products.ToListAsync();
            List<Object> ProductRatings = new List<Object>();
            foreach (var product in products)
            {
                ProductRatings.Add(await productRating.GetProductRating(product, userId));
            }
            return Ok(responeMessage.response200(ProductRatings));
        }
        [HttpGet("getWishList")]
        [Authorize]
        public async Task<IActionResult> getWishList()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist = await (from w in myDbContext.WishListItems
                                  where w.userId == userId
                                  select w).ToListAsync();
            List<Object> products = new List<Object>();
            foreach (var wish in wishlist)
            {
                products.Add(await productRating.GetProductRating(await myDbContext.Products.SingleOrDefaultAsync(x => x.id == wish.productId), userId));

            }
            if (products.Count > 0)
            {

                return Ok(responeMessage.response200(products));
            }
            return Ok(responeMessage.response404);

        }
        [HttpGet("getProductByName")]
        public async Task<IActionResult> getByName([Required] string name)
        public async Task<IActionResult> getByName([Required]string name)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = await myDbContext.Products.Where(x =>
            x.name.Contains(name)).ToListAsync();
            if (products != null)
            {
                List<Object> result = new List<Object>();
                foreach (var product in products)
                {
                    result.Add(await productRating.GetProductRating(product, userId));
                }
                return Ok(responeMessage.response200(result));
            }
            else
            {
                return Ok(responeMessage.response404);
            }

        }
        [HttpGet("getSimilarProduct")]
        public async Task<IActionResult> getSimilarProduct([Required] long productId)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var category_product = await myDbContext.Product_Categories.SingleOrDefaultAsync(x => x.productId == productId);
            if (category_product != null)
            {
                var products = await (from p in myDbContext.Products
                                      join c in myDbContext.Product_Categories on p.id equals c.productId
                                      where c.categoryId == category_product.categoryId
                                      select new Product().convert(p)).ToListAsync();

                if (products != null)
                {
                    List<Object> result = new List<Object>();
                    foreach (var product in products)
                    {
                        result.Add(await productRating.GetProductRating(product, userId));
                    }
                    return Ok(responeMessage.response200(result));
                }
            }
            return Ok(responeMessage.response404);
        }


        [HttpGet("getProduct/categoryId={categoryId}")]
        public async Task<IActionResult> getProductByCategoryId([Required] long categoryId)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products = from p in myDbContext.Products
                           join c in myDbContext.Product_Categories on p.id equals c.productId
                           where c.categoryId == categoryId
                           select p;
            if (products != null)
            {
                List<Object> result = new List<Object>();
                foreach (var product in products)
                {
                    result.Add(await productRating.GetProductRating(product, userId));
                }
                return Ok(responeMessage.response200(result));
            }

            return Ok(responeMessage.response404);
        }
        [HttpGet("getProduct/categoryName={categoryName}")]
        public async Task<IActionResult> getProductByCategoryName(String categoryName)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var category = await myDbContext.Categories.FirstOrDefaultAsync(x => x.name.Contains(categoryName));
            if (category != null)
            {
                var products = from p in myDbContext.Products
                               join c in myDbContext.Product_Categories on p.id equals c.productId
                               where c.categoryId == category.id
                               select p;
                if (products != null)
                {
                    List<Object> result = new List<Object>();
                    foreach (var product in products)
                    {
                        result.Add(await productRating.GetProductRating(product, userId));
                    }
                    return Ok(responeMessage.response200(result));
                }
            }

            return Ok(responeMessage.response404);
        }
        [HttpDelete("deleteProduct/productId={productId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> deleteProduct([Required] long productId)
        {
            var product = await myDbContext.Products.FirstOrDefaultAsync(x => x.id ==  productId);
            if(product != null)
            {
                myDbContext.Remove(product);
                await myDbContext.SaveChangesAsync();
                return Ok(responeMessage.response200);
            }
            else
            {
                return Ok(responeMessage.response400(null, "Id không chính xác"));
            }
        }

        [HttpPut("updateProduct")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> update([Required] Product product)
        {
            var _product =await myDbContext.Products.SingleOrDefaultAsync(x => (x.id) == product.id);
            if (_product != null)
            {
                _product = product;
                var rs =await myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(_product));
                }
                else
                {
                    return Ok(responeMessage.response500);
                }
            }
            else
            {
                return Ok(responeMessage.response400(null,"Mã sản phẩm không chính xác"));
            }


        }

        [HttpPost("addWishList/productId={productId}")]
        [Authorize]
        public async Task<IActionResult> AddWishList([Required] long productId)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist =await myDbContext.WishListItems.SingleOrDefaultAsync(x => x.productId == productId && x.userId == userId);
            var product =await myDbContext.Products.SingleOrDefaultAsync(x => x.id == productId);
            if (product == null)
            {
                return Ok(responeMessage.response400("Mã sản phẩm không chính xác"));
            }
            
            if (wishlist != null)
            {
                return Ok(responeMessage.response400("Sản phẩm đã có trong danh sách yêu thích"));
            }
            else
            {
                wishlist = new WishListItem
                {
                    userId = userId,
                    productId = productId,
                    createdAt = DateTime.Now
                };
                await myDbContext.WishListItems.AddAsync(wishlist);
                int rs =await  myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(wishlist));
                }
                return Ok(responeMessage.response500);
            }
            

        }

        [HttpDelete("deleteWishList/productId={productId}")]
        [Authorize]
        public async Task<IActionResult> DeleteWishList([Required] long productId)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            
            var product =await myDbContext.Products.SingleOrDefaultAsync(x => x.id == productId);
            if (product == null)
            {
                return Ok(responeMessage.response400("Mã sản phẩm không chính xác"));
            }
            else
            {
                var wishlist =await myDbContext.WishListItems.SingleOrDefaultAsync(x => x.productId == productId && x.userId == userId);
                if (wishlist != null)
                {
                    myDbContext.WishListItems.Remove(wishlist);
                    await myDbContext.SaveChangesAsync();
                    return Ok(responeMessage.response200("Xóa sản phẩm khỏi danh sách yêu thích thành công"));
                }
                else
                {
                    return Ok(responeMessage.response400("Sản phẩm không có trong danh sách yêu thích!"));
                }

               
            }
            

        }
        [HttpGet("getByOrderRating")]
        public async Task<IActionResult> getRecommend()
        {
            var productRatings =await myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     .OrderByDescending(x => x.rating)
                     .Take(10)
                     .ToListAsync();
            return Ok(responeMessage.response200(productRatings));
        }
        [HttpGet("getRecommendByOrderRating/productId={productId}")]
        public async Task<IActionResult> getRecommendByOrderRating([Required] long productId)
        {
            var category_product =await myDbContext.Product_Categories.SingleOrDefaultAsync(x => x.productId == productId);
            var productRatings =await myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     
                     .OrderByDescending(x => x.rating)
                     .Take(10)
                     .ToListAsync();
            var productRatings2 = from pr in productRatings
                                  where (myDbContext.Product_Categories.Single(x => x.productId == pr.Product.id).categoryId == category_product.categoryId)
                                  select pr;
            return Ok(responeMessage.response200(productRatings2));
        }

        [HttpGet("getTopSell")]
        public async Task<IActionResult> getTopSell()
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products =await myDbContext.OrderItems
                     .GroupBy(pr => pr.productId)
                     .Select(g => new 
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         Quantity = g.Sum(pr => pr.quantity),
                         
                     })
                     .OrderByDescending(x => x.Quantity)
                     .Take(10)
                     .ToListAsync();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(new
                {
                    product=productRating.GetProductRating(product.Product, userId).Result.Product,
                    quantity=product.Quantity,
                    rating = productRating.GetProductRating(product.Product, userId).Result.rating,
                    wishlist = productRating.GetProductRating(product.Product, userId).Result.wishlist
                });
            }
            return Ok(responeMessage.response200(result));
        }

        [HttpGet("getReleases")]
        public async Task<IActionResult> getReleases()
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var products =await myDbContext.Products
                        
                     .OrderByDescending(x => x.createdAt)
                     .Take(10)
                     .ToListAsync();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(await productRating.GetProductRating(product, userId ));
            }
            return Ok(responeMessage.response200(result));
        }
        [HttpGet("getById/productId={productId}")]
        public async Task<IActionResult> getById([Required] long productId)
        {
            long userId = -1;
            if (this.User.FindFirstValue("Id") != null)
            {
                userId = long.Parse(this.User.FindFirstValue("Id"));
            }
            var product =await myDbContext.Products
                            .SingleOrDefaultAsync(x => x.id == productId)
                       ;
            if(product == null)
            {
                return Ok(responeMessage.response404);
            }
            var  result = (await productRating.GetProductRating(product, userId));
            
            return Ok(responeMessage.response200(result));
        }
        [HttpGet("getPerchased")]
        [Authorize]
        public async Task<IActionResult> getPerchased()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var orderItemss =await (from o in myDbContext.Orders
                               join oi in myDbContext.OrderItems on o.id equals oi.orderId

                               where o.userId == userId
                               select oi).ToListAsync();
            var products =(from oi in orderItemss
                            join p in myDbContext.Products on oi.productId equals p.id
                            select new Product().convert(p))
                            .GroupBy(x => x.id)
                            .Select(g => g.First())
                            .ToList();
            List<Object> result = new List<Object>();
            foreach (var product in products)
            {
                result.Add(await productRating.GetProductRating(product, userId ));
            }
            return Ok(responeMessage.response200(result));
           


        }
        
    }
}
