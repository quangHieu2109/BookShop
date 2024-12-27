using BookshopAPI.Models;
using BookshopAPI.Service;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using Newtonsoft.Json.Linq;
using System.Management;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using BookshopAPI.Database;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace BookshopAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IConfiguration configuration = new MyDbContextService().GetConfiguration();
        private MyDbContext myDbContext = new MyDbContextService().GetMyDbContext();
        private ResponeMessage responeMessage = new ResponeMessage();

        [HttpGet("getAllUser")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> getAll()
        {

            return Ok(responeMessage.response200(myDbContext.Users));
        }

        [HttpGet("getInfor")]
        [Authorize]
        public async Task<IActionResult> getInfor()
        {

            long userId = long.Parse(this.User.FindFirstValue("Id"));
            var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.id == userId);
            return Ok(responeMessage.response200(user));
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> changePassword(string password)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            if (password.IndexOf(" ") != -1)
            {
                return Ok(responeMessage.response400(null, "Mật khẩu không hợp lệ"));
            }
            else
            {
                var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.id == userId);
                if (user.password == Hash(password))
                {
                    return Ok(responeMessage.response400(null, "Mật khẩu mới trùng với mâtj khẩu cũ"));

                }
                else
                {
                    user.password = Hash(password);
                    await myDbContext.SaveChangesAsync(); ;
                    return Ok(responeMessage.response200);
                }

            }
        }
        [HttpPost("changePasswordByAdmin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> changePasswordByAdmin(string username, string password)
        {

            var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.username == username);

            if (user != null)
            {
                user.password = Hash(password);

                await myDbContext.SaveChangesAsync(); ;
                return Ok(responeMessage.response200(null, "Đổi mật khẩu thành công"));
            }

            return Ok(responeMessage.response400(null, "Username không chính xác"));


        }

        [HttpPost("changeInfor")]
        [Authorize]
        public async Task<IActionResult> ChangeInfor(UserInfor userInfor)
        {
            long userId = long.Parse(this.User.FindFirstValue("Id"));
            User user = await myDbContext.Users.SingleOrDefaultAsync(x => x.email == userInfor.email);
            if (user != null)
            {
                return Ok(responeMessage.response400(null, "Email đã tồn tại"));
            }

             user = await myDbContext.Users.SingleOrDefaultAsync(x => x.id == userId);

            user.fullName = userInfor.fullName;
            user.phoneNumber = userInfor.phoneNumber;
            user.email = userInfor.email;
            user.gender = userInfor.gender;
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(user, validationContext, validationResults, true);
            if (validationResults.Count == 0)
            {
                await myDbContext.SaveChangesAsync();
                var email =await myDbContext.Emails.SingleOrDefaultAsync(x => x.userId == userId);
                if(email == null)
                {
                    email = new Email
                    {
                        uid = userInfor.uid,
                        email = userInfor.email,
                        userId = userId
                    };
                    await myDbContext.Emails.AddAsync(email);
                }
                else
                {
                    email.uid = userInfor.uid;
                    email.email = userInfor.email;
                }
                await myDbContext.SaveChangesAsync();
                return Ok(responeMessage.response200(user));
            }
            else
            {
                return Ok(responeMessage.response400(null, convertValidationResult(validationResults)));
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> login(UserLogin userLogin)
        {
            var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.username == userLogin.username);
            if (user == null)
            {
                return BadRequest(responeMessage.response400("Tài khoản không chính xác"));
            }
            else
            {
                if (user.password != Hash(userLogin.password))
                {
                    return BadRequest(responeMessage.response400("Mật khẩu không chính xác"));
                }
                else
                {
                    var accessToken = await generateToken(user);
                    return Ok(responeMessage.response200(accessToken, "Đăng nhập thành công"));
                }
            }
        }

        [HttpPost("sendOTP/email:{email}")]
        public async Task<IActionResult> sendOTP(string email)
        {
            if (await myDbContext.Users.SingleOrDefaultAsync(x => x.email == email) != null)
            {
                var senMail = new SendMail();
                var rd = new Random();
                string otp = "";
                for (int i = 0; i < 6; i++)
                {
                    otp += rd.Next(0, 9);
                }
                senMail.SendEmail(email, otp);
                var sendOtp = await myDbContext.OPTs.SingleOrDefaultAsync(x => x.email == email);
                if (sendOtp != null)
                {
                    myDbContext.OPTs.Remove(sendOtp);
                    await myDbContext.SaveChangesAsync();
                }
                sendOtp = new OTP
                {
                    email = email,
                    otp = otp,
                    accuracy = 0,
                    endAt = DateTime.Now.AddMinutes(5)
                };
                myDbContext.OPTs.Add(sendOtp);
                await myDbContext.SaveChangesAsync(); ;
                return Ok(responeMessage.response200(null, "Gửi OTP thành công"));
            }
            else
            {
                return Ok(responeMessage.response400(null, "Email chưa được đăng ký tài khoản"));
            }
        }
        [HttpPost("accuracyOTP")]
        public async Task<IActionResult> accuracyOtp(AccuracyOtp accuracyOtp)
        {
            var otp = await myDbContext.OPTs.SingleOrDefaultAsync(x => x.email == accuracyOtp.email);
            if (otp == null)
            {
                return Ok(responeMessage.response400(null, "Email không chính xác"));
            }
            else
            {
                if (otp.accuracy == 1)
                {
                    return Ok(responeMessage.response200);
                }
                else
                {
                    if (otp.endAt < DateTime.Now)
                    {
                        return Ok(responeMessage.response400(null, "OTP đã hết hiệu lực"));
                    }
                    else
                    {
                        if (otp.otp != accuracyOtp.otp)
                        {
                            return Ok(responeMessage.response400(null, "OTP không chính xác"));
                        }
                        else
                        {
                            otp.accuracy = 1;
                            otp.endAt = DateTime.Now.AddMinutes(5);
                            await myDbContext.SaveChangesAsync(); ;
                            return Ok(responeMessage.response200(null, "Xác thực OTP thành công"));
                        }
                    }
                }
            }
        }
        [HttpPost("changePasswordByOTP")]
        public async Task<IActionResult> changePasswordByOTP(ChangePasswordOtp changePasswordOtp)
        {
            var otp = await myDbContext.OPTs.SingleOrDefaultAsync(x => x.email == changePasswordOtp.email);
            if (otp == null)
            {
                return Ok(responeMessage.response400(null, "Email không chính xác"));
            }
            else
            {
                if (otp.accuracy == 0)
                {
                    return Ok(responeMessage.response400(null, "OTP chưa được xác thực"));
                }
                else
                {
                    if (otp.endAt < DateTime.Now)
                    {
                        return Ok(responeMessage.response400(null, "OTP đã hết hạn"));
                    }
                    else
                    {
                        var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.email == otp.email);
                        user.password = Hash(changePasswordOtp.password);
                        myDbContext.OPTs.Remove(otp);
                        await myDbContext.SaveChangesAsync(); ;
                        return Ok(responeMessage.response200(null, "Đổi mật khẩu thành công"));
                    }
                }
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> register(UserRegister userRegister)
        {
            var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.username == userRegister.username);
            if (user != null)
            {
                return BadRequest(responeMessage.response400("Username đã tồn tại!"));
            }
            else
            {
                user = await myDbContext.Users.SingleOrDefaultAsync(x => x.email == userRegister.email);
                if (user != null)
                {
                    return Ok(responeMessage.response400("Email đã tồn tại!"));
                }
                else
                {
                    user = new User
                    {
                        id = DateTime.Now.ToFileTimeUtc(),
                        username = userRegister.username,
                        password = Hash(userRegister.password),
                        createAt = DateTime.Now,
                        fullName = userRegister.fullName,
                        email = userRegister.email,
                        role = "CUSTOMER"
                    };

                    var validationContext = new ValidationContext(user);
                    var validationResults = new List<ValidationResult>();
                    Validator.TryValidateObject(user, validationContext, validationResults, true);
                    if (validationResults.Count == 0)
                    {
                        await myDbContext.Users.AddAsync(user);
                        var rs = await myDbContext.SaveChangesAsync(); ;
                        if (rs > 0)
                        {
                            var cart = new Cart
                            {
                                id = DateTime.Now.ToFileTimeUtc(),
                                userId = user.id,
                                createdAt = DateTime.Now

                            };
                            await myDbContext.Carts.AddAsync(cart);
                            await myDbContext.SaveChangesAsync(); ;
                            var email = new Email
                            {
                                userId = user.id,
                                email = user.email,
                                uid = userRegister.uid
                            };
                            await myDbContext.Emails.AddAsync(email);
                            await myDbContext.SaveChangesAsync();
                            return Ok(responeMessage.response200(user, "Đăng ký thành công"));
                        }
                        else
                        {
                            return Ok(responeMessage.response500);
                        }
                    }
                    else
                    {
                        return Ok(responeMessage.response400(null, convertValidationResult(validationResults)));
                    }

                }
            }

        }
        [HttpPost("loginGoogleUser")]
        public async Task<IActionResult> loginGoogle(Token token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSecurityToken = handler.ReadJwtToken(token.googleToken);

                var claim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "email");

                if (claim != null)
                {
                    var email = claim?.Value;
                    var fullName = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    var user = myDbContext.Users.SingleOrDefault(x => x.email == email);
                    if (user == null)
                    {
                        user = new User
                        {
                            email = email,
                            fullName = fullName,
                            role = "CUSTOMER",
                            createAt = DateTime.Now,
                            gender = 1
                        };
                        await myDbContext.Users.AddAsync(user);
                        await myDbContext.SaveChangesAsync();



                    }
                    var uid = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                    user = await myDbContext.Users.SingleOrDefaultAsync(x => x.email == email);
                    var emailObj = await myDbContext.Emails.SingleOrDefaultAsync(x => x.email == email);
                    if (emailObj == null)
                    {
                        emailObj = new Email
                        {
                            uid = uid,
                            email = email,
                            userId = user.id
                        };
                    }
                    await myDbContext.Emails.AddAsync(emailObj);
                    await myDbContext.SaveChangesAsync();
                    var accessToken = await generateToken(user);
                    return Ok(responeMessage.response200(accessToken, "Đăng nhập thành công"));

                }
                else
                {
                    return Ok(responeMessage.response400("Token không chính xác!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(responeMessage.response400("Token không hợp lệ!"));

            }

        }

        [HttpPost("loginFacebookUser")]
        public async Task<IActionResult> loginFacebook(FacebookUserLogin facebookUserLogin)
        {
            string url = $"https://graph.facebook.com/debug_token?input_token={facebookUserLogin.inputToken}&access_token={facebookUserLogin.accessToken}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Phân tích JSON
                    JsonResponse responseJson = JsonConvert.DeserializeObject<JsonResponse>(responseBody);

                    // Lấy user_id
                    string uid = responseJson.data.user_id;
                    var email = await myDbContext.Emails.SingleOrDefaultAsync(x => x.uid == uid);
                    if (email != null)
                    {
                        var user = await myDbContext.Users.SingleOrDefaultAsync(x => x.id == email.userId);
                        var accessToken = await generateToken(user);
                        return Ok(responeMessage.response200(accessToken, "Đăng nhập thành công"));
                    }
                    else
                    {
                        var user = new User
                        {
                            id = DateTime.Now.ToFileTimeUtc(),

                            createAt = DateTime.Now,
                            fullName = facebookUserLogin.name,
                            email = facebookUserLogin.email,
                            role = "CUSTOMER",
                            gender =0
                        };


                        await myDbContext.Users.AddAsync(user);
                        var rs = await myDbContext.SaveChangesAsync(); ;
                        if (rs > 0)
                        {
                            var cart = new Cart
                            {
                                id = DateTime.Now.ToFileTimeUtc(),
                                userId = user.id,
                                createdAt = DateTime.Now
                               

                            };
                            await myDbContext.Carts.AddAsync(cart);
                            await myDbContext.SaveChangesAsync(); ;
                            email = new Email
                            {
                                userId = user.id,
                                email = user.email,
                                uid = uid
                            };
                            await myDbContext.Emails.AddAsync(email);
                            await myDbContext.SaveChangesAsync();
                            var accessToken = await generateToken(user);
                            return Ok(responeMessage.response200(accessToken, "Đăng nhập thành công"));
                        }
                        else
                        {
                            return Ok(responeMessage.response500);
                        }
                    }



                   

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return StatusCode(500, "Có lỗi xảy ra trong quá trình xử lý yêu cầu.");
                }
            }

        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var refreshTK = await myDbContext.RefreshTokens.SingleOrDefaultAsync(x => x.refreshToken == refreshToken);
            if (refreshTK == null)
            {
                return Ok(responeMessage.response400(null, "RefreshToken không chính xác!"));
            }
            else
            {
                if (refreshTK.endAt < DateTime.Now)
                {
                    return Ok(responeMessage.response400(null, "RefreshToken đã hết hạn!"));
                }
                else
                {
                    User user = await myDbContext.Users.SingleOrDefaultAsync(x => x.id == refreshTK.userId);
                    LoginResponse loginResponse = await generateToken(user);
                    return Ok(responeMessage.response200(loginResponse));

                }
            }

        }
        private async Task<LoginResponse> generateToken(User user)
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
                    new Claim("Id", user.id+"")

                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = jwtTokenHandler.CreateToken(tokenDesciption);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshTK = myDbContext.RefreshTokens.SingleOrDefault(x => x.userId == user.id);
            if (refreshTK == null)
            {
                refreshTK = new RefreshToken
                {
                    userId = user.id,
                    refreshToken = Guid.NewGuid().ToString()

                };
                await myDbContext.RefreshTokens.AddAsync(refreshTK);

                await myDbContext.SaveChangesAsync(); ;
            }
            refreshTK.endAt = DateTime.UtcNow.AddDays(30);
            //refreshTK.refreshToken = Guid.NewGuid().ToString();
            await myDbContext.SaveChangesAsync(); ;
            return new LoginResponse
            {
                accessToken = accessToken,
                refreshToken = refreshTK.refreshToken
            };
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
        public static string convertValidationResult(List<ValidationResult> validationResults)
        {
            string result = "";

            foreach (var validationResult in validationResults)
            {
                result += validationResult.ToString() + "\n";
            }
            return result;
        }


    }
}
