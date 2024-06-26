using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreRestApi.Models
{
    public class ProductData
    {   
        [Column("product_id")]
        public int ProductId { get; }

        [Column("unique_id")]
        public Guid UniqueId { get; }

        [Column("product_name")]
        public string? ProductName { get; }

        [Column("code")]
        public string? Code { get; }

        [Column("unit_price")]
        public double UnitPrice { get; }

        [Column("description")]
        public string? Description { get; }

        [Column("created_at")]
        public DateTime CreatedAt { get; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; }

        [Column("category_id")]
        public int CategoryId { get; }

        [Column("category_name")]
        public string? CategoryName { get; }

        [Column("supplier_id")]
        public int SupplierId { get; }

        [Column("supplier_name")]
        public string? SupplierName { get; }
    }
}