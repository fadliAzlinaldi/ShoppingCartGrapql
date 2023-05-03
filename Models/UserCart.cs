using System;
using System.Collections.Generic;

namespace ShoppingCartGrapql.Models;

public partial class UserCart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool Checkout { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}
