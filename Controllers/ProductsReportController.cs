using Microsoft.AspNetCore.Mvc;
using System.Text;
using CsvHelper;
using System.Globalization;
using AspNetCoreRestApi.Repositories;
using AspNetCoreRestApi.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReportsController : ControllerBase
    {

        private readonly ProductReportRepository _repository;
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductReportsController(ProductReportRepository repository, ProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _productRepository = productRepository;
            _logger = logger;
        }
        
        
        [HttpGet("export-csv")]
        public IActionResult ExportProductsToCsv()
        {
            try
            {
                var products = _productRepository.GetAllDataAsync().Result; 
                var selectedFields = _repository.GetFieldsForReport(products);
                
                if (products.Count == 0)
                {
                    return NotFound();
                }

                // Generate CSV content
                var csvContent = new StringBuilder();
                using (var csvWriter = new CsvWriter(new StringWriter(csvContent), CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(selectedFields);
                }
                // Set response headers for CSV download
                var fileName = "products.csv";
                var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
                _logger.LogInformation($"Export all product to csv: {fileName}");
                return File(csvBytes, "text/csv", fileName, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to CSV.");
                return StatusCode(500, "An error occurred while exporting products to CSV");
            }
        }


        [HttpGet("export-pdf")]
        public IActionResult ExportProductsToPdf()
        {
            try
            {
                var products = _productRepository.GetAllDataAsync().Result; 
                var selectedFields = _repository.GetFieldsForReport(products);
                
                if (products.Count == 0)
                {
                    return NotFound();
                }

                var fileName = "products.pdf";
                _logger.LogInformation($"Export all products to PDF: {fileName}.");
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to pdf.");
                return StatusCode(500, "An error occurred while exporting products to pdf");
            }
        }

        
        [HttpGet("export-excel")]
        public IActionResult ExportProductsToExcel()
        {
            try
            {
                var products = _productRepository.GetAllDataAsync().Result; 
                var selectedFields = _repository.GetFieldsForReport(products);
                
                if (products.Count == 0)
                {
                    return NotFound();
                }

                // Create a new Excel package
                using (var excelPackage = new ExcelPackage())
                {
                    var fileName = "products.xlsx";
                    // Add a new worksheet
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Products");

                    // Set column headers
                    worksheet.Cells[1, 1].Value = "Name";
                    worksheet.Cells[1, 2].Value = "Code";
                    worksheet.Cells[1, 3].Value = "Unit Price";
                    worksheet.Cells[1, 4].Value = "Category";
                    worksheet.Cells[1, 5].Value = "Description";
                    worksheet.Cells[1, 6].Value = "Created";
                    worksheet.Cells[1, 7].Value = "Updated";

                    // Populate data rows
                    int row = 2;
                    foreach (var product in products)
                    {
                        worksheet.Cells[row, 1].Value = product.ProductName;
                        worksheet.Cells[row, 2].Value = product.Code;
                        worksheet.Cells[row, 3].Value = product.UnitPrice;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00"; // Format currency
                        worksheet.Cells[row, 4].Value = product.CategoryName;
                        worksheet.Cells[row, 5].Value = product.Description;
                        worksheet.Cells[row, 6].Value = product.CreatedAt.ToString();
                        worksheet.Cells[row, 7].Value = product.UpdatedAt.ToString();
                        row++;
                    }
                    // Auto fit columns
                    worksheet.Cells.AutoFitColumns();

                    var fileBytes = excelPackage.GetAsByteArray();
                    _logger.LogInformation($"Export all products to xlsx: {fileName}.");
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to xlsx.");
                return StatusCode(500, "An error occurred while exporting products to xlsx");
            }
        }
    
    }
}