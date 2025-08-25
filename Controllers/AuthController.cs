using Azure.Core;
using chess.api.dal;
using chess.api.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly UserDal _userDal = new UserDal();
        private static readonly AuthDal _authDal = new AuthDal();



        [HttpPost()]
        public async Task<IActionResult> Authenticate(UsernameAndPassword details)
        {
            var user = await _userDal.Authenticate(details.Username, details.Password);

            if(user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = GenerateToken(claims);
            var refreshToken = _authDal.AddRefreshToken(user.Id);
            AddRefreshCookie(refreshToken);

            return Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var refreshToken = new Guid(Request.Cookies["refreshToken"]); // if using cookies
                var storedTokenData = _authDal.RotateRefreshToken(refreshToken);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, storedTokenData.UserId.ToString()),
                    new Claim(ClaimTypes.Role, "User")
                };

                var newAccessToken = GenerateToken(claims);

                AddRefreshCookie(storedTokenData.Token);

                return Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken) });
            }catch(Exception ex)
            {
                return Ok(ex);
            }
        }

        private JwtSecurityToken GenerateToken(Claim[] claims)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("26edadf0-a811-4ca6-81ca-2e20a47f8b7b"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "club.chess.api",
                audience: "club.chess",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return token;
        }

        private void AddRefreshCookie(Guid refresh)
        {
            Response.Cookies.Append("refreshToken", refresh.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }



    public class UsernameAndPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
