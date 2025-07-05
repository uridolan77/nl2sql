using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    /// <summary>
    /// Entity model for QuerySuggestions table containing pre-defined query suggestions
    /// Matches exact schema from database definition
    /// </summary>
    [Table("QuerySuggestions", Schema = "dbo")]
    public class QuerySuggestions
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long CategoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string QueryText { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DefaultTimeFrame { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? TargetTables { get; set; }

        public byte Complexity { get; set; } = 1;

        [StringLength(200)]
        public string? RequiredPermissions { get; set; }

        [StringLength(300)]
        public string? Tags { get; set; }

        public int UsageCount { get; set; } = 0;

        public DateTime? LastUsed { get; set; }

        [Required]
        [Column(TypeName = "decimal(3,2)")]
        public decimal Relevance { get; set; } = 1.0m;

        [Required]
        [StringLength(500)]
        public string Query { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Keywords { get; set; }

        [StringLength(500)]
        public string? RequiredTables { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(3,2)")]
        public decimal Confidence { get; set; } = 1.0m;

        [Required]
        [StringLength(100)]
        public string Source { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
