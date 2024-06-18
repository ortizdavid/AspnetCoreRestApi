using Microsoft.AspNetCore.Mvc;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;
using AspNetCoreRestApi.Helpers;
using System.Xml.Serialization;
using BankCoreApi.Helpers;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IConfiguration configuration, ProductRepository repository, 
            ImageRepository imageRepository, ILogger<ProductsController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _repository = repository;
            _imageRepository = imageRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _imageUploader = new FileUploader(_configuration["UploadsDirectory"], FileExtensions.Images, 5 * CapacityUnit.MEGA_BYTE);;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var count = await _repository.CountDataAsync();
                if (count == 0)
                {
                    return NotFound("No products found.");
                }
                var products = await _repository.GetAllDataAsync(pageSize, pageIndex);
                var pagination = new Pagination<ProductData>(products, count, pageIndex, pageSize, _httpContextAccessor);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
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
                    return Conflict($"Product code '{updatedProduct.Code}' is already in use by another product.");
                }
                product.ProductName = updatedProduct.ProductName;
                product.UnitPrice = updatedProduct.UnitPrice;
                product.Description = updatedProduct.Description;
                product.CategoryId = updatedProduct.CategoryId;
                product.SupplierId = updatedProduct.SupplierId;
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
                return Ok($"Images for Product '{product.ProductName}' uploaded.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
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

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
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
                var products = new List<Product>();
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    // Skip the header line
                    await reader.ReadLineAsync();
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var data = line.Split(',');
                        var productCode = data[1];
                        // verify number of fields
                        if (data.Length != 5)
                        {
                            return BadRequest("Invalid CSV format. Each line must contain ProductName,Code,UnitPrice,CategoryId,SupplierId.");
                        }
                        // verify csv format
                        if (!float.TryParse(data[2], out float unitPrice) || !int.TryParse(data[3], out int categoryId) || !int.TryParse(data[4], out int supplierId))
                        {
                            return BadRequest("Invalid CSV format. UnitPrice and CategoryId and SupplierId must be numeric.");
                        }
                        //verify if exists
                        if (await _repository.ExistsAsync(productCode))
                        {
                            return BadRequest($"Product code '{productCode}' already exists");
                        }
                        var product = new Product
                        {
                            ProductName = data[0],
                            Code = productCode,
                            UnitPrice = float.Parse(data[2]),
                            CategoryId = int.Parse(data[3]),
                            SupplierId = int.Parse(data[4]) 
                        };
                        products.Add(product);
                    }
                }
                await _repository.CreateBatchAsync(products);
                _logger.LogInformation($"Products imported by CSV successfully: {products.Count} lines");
                return StatusCode(201, $"Products imported: {products.Count} lines");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("xml")]
        public async Task<IActionResult> GetAllProductsXml(int pageIndex = 1, int pageSize = 0)
        {
            try
            {
                var count = await _repository.CountDataAsync();
                if (count == 0)
                {
                    return NotFound("No products found.");
                }
                int offset = (pageIndex - 1) * pageSize;
                var products = await _repository.GetAllDataAsync(pageSize, offset);
                
                var pagination = new Pagination<ProductData>(products, count, pageIndex, pageSize, _httpContextAccessor);

                var xmlSerializer = new XmlSerializer(typeof(List<ProductData>));
                var xmlContent = new StringWriter();
                xmlSerializer.Serialize(xmlContent, products.ToList());

                return Content(xmlContent.ToString(), "application/xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}/xml")]
        public async Task<IActionResult> GetProductXml(int id)
        {
           
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var xmlSerializer = new XmlSerializer(typeof(Product));
            var xmlContent = new StringWriter();
            xmlSerializer.Serialize(xmlContent, product);
            return Content(xmlContent.ToString(), "application/xml");
        }
    }
}
