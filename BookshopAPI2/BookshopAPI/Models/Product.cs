using BookshopAPI.Service;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopAPI.Models
{
    public class ProductVM
    {
        public string name { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public int quantity { get; set; }
        public string author { get; set; }
        public int pages { get; set; }
        public string publisher { get; set; }
        public int yearPublishing { get; set; }
        public string description { get; set; }
        public string imageName { get; set; }
        public int shop { get; set; }
        public DateTime createdAt { get; set; }
    }
    [Table("product")]
    public class Product: ProductVM
    {
        [Key]
        public long id { get; set; }

        public DateTime? updatedAt { get; set; }
        public DateTime? startsAt {  get; set; }
        public DateTime? endsAt {  get; set; }

        public Product convert(Product p)
        {
            return p;
        }
    }
    public class ProductRating
    {
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        public Product Product { get; set; }
        public double? rating { get; set; }
        public ProductRating GetProductRating(Product p)
        {
            var productReview = myDbContext.ProductReviews.FirstOrDefault(x => x.productId == p.id);
            if(productReview == null)
            {
                return new ProductRating
                {
                    Product = p
                };
            }
            var productRatings = myDbContext.ProductReviews
                    .Where(pr => pr.productId == p.id)
                    .Average(x => x.ratingScore);
            return new ProductRating
            {
                Product = p,
                rating = Math.Round(productRatings, 1)
            };
        }
    }
}
