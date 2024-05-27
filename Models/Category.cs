using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCoreRestApi.Helpers;

namespace AspNetCoreRestApi.Models
{
    /* category_name VARCHAR(50) UNIQUE,
    description VARCHAR(150),*/
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "name must be between {2} and {1}", MinimumLength = 3)]
        [Column("category_name")]        
        public string? CategoryName { get; set; }
   
        [Required]
        [StringLength(150, ErrorMessage = "description must be between {2} and {1}", MinimumLength = 10)]
        [Column("description")]
        public string? Description { get; set; }
        
        [Column("unique_id")]
        public Guid UniqueId { get; set; } = Encryption.GenerateUUID();
    
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}