using HotChocolate.Authorization;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartGrapql.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace ShoppingCartGrapql
{
    public class Mutation
    {
        public string Register([Service] GraphqlStudyCaseDbContext context, RegisterUser registerUser)
        {
            // transaction
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    // tambah user
                    var u = new User
                    {
                        Username = registerUser.Username,
                        Fullname = registerUser.Fullname,
                        Password = BC.HashPassword(registerUser.Password)
                    };

                    // ambil role member
                    var role = context.Roles.Where(o => o.Name == "member").FirstOrDefault();
                    // assign role ke user
                    if (role != null)
                    {
                        var ur = new UserRole();
                        ur.User = u;
                        ur.Role = role;

                        context.UserRoles.Add(ur);
                        // simpan dan commit
                        context.SaveChanges();
                        trans.Commit(); //commit
                        return "Register succes";
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
            return "Register Failed";
        }

        public UserToken Login([Service] GraphqlStudyCaseDbContext context, [Service] IConfiguration configuration, UserLogin userLogin)
        {
            // linq
            var usr = context.Users.Where(o => o.Username == userLogin.Username).FirstOrDefault();
            if (usr != null)
            {
                if (BC.Verify(userLogin.Password, usr.Password))
                {
                    // login sukses
                    // ambil role
                    //var userroles = _context.UserRoles.Where(o => o.UserId == usr.Id).ToList();                       
                    // joins
                    var roles = from ur in context.UserRoles
                                join r in context.Roles
                                on ur.RoleId equals r.Id
                                where ur.UserId == usr.Id
                                select r.Name;

                    var roleClaims = new Dictionary<string, object>();
                    foreach (var role in roles)
                    {
                        roleClaims.Add(ClaimTypes.Role, "" + role);
                    }

                    var secret = configuration.GetValue<string>("AppSettings:Secret");
                    var secretBytes = Encoding.ASCII.GetBytes(secret);

                    // token
                    var expired = DateTime.Now.AddDays(2); // 2 hari
                    var tokenHandler = new JwtSecurityTokenHandler();
                    // data
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        // payload
                        Subject = new System.Security.Claims.ClaimsIdentity(
                            new Claim[]
                            {
                                new Claim(ClaimTypes.Name, userLogin.Username)
                            }),
                        Claims = roleClaims, // claims - roles
                        Expires = expired,
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(secretBytes),
                            SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var userToken = new UserToken
                    {
                        Token = tokenHandler.WriteToken(token),
                        ExpiredAt = expired.ToString(),
                        Message = "succes login"
                    };

                    return userToken;

                }
            }
            return new UserToken { Message = "Invalid username or password" };
        }

        [Authorize(Roles = new[] {"admin"} )]
        public Product Create([Service] GraphqlStudyCaseDbContext context, Product product)
        {
            var newProduct = new Product { Name = product.Name , Price = product.Price , Stock = product.Stock , Deleted = false };
            context.Products.Add(newProduct);
            context.SaveChanges();
            return product;
        }

        [Authorize(Roles = new[] { "admin" })]
        public Product Update([Service] GraphqlStudyCaseDbContext context, Product product)
        {
            var Oldproduct = context.Products.FirstOrDefault(o => o.Id == product.Id );
            Oldproduct.Name = product.Name;
            Oldproduct.Stock = product.Stock;
            Oldproduct.Price = product.Price;
            context.Products.Update(Oldproduct);
            context.SaveChanges();
            return Oldproduct;
        }

        [Authorize(Roles = new[] { "admin" })]
        public string Delete([Service] GraphqlStudyCaseDbContext context, int id)
        {
            var product = context.Products.FirstOrDefault(o => o.Id == id);
            product.Deleted = true;
            context.SaveChanges();
            return "Product deleted";
        }
    }
}
