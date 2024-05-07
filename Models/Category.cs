using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCoreRestApi.Helpers;

namespace AspNetCoreRestApi.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
        
        [StringLength(50)]
        [Column("category_name")]        
        public string? CategoryName { get; set; }
   
        [StringLength(100)]
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