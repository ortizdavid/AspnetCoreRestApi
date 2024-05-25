using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCoreRestApi.Helpers;

namespace AspNetCoreRestApi.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        public string? UserName { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = true;

        [Column("image")]
        public string? Image { get; set; }

        [Column("unique_id")]
        public Guid UniqueId { get; set; } = Encryption.GenerateUUID();

        [Column("token")]
        public string? Token { get; set; } = Encryption.GenerateRandomToken(32);

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}