using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("PromptTemplates", Schema = "dbo")]
    public class PromptTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SuccessRate { get; set; }

        [Required]
        public int UsageCount { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string? Parameters { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? UpdatedBy { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? BusinessPurpose { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? RelatedBusinessTerms { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? BusinessFriendlyName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? NaturalLanguageDescription { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? BusinessRules { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? RelationshipContext { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? DataGovernanceLevel { get; set; }

        public DateTime? LastBusinessReview { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UsageFrequency { get; set; } = 0.0m;

        [StringLength(100)]
        public string? TemplateKey { get; set; }

        [StringLength(50)]
        public string? IntentType { get; set; }

        [Required]
        public int Priority { get; set; } = 1;

        [StringLength(500)]
        public string? Tags { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? BusinessMetadata { get; set; }

        public DateTime? LastBusinessReviewDate { get; set; }

        public DateTime? LastUsedDate { get; set; }
    }
}
