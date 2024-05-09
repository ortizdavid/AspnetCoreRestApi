using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreRestApi.Models
{
    [Table("images")]
    public class Image
    {
        [Key]
        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("product_id")]  
        public int ProductId { get; set; }

        [StringLength(50)]
        [Column("front_image")]  
        public string? FrontImage { get; set; }

        [StringLength(50)]
        [Column("back_image")]  
        public string? BackImage { get; set; }

        [StringLength(50)]
        [Column("left_image")]  
        public string? LeftImage { get; set; }

        [StringLength(50)]
        [Column("right_image")]  
        public string? RightImage { get; set; }

        [StringLength(50)]
        [Column("upload_dir")]  
        public string? UploadDir { get; set; }

        [Column("created_at")]  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]  
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}