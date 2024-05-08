using System.Net;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository _repository;
        private readonly ILogger<CategoriesController> _logger;
        
        public CategoriesController(CategoryRepository repository, ILogger<CategoriesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _repository.GetAllAsync();
            if (categories.Count == 0)
            {
                return NotFound();
            }
            return Ok(categories);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest();
            }
            try
            {
                if (await _repository.ExistsAsync(category.CategoryName))
                {
                    return BadRequest($"Category '{category.CategoryName}' already exists.");
                }
                await _repository.CreateAsync(category);
                _logger.LogInformation($"Category '{category.CategoryName}' created.");
                return StatusCode(201, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            try
            {
                category.CategoryName = updatedCategory.CategoryName;
                category.Description = updatedCategory.Description;

                category.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(category);
                _logger.LogInformation($"Category '{updatedCategory.CategoryName}' updated.");
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            try
            {
                await _repository.DeleteAsync(category);
                _logger.LogInformation($"Category '{category.CategoryName}' deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
