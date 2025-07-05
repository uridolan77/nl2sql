using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("ComplianceRules", Schema = "dbo")]
    public class ComplianceRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RuleKey { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string RuleName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string RuleContent { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ComplianceType { get; set; } = string.Empty; // GDPR, SOX, GAMING_REGULATION

        [StringLength(100)]
        public string? Jurisdiction { get; set; } // EU, US, UK

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
