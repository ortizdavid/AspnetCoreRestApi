﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;


namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllDataAsync();
            if (products.Count == 0)
            {
                return NotFound();
            }
            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            try
            {
                if (await _repository.ExistsAsync(product.Code))
                {
                    return BadRequest($"Product code: '{product.Code}' already exist");
                }
                await _repository.CreateAsync(product);
                _logger.LogInformation($"Product '{product.ProductName}' created");
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
        public async Task<IActionResult> UpdateProduct(int id, [FromBody]Product updatedProduct)
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
                _logger.LogInformation($"Product '{product.ProductName}' updated");
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
                _logger.LogInformation($"Product '{product.ProductName}' deleteted ");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
