using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCoreRestApi.Helpers;

namespace AspNetCoreRestApi.Models
{   
    [Table("products")]
    public class Product
    {   
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "name must be between {2} and {1}", MinimumLength = 3)]
        [Column("product_name")]  
        public string? ProductName { get; set; }
        
        [Required]
        [StringLength(20, ErrorMessage = "code must be between {2} and {1}", MinimumLength = 3)]
        [Column("code")]  
        public string? Code { get; set; }

        [Required]
        [Range(0, 1_000_000)]
        [Column("unit_price")]  
        public double UnitPrice { get; set; }

        [StringLength(100, ErrorMessage = "description must be between {2} and {1}", MinimumLength = 3)]
        [Column("description")]  
        public string? Description { get; set; }
        
        [Required]
        [Column("category_id")]  
        public int CategoryId { get; set; }

        [Required]
        [Column("supplier_id")]  
        public int SupplierId { get; set; }
        
        [Column("unique_id")]  
        public Guid UniqueId { get; set; } = Encryption.GenerateUUID();
        
        [Column("created_at")]  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]  
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

