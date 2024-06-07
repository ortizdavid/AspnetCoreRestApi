using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCoreRestApi.Helpers;

namespace AspNetCoreRestApi.Models
{
    [Table("suppliers")]
    public class Supplier
    {
        [Key]
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "suppliername must be between {2} and {1}", MinimumLength = 3)]
        [Column("supplier_name")]
        public string? SupplierName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "identification must be between {2} and {1}", MinimumLength = 5)]
        [Column("identification_number")]
        public string? IdentificationNumber { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "email must be between {2} and {1}", MinimumLength = 10)]
        [Column("email")]
        public string? Email { get; set; }  

        [Required]
        [StringLength(20, ErrorMessage = "primary phone must be between {2} and {1}", MinimumLength = 8)]
        [Column("primary_phone")]
        public string? PrimaryPhone { get; set; } 

        [StringLength(20, ErrorMessage = "secondary phone must be between {2} and {1}", MinimumLength = 8)]
        [Column("secondary_phone")]
        public string? SecondaryPhone { get; set; } 

        [StringLength(150, ErrorMessage = "address must be between {2} and {1}", MinimumLength = 5)]
        [Column("address")]
        public string? Address { get; set; }

        [Column("unique_id")]
        public Guid UniqueId { get; set; } = Encryption.GenerateUUID();

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
    }
}