using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetCoreRestApi.Helpers;
using AspNetCoreRestApi.Models;
using AspNetCoreRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(UserRepository userRepository, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (!IsValidUser(login.Username, login.Password))
            {
                return Unauthorized("Username or Password incorrect!");
            }
            var token = GenerateJwtToken(login.Username);
            return Ok(new {token} );
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logged out successfully!");
        }


        private bool IsValidUser(string? userName, string? password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            var user = _userRepository.GetByUserName(userName);
            if (user == null)
            {
                return false;
            }
            return PasswordHelper.Verify(password, user.Password);
        }


        private string GenerateJwtToken(string userName)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);
            var audience = jwtSettings["Audience"];
            var expiryMinutes = Convert.ToInt32(jwtSettings["ExpiryMinutes"]);
            var issuer = jwtSettings["Issuer"]; // Retrieve issuer from configuration


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = audience,
                Issuer = issuer // Set the issuer claim in the token
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}