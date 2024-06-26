using AspNetCoreRestApi.Models;
using AspNetCoreRestApi.Repositories;
using BankCoreApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly SupplierRepository _repository;
        private readonly ProductRepository _productRepository;
        private readonly ILogger<SuppliersController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public SuppliersController(IConfiguration configuration, ILogger<SuppliersController> logger, 
            SupplierRepository repository, ProductRepository productRepository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _productRepository = productRepository;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var count = await _repository.CountAsync();
                if (count == 0)
                {
                    return NotFound("No suppliers found.");
                }
                var products = await _repository.GetAllAsync(pageSize, pageIndex);
                var pagination = new Pagination<Supplier>(products, count, pageIndex, pageSize, _httpContextAccessor);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            if (supplier is null)
            {
                return BadRequest();
            }
            try
            {
                if (await _repository.ExistsAsync(supplier.IdentificationNumber))
                {
                    return BadRequest($"Supplier identification: '{supplier.SupplierName}' already exists.");
                }
                await _repository.CreateAsync(supplier);
                _logger.LogInformation($"Supplier '{supplier.SupplierName}' created.");
                return StatusCode(201, supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier is null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier updatedSupplier)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier is null)
            {
                return NotFound();
            }
            try
            {
                var existing = await _repository.GetByIdentNumberAsync(updatedSupplier.IdentificationNumber);
                if (existing != null && existing.SupplierId != id)
                {
                    return Conflict($"Supplier identification: '{supplier.IdentificationNumber}' already exists.");
                }
                supplier.SupplierName = updatedSupplier.SupplierName;
                supplier.Address = updatedSupplier.Address;
                supplier.PrimaryPhone = updatedSupplier.PrimaryPhone;
                supplier.SecondaryPhone = updatedSupplier.SecondaryPhone;
                supplier.Email = updatedSupplier.Email;
                supplier.Address = updatedSupplier.Address;
                supplier.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(supplier);
                _logger.LogInformation($"Supplier '{supplier.SupplierName}' updated.");
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier is null)
            {
                return NotFound();
            }
            try
            {
                await _repository.DeleteAsync(supplier);
                _logger.LogInformation($"Supplier '{supplier.SupplierName}' deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetSupplierProducts(int id, int pageIndex = 1, int pageSize = 10)
        {
           
            try
            {
                var count = await _productRepository.CountBySupplierAsync(id);
                if (count == 0)
                {
                    return NotFound("No suppliers found.");
                }
                var products = await _productRepository.GetAllBySupplierAsync(id, pageSize, pageIndex);
                var pagination = new Pagination<Product>(products, count, pageIndex, pageSize, _httpContextAccessor);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportSuppiersCSV(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                return BadRequest("Invalid file format. Please upload a CSV file.");
            }
            try
            {
                var suppliers = new List<Supplier>();
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    // Skip the header line
                    await reader.ReadLineAsync();
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var data = line.Split(',');
                        var supplierName = data[0];
                        var identification = data[1];
                        var email = data[2];
                        var primaryPhone = data[3];
                        var secondaryPhone = data[4];
                        var address = data[5];
                        // verify number of fields
                        if (data.Length != 6)
                        {
                            return BadRequest("Invalid CSV format. Each line must contain SupplierName,IdentificationNumber,Email,PrimaryPhone,SecondaryPhone,Address.");
                        }
 
                        //verify if exists
                        if (await _repository.ExistsRecord("identification_number", identification))
                        {
                            return BadRequest($"Supplier Identification Number '{identification}' already exists.");
                        }
                        if (await _repository.ExistsRecord("email", email))
                        {
                            return BadRequest($"Supplier Email '{email}' already exists.");
                        }
                        if (await _repository.ExistsRecord("primary_phone", primaryPhone))
                        {
                            return BadRequest($"Supplier Primary Phone '{primaryPhone}' already exists.");
                        }
                        if (await _repository.ExistsRecord("secondary_phone", secondaryPhone))
                        {
                            return BadRequest($"Supplier Secondary Phone '{secondaryPhone}' already exists.");
                        }
                        var supplier = new Supplier
                        {
                            SupplierName = supplierName,
                            IdentificationNumber = identification,
                            Email = email,
                            PrimaryPhone = primaryPhone,
                            SecondaryPhone = secondaryPhone,
                            Address = address
                        };
                        suppliers.Add(supplier);
                    }
                }
                await _repository.CreateBatchAsync(suppliers);
                _logger.LogInformation($"Suppliers imported by CSV successfully: {suppliers.Count} lines");
                return StatusCode(201, $"Suppliers imported: {suppliers.Count} lines");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}