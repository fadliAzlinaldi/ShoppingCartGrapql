using HotChocolate.Authorization;
using ShoppingCartGrapql.Models;

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
    }
}
