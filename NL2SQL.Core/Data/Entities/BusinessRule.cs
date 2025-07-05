using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("BusinessRules", Schema = "dbo")]
    public class BusinessRule
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
        public string RuleCategory { get; set; } = string.Empty; // FINANCIAL, DATE_HANDLING, FRAUD_PREVENTION

        [StringLength(50)]
        public string? IntentType { get; set; } // QUERY_GENERATION, ANALYTICAL

        [Required]
        public int Priority { get; set; } = 1;

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(1000)]
        public string? Condition { get; set; } // When this rule applies

        [Column(TypeName = "nvarchar(max)")]
        public string? Action { get; set; } // What the rule enforces

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
