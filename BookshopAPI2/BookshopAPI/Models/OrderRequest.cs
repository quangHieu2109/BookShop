namespace BookshopAPI.Models
{
    public class OrderRequest
    {
        public int deliveryMethod {  get; set; }
        public double deliveryPrice {  get; set; }
        public List<long> cartItemIds { get; set; }

    }
}
