using System.Net;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;
using AspNetCoreRestApi.Helpers;


namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ProductRepository _repository;
        private readonly ImageRepository _imageRepository;
        private readonly ILogger<ProductsController> _logger;
        private readonly FileUploader _imageUploader;
        private readonly FileUploader _csvUploader;

        public ProductsController(IConfiguration configuration, ProductRepository repository, ImageRepository imageRepository, ILogger<ProductsController> logger)
        {
            _configuration = configuration;
            _repository = repository;
            _imageRepository = imageRepository;
            _logger = logger;

            var uploadsDirectory = _configuration["UploadsDirectory"];
            _imageUploader = new FileUploader(uploadsDirectory, FileExtensions.Images, 5 * CapacityUnit.MEGA_BYTE);;
            _csvUploader = new FileUploader(uploadsDirectory, FileExtensions.CsvTxts, 5 * CapacityUnit.MEGA_BYTE);;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllDataAsync();
            if (!products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            try
            {
                if (await _repository.ExistsAsync(product.Code))
                {
                    return BadRequest($"Product code: '{product.Code}' already exist.");
                }
                await _repository.CreateAsync(product);
                _logger.LogInformation($"Product '{product.ProductName}' created.");
                return StatusCode(201, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetDataByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(product);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            try
            {
                var existing = await _repository.GetByCodeAsync(product.Code);
                if (existing != null && existing.ProductId != id)
                {
                    _logger.LogError($"Product {product.Code} already exists.");
                    return Conflict($"Product code '{updatedProduct.Code}' is already in use by another product.");
                }

                product.ProductName = updatedProduct.ProductName;
                product.UnitPrice = updatedProduct.UnitPrice;
                product.Description = updatedProduct.Description;
                product.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(product);
                _logger.LogInformation($"Product '{product.ProductName}' updated.");
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            try
            {
                await _repository.DeleteAsync(product);
                _logger.LogInformation($"Product '{product.ProductName}' deleteted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddProductImages(int id, List<IFormFile> files)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files selected.");
            }
            if (files.Count > 4) 
            {
                return BadRequest("Only 4 images are allowed: front, back, left and right.");
            }
            try
            {
                var imagesInfo = await _imageUploader.UploadMultipleFiles(HttpContext, "files");
                var imagesName = imagesInfo.Select(item => item.FinalName).ToList();

                var image = new Image
                {
                    ProductId = product.ProductId,
                    BackImage = imagesName.ElementAtOrDefault(0),
                    FrontImage = imagesName.ElementAtOrDefault(1),
                    LeftImage = imagesName.ElementAtOrDefault(2),
                    RightImage = imagesName.ElementAtOrDefault(3),
                    UploadDir = _configuration["UploadsDirectory"]
                };
                await _imageRepository.CreateAsync(image);
                _logger.LogInformation($"Images for Product '{product.ProductName}' uploaded.");
                return Ok(imagesInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetProductImages(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                var images = await _imageRepository.GetAllAsync(product.ProductId);
                if (!images.Any())
                    return NotFound();
                return Ok(images);
            }
        }

    }
}
