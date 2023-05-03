using System;
using System.Collections.Generic;

namespace ShoppingCartGrapql.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string Stock { get; set; } = null!;

    public bool Deleted { get; set; }
}
