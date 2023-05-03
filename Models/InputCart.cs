namespace ShoppingCartGrapql.Models
{
    public class InputCart
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }   
    }
}
