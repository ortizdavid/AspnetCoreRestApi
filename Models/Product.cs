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

        [StringLength(50)]
        [Column("product_name")]  
        public string? ProductName { get; set; }
        
        [StringLength(20)]
        [Column("code")]  
        public string? Code { get; set; }

        [Range(0, 1_000_000)]
        [Column("unit_price")]  
        public double UnitPrice { get; set; }

        [StringLength(100)]
        [Column("description")]  
        public string? Description { get; set; }
        
        [Column("category_id")]  
        public int CategoryId { get; set; }
        
        [Column("unique_id")]  
        public Guid UniqueId { get; set; } = Encryption.GenerateUUID();
        
        [Column("created_at")]  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]  
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

