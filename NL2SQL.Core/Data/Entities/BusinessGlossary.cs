using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    /// <summary>
    /// Entity model for BusinessGlossary table containing business terminology and definitions
    /// Matches exact schema from database definition
    /// </summary>
    [Table("BusinessGlossary", Schema = "dbo")]
    public class BusinessGlossary
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Term { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Definition { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? BusinessContext { get; set; }

        [StringLength(1000)]
        public string? Synonyms { get; set; }

        [StringLength(1000)]
        public string? RelatedTerms { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public int UsageCount { get; set; } = 0;

        public DateTime? LastUsed { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }

        [Required]
        [StringLength(200)]
        public string Domain { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Examples { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string MappedTables { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string MappedColumns { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string HierarchicalRelations { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string PreferredCalculation { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string DisambiguationRules { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string BusinessOwner { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string RegulationReferences { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        public decimal ConfidenceScore { get; set; } = 1.0m;

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        public decimal AmbiguityScore { get; set; } = 0.0m;

        [Required]
        [StringLength(1000)]
        public string ContextualVariations { get; set; } = string.Empty;

        public DateTime? LastValidated { get; set; }

        [StringLength(2000)]
        public string? SemanticEmbedding { get; set; }

        [StringLength(1000)]
        public string? QueryPatterns { get; set; }

        [StringLength(1000)]
        public string? LLMPromptTemplates { get; set; }

        [StringLength(500)]
        public string? DisambiguationContext { get; set; }

        [StringLength(1000)]
        public string? SemanticRelationships { get; set; }

        [StringLength(500)]
        public string? ConceptualLevel { get; set; }

        [StringLength(1000)]
        public string? CrossDomainMappings { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? SemanticStability { get; set; }

        [StringLength(1000)]
        public string? InferenceRules { get; set; }

        // Extended Business Fields (nvarchar(max) fields)
        public string? BusinessPurpose { get; set; }

        public string? RelatedBusinessTerms { get; set; }

        public string? BusinessFriendlyName { get; set; }

        public string? NaturalLanguageDescription { get; set; }

        public string? BusinessRules { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UsageFrequency { get; set; } = 0.0m;

        public string? RelationshipContext { get; set; }

        public string? DataGovernanceLevel { get; set; }

        public DateTime? LastBusinessReview { get; set; }
    }
}
