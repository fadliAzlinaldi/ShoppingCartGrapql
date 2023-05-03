using System;
using System.Collections.Generic;

namespace ShoppingCartGrapql.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
