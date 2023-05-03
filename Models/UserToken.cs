namespace ShoppingCartGrapql.Models
{
    public class UserToken
    {
        public string Token { get; set; }
        public string ExpiredAt { get; set; }
        public string Message { get; set; }
    }
}
