using BookshopAPI.Models;
using BookshopAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookshopAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
       private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();

        [HttpGet("getAllUser")]
        [Authorize(Roles ="ADMIN")]
        public IActionResult getAll()
        {
            
            return Ok(this.User.FindFirstValue("UserName"));
        }
        [HttpPost("login")]
        public IActionResult login(UserLogin userLogin) {
               var user = myDbContext.Users.SingleOrDefault(x => x.username == userLogin.username);
            if (user == null)
            {
                return BadRequest("Username không chính xác");
            }
            else
            {
                if(user.password != Hash(userLogin.password))
                {
                    return BadRequest("Password không chính xác");
                }
                else
                {
                    var accessToken = generateToken(user);
                    return Ok(new
                    {
                        Success = true,
                        Message = "Đăng nhập thành công!",
                        Data = accessToken
                    }) ;
                }
            }
        }
        [HttpPost("register")]
        public IActionResult register(UserRegister userRegister) {
            var user = myDbContext.Users.SingleOrDefault(x => x.username == userRegister.username);
            if (user != null) {
                return BadRequest("Username đã tồn tại!");
            }else
            {
                user = myDbContext.Users.SingleOrDefault(x => x.email == userRegister.email);
                if (user != null)
                {
                    return BadRequest("Email đã tồn tại!");
                }
                else
                {
                    user = new User { id = DateTime.Now.ToFileTimeUtc(),
                        username = userRegister.username,
                        password = Hash(userRegister.password),
                        createAt = DateTime.Now,
                        fullName = userRegister.fullName, 
                        email = userRegister.email, 
                        role = "CUSTOMER" };
                    myDbContext.Users.Add(user);
                    var rs =  myDbContext.SaveChanges();
                    if (rs > 0)
                    {
                        var cart = new Cart
                        {
                            id = DateTime.Now.ToFileTimeUtc(),
                            userId = user.id,
                            createdAt = DateTime.Now

                        };
                        myDbContext.Carts.Add(cart);
                        myDbContext.SaveChanges();
                        
                        return Ok(user);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi từ server, vui lòng thử lại sau!");
                    }
                }
            }
            
        }

        private String generateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["AppSettings:SecretKey"]);
            var tokenDesciption = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.fullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.role),

                    new Claim("UserName", user.username),
                     new Claim("Id", user.id+""),
                    new Claim("Password", user.password)
                    // role

                    //token
                    

                }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = jwtTokenHandler.CreateToken(tokenDesciption);
            var accessToken = jwtTokenHandler.WriteToken(token);
            
            return accessToken;
        }
        public static string Hash(string s)
        {
            string hashed = "";
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    hashed = sb.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return hashed;
        }
    }
}
