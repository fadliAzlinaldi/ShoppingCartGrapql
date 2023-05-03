using System;
using System.Collections.Generic;

namespace ShoppingCartGrapql.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int UserCartId { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public int Quantity { get; set; }

    public virtual UserCart UserCart { get; set; } = null!;
}
