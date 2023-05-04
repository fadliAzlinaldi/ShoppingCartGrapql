using HotChocolate.Authorization;
using Microsoft.OpenApi.Validations;
using ShoppingCartGrapql.Models;
using System.Security.Claims;

namespace ShoppingCartGrapql
{
    public class Query
    {
        public IQueryable<Product> GetProducts([Service]GraphqlStudyCaseDbContext context) => context.Products.Where(o => o.Deleted == false);
        public Product GetProductDetails([Service]GraphqlStudyCaseDbContext context, int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null && product.Deleted != true)
            {
                return product;
            }
            else
            {
                throw new ArgumentException("product not found");
            }
        }

        [Authorize(Roles = new[] { "member" })]
        public IQueryable<CartItem>? GetCartItems ([Service]GraphqlStudyCaseDbContext context, HttpContext httpContext)
        {
            // get username
            var user = httpContext.User.FindFirstValue(ClaimTypes.Name);
            if (user != null)
            {
                var cart = context.UserCarts.Where(o => o.User.Username == user && o.Checkout == false).FirstOrDefault();
                if ( cart != null )
                {
                    var cartItems = context.CartItems.Where(o => o.UserCartId == cart.Id);
                    return cartItems;
                }
               
            }
            return null;
        }
    }
}
