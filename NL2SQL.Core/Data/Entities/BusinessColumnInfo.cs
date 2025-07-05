using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    /// <summary>
    /// Entity model for BusinessColumnInfo table containing business metadata about database columns
    /// Matches exact schema from database definition
    /// </summary>
    [Table("BusinessColumnInfo", Schema = "dbo")]
    public class BusinessColumnInfo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long TableInfoId { get; set; }

        [Required]
        [StringLength(128)]
        public string ColumnName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? BusinessMeaning { get; set; }

        [StringLength(1000)]
        public string? BusinessContext { get; set; }

        [StringLength(2000)]
        public string? DataExamples { get; set; }

        [StringLength(1000)]
        public string? ValidationRules { get; set; }

        public bool IsKeyColumn { get; set; } = false;

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
        public string NaturalLanguageAliases { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string ValueExamples { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string DataLineage { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string CalculationRules { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string SemanticTags { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string BusinessDataType { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string ConstraintsAndRules { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        [StringLength(255)]
        public string? PreferredAggregation { get; set; }

        [StringLength(1000)]
        public string? RelatedBusinessTerms { get; set; }



        [StringLength(1000)]
        public string? SemanticContext { get; set; }

        [StringLength(1000)]
        public string? ConceptualRelationships { get; set; }

        [StringLength(1000)]
        public string? DomainSpecificTerms { get; set; }

        [StringLength(1000)]
        public string? QueryIntentMapping { get; set; }

        [StringLength(1000)]
        public string? BusinessQuestionTypes { get; set; }

        [StringLength(1000)]
        public string? SemanticSynonyms { get; set; }

        [StringLength(1000)]
        public string? AnalyticalContext { get; set; }

        [StringLength(500)]
        public string? BusinessMetrics { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? SemanticRelevanceScore { get; set; }

        [StringLength(1000)]
        public string? LLMPromptHints { get; set; }

        [StringLength(500)]
        public string? VectorSearchTags { get; set; }

        // Extended Business Fields (nvarchar(max) fields)
        public string? BusinessPurpose { get; set; }

        public string? BusinessFriendlyName { get; set; }

        public string? NaturalLanguageDescription { get; set; }

        public string? BusinessRules { get; set; }

        public string? RelationshipContext { get; set; }

        public string? DataGovernanceLevel { get; set; }

        public DateTime? LastBusinessReview { get; set; }

        // Navigation property
        public virtual BusinessTableInfo? BusinessTable { get; set; }
    }
}
