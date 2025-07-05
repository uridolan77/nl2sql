using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("PromptPlaceholders", Schema = "dbo")]
    public class PromptPlaceholder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PlaceholderKey { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PlaceholderName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DataSource { get; set; } = string.Empty; // TABLE, FUNCTION, STATIC, COMPUTED

        [Column(TypeName = "nvarchar(max)")]
        public string? SourceQuery { get; set; } // SQL to retrieve content

        [Column(TypeName = "nvarchar(max)")]
        public string? StaticContent { get; set; } // For static placeholders

        [Required]
        public int CacheMinutes { get; set; } = 60; // How long to cache the content

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
