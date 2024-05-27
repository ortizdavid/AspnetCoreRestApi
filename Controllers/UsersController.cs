using System.Net;
using AspNetCoreRestApi.Helpers;
using AspNetCoreRestApi.Models;
using AspNetCoreRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repository;
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _tokenLength = 50;

        public UsersController(UserRepository repository, ILogger<UsersController> logger, IConfiguration configuration) 
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repository.GetAllAsync();
            if (!users.Any())
            {
                return NotFound("Users not found");
            }
            return Ok(users);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                if (await _repository.ExistsAsync(user.UserName))
                {
                    return Conflict($"User {user.UserName} already exists.");
                }
                user.Password = PasswordHelper.Hash(user.Password);
                await _repository.CreateAsync(user);
                _logger.LogInformation($"User {user.UserName} was created");
                return StatusCode(201, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }


        [HttpGet("name/{userName}")]
        public async Task<IActionResult> GetUserByUsername(string userName)
        {
            var user = await _repository.GetByUserNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                var existing = await _repository.GetByUserNameAsync(user.UserName);
                if (existing != null && existing.UserId != id)
                {
                    return Conflict($"User '{updatedUser.UserName}' is already in use.");
                }
                //user.UserName = updatedUser.UserName;
                user.Password = PasswordHelper.Hash(updatedUser.Password);
                user.Token = Encryption.GenerateRandomToken(_tokenLength);
                user.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(user);
                _logger.LogInformation($"User {user.UserName} was created");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            try
            {
                await _repository.DeleteAsync(user);
                _logger.LogInformation($"{user.UserName} was deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                if (user.IsActive == true)
                {
                    return Conflict("User is already active.");
                }
                user.IsActive = true;
                user.Token = Encryption.GenerateRandomToken(_tokenLength);
                user.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(user);
                _logger.LogInformation($"User {user.UserName} was activated.");
                return Ok($"User {user.UserName} activated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


         [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                if (user.IsActive == false)
                {
                    return Conflict("User is already inactive.");
                }
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(user);
                _logger.LogInformation($"User {user.UserName} was deactivated");
                return Ok($"User {user.UserName} activated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


    }
}