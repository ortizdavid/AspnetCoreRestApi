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

        [Column("supplier_name")]
        public string? SupplierName { get; set; }

        [Column("identification_number")]
        public string? IdentificationNumber { get; set; }

        [Column("email")]
        public string? Email { get; set; }  

        [Column("primary_phone")]
        public string? PrimaryPhone { get; set; } 

        [Column("secondary_phone")]
        public string? SecondaryPhone { get; set; } 

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
