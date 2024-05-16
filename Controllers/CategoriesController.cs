using System.Net;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _repository.GetAllAsync();
            if (!categories.Any())
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
                var existing = await _repository.GetByNameAsync(category.CategoryName);
                if (existing != null && existing.CategoryId != id)
                {
                    return Conflict($"Category '{category.CategoryName}' already exists.");
                }
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


        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCategorys(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                return BadRequest("Invalid file format. Please upload a CSV file.");
            }
            try
            {
                var categories = new List<Category>();
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    // Skip the header line
                    await reader.ReadLineAsync();
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var data = line.Split(',');
                        var categoryName = data[0];
                        // verify number of fields
                        if (data.Length != 2)
                        {
                            return BadRequest("Invalid CSV format. Each line must contain CategoryName, Description.");
                        }
                        if (await _repository.ExistsAsync(categoryName))
                        {
                            return BadRequest($"Category '{categoryName}' already exist");
                        }
                        var category = new Category
                        {
                            CategoryName = categoryName,
                            Description =  data[1],
                        };
                        categories.Add(category);
                    }
                }
                await _repository.CreateBatchAsync(categories);
                _logger.LogInformation($"Categories imported by CSV successfully: {categories.Count} lines");
                return StatusCode(201, $"Categories imported: {categories.Count} lines");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
