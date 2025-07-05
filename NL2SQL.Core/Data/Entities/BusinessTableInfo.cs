using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    /// <summary>
    /// Entity model for BusinessTableInfo table containing business metadata about database tables
    /// Matches exact schema from database definition
    /// </summary>
    [Table("BusinessTableInfo", Schema = "dbo")]
    public class BusinessTableInfo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(128)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string SchemaName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? BusinessPurpose { get; set; }

        [StringLength(2000)]
        public string? BusinessContext { get; set; }

        [StringLength(500)]
        public string? PrimaryUseCase { get; set; }

        [StringLength(4000)]
        public string? CommonQueryPatterns { get; set; }

        [StringLength(2000)]
        public string? BusinessRules { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }

        [Required]
        [StringLength(1000)]
        public string DomainClassification { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string NaturalLanguageAliases { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        public string UsagePatterns { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string DataQualityIndicators { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string RelationshipSemantics { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        public decimal UsageFrequency { get; set; } = 0.0m;

        public DateTime? LastAnalyzed { get; set; }

        [Required]
        [StringLength(500)]
        public string BusinessOwner { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string DataGovernancePolicies { get; set; } = string.Empty;

        // Semantic Enhancement Fields
        [StringLength(2000)]
        public string? SemanticDescription { get; set; }

        [StringLength(1000)]
        public string? BusinessProcesses { get; set; }

        [StringLength(1000)]
        public string? AnalyticalUseCases { get; set; }

        [StringLength(500)]
        public string? ReportingCategories { get; set; }

        [StringLength(1000)]
        public string? SemanticRelationships { get; set; }

        [StringLength(500)]
        public string? QueryComplexityHints { get; set; }

        [StringLength(1000)]
        public string? BusinessGlossaryTerms { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? SemanticCoverageScore { get; set; }

        [StringLength(500)]
        public string? LLMContextHints { get; set; }

        [StringLength(1000)]
        public string? VectorSearchKeywords { get; set; }

        // Extended Business Fields (nvarchar(max) fields)
        public string? RelatedBusinessTerms { get; set; }

        public string? BusinessFriendlyName { get; set; }

        public string? NaturalLanguageDescription { get; set; }

        public string? RelationshipContext { get; set; }

        public string? DataGovernanceLevel { get; set; }

        public DateTime? LastBusinessReview { get; set; }
    }
}
