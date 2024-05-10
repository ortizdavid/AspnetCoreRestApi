using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetCoreRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            if (!IsValidUser(model.Username, model.Password))
            {
                return Unauthorized();
            }
            var token = GenerateJwtToken(model.Username);
            return Ok(new { token });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logged out successfully!");
        }

        private bool IsValidUser(string? username, string? password)
        {
            // Implement your authentication logic here (e.g., validate credentials against a database)
            // This is just a placeholder implementation for demonstration purposes
            return username == "example_user" && password == "example_password";
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}