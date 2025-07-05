using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    /// <summary>
    /// Entity model for BusinessDomain table containing domain classification and business context
    /// Matches exact schema from database definition
    /// </summary>
    [Table("BusinessDomain", Schema = "dbo")]
    public class BusinessDomain
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string DomainName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string RelatedTables { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string KeyConcepts { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string CommonQueries { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string BusinessOwner { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string RelatedDomains { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Extended Business Fields (nvarchar(max) fields)
        public string? BusinessPurpose { get; set; }

        public string? RelatedBusinessTerms { get; set; }

        public string? BusinessFriendlyName { get; set; }

        public string? NaturalLanguageDescription { get; set; }

        public string? BusinessRules { get; set; }

        public string? RelationshipContext { get; set; }

        public string? DataGovernanceLevel { get; set; }

        public DateTime? LastBusinessReview { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UsageFrequency { get; set; } = 0.0m;
    }
}
