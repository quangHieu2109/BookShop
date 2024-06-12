using BookshopAPI.Models;
using BookshopAPI.Service;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Security.Claims;

namespace BookshopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();
        [HttpGet("getAllProduct")]
        public IActionResult getAllProduct()
        {
            var productRatings = myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new ProductRecommended
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     
                     .ToList();
            return Ok(productRatings);
        }
        [HttpGet("getWishList")]
        [Authorize]
        public IActionResult getWishList()
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist = (from w in myDbContext.WishListItems
                           where w.userId == userId
                           select w).ToList();
            List<Product> products = new List<Product>();
            foreach(var wish in wishlist)
            {
                products.Add(myDbContext.Products.SingleOrDefault(x=>x.id ==  wish.productId));

            }
            if(products.Count > 0)
            {
                var result = (from r in GetProductRecommended()
                             join p in products on r.Product.id equals p.id
                             select r).ToList();
                return Ok(result);
            }
            return NotFound();

        }
        [HttpGet("getProductByName")]
        public IActionResult getByName(string name)
        {
            var products = myDbContext.Products.Where(x =>
            x.name.Contains(name)).ToList();
            if (products != null)
            {
                var result = (from r in GetProductRecommended()
                              join p in products on r.Product.id equals p.id
                              select r).ToList();
                return Ok(result);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet("getSimilarProduct")]
        public IActionResult getSimilarProduct(long productId)
        {
            var category_product = myDbContext.Product_Categories.SingleOrDefault(x => x.productId == productId);
            if (category_product != null)
            {
                var products = (from p in myDbContext.Products
                               join c in myDbContext.Product_Categories on p.id equals c.productId
                               where c.categoryId == category_product.categoryId
                               select  new Product().convert(p)).ToList() ;
                
                if (products != null)
                {
                    var result = (from r in GetProductRecommended()
                                  join p in products on r.Product.id equals p.id
                                  select r).ToList();
                    return Ok(result);
                }
            }
            return NotFound();
        }


        [HttpGet("getProductByCategory{Id}")]
        public IActionResult getProductByCategoryId(long categoryId)
        {
            
            var products = from p in myDbContext.Products
                            join c in myDbContext.Product_Categories on p.id equals c.productId
                            where c.categoryId == categoryId
                            select  p ;
            if (products != null)
            {
                var result = (from r in GetProductRecommended()
                              join p in products on r.Product.id equals p.id
                              select r).ToList();
                return Ok(result);
            }
            
            return NotFound();
        }
        [HttpGet("getProductByCategory{Name}")]
        public IActionResult getProductByCategoryName(String categoryName)
        {
            var category = myDbContext.Categories.FirstOrDefault(x => x.name ==  categoryName);
            if(category != null)
            {
                var products = from p in myDbContext.Products
                               join c in myDbContext.Product_Categories on p.id equals c.productId
                               where c.categoryId == category.id
                               select p ;
                if (products != null)
                {
                    var result = (from r in GetProductRecommended()
                                  join p in products on r.Product.id equals p.id
                                  select r).ToList();
                    return Ok(result);
                }
            }

            return NotFound();
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
                return BadRequest("Mã sản phẩm không chính xác");
            }
            
            if (wishlist == null)
            {
                wishlist = new WishListItem
                {
                    userId = userId,
                    productId = productId,
                    createdAt = DateTime.Now
                };
                myDbContext.WishListItems.Add(wishlist);
                int rs = myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200(wishlist));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            return BadRequest(responeMessage.response400);

        }

        [HttpPost("deleteWishList/productId={productId}")]
        [Authorize]
        public IActionResult DeleteWishList(long productId)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var wishlist = myDbContext.WishListItems.SingleOrDefault(x => x.productId == productId && x.userId == userId);
            var product = myDbContext.Products.SingleOrDefault(x => x.id == productId);
            if (product == null)
            {
                return BadRequest("Mã sản phẩm không chính xác");
            }

            if (wishlist != null)
            {
                
                myDbContext.WishListItems.Remove(wishlist);
                int rs = myDbContext.SaveChanges();
                if (rs > 0)
                {
                    return Ok(responeMessage.response200);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, responeMessage.response500);
            }
            return BadRequest(responeMessage.response404);

        }
        [HttpGet("getByOrderRating")]
        public IActionResult getRecommend()
        {
            var productRatings = myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new ProductRecommended
                     {
                         Product = myDbContext.Products.Single(x => x.id == g.Key),
                         rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                     })
                     .OrderByDescending(x => x.rating)
                     .Take(10)
                     .ToList();
            return Ok(productRatings);
        }
        [HttpGet("getRecommendByOrderRating/productId={productId}")]
        public IActionResult getRecommendByOrderRating(long productId)
        {
            var category_product = myDbContext.Product_Categories.SingleOrDefault(x => x.productId == productId);
            var productRatings = myDbContext.ProductReviews
                     .GroupBy(pr => pr.productId)
                     .Select(g => new ProductRecommended
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
            return Ok(productRatings2);
        }

        [HttpGet("getTopSell")]
        public IActionResult getTopSell()
        {
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
            var result = (from r in GetProductRecommended()
                          join p in products on r.Product.id equals p.Product.id
                          select r).ToList();
            return Ok(result);
        }

        
        public List<ProductRecommended> GetProductRecommended()
        {
            var productRatings = myDbContext.ProductReviews
                    .GroupBy(pr => pr.productId)
                    .Select(g => new ProductRecommended
                    {
                        Product = myDbContext.Products.Single(x => x.id == g.Key),
                        rating = Math.Round(g.Average(pr => pr.ratingScore), 2)
                    })
                    .OrderByDescending(x => x.rating)
                    .Take(10)
                    .ToList();
            return (productRatings);
        }
    }
}
