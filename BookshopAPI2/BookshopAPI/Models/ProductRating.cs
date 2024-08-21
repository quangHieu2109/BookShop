using BookshopAPI.Service;
using Microsoft.EntityFrameworkCore;

namespace BookshopAPI.Models
{
    public class ProductRating
    {
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        public Product Product { get; set; }
        public double? rating { get; set; }
        public bool wishlist { get; set; }
        public async Task<ProductRating> GetProductRating(Product p, long userId)
        {
            bool _wishlist = await (myDbContext.WishListItems.FirstOrDefaultAsync(x => x.productId == p.id && x.userId == userId)) != null;
            var productReview = await myDbContext.ProductReviews.FirstOrDefaultAsync(x => x.productId == p.id);
            if (productReview == null)
            {
                return new ProductRating
                {
                    Product = p,
                    wishlist = _wishlist,
                };
            }
            else
            {
                var productRatings = await myDbContext.ProductReviews
                        .Where(pr => pr.productId == p.id)
                        .AverageAsync(x => x.ratingScore);

                return new ProductRating
                {
                    Product = p,
                    rating = Math.Round(productRatings, 1),
                    wishlist = _wishlist

                };
            }
        }
    }
}
