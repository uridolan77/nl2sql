using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("KPIDefinitions", Schema = "dbo")]
    public class KPIDefinition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KPIKey { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string KPIName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Definition { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? CalculationFormula { get; set; }

        [StringLength(1000)]
        public string? BusinessContext { get; set; }

        [Required]
        [StringLength(50)]
        public string Industry { get; set; } = "Gaming";

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // REVENUE, PLAYER_BEHAVIOR, OPERATIONAL

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportanceScore { get; set; } = 1.0m;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
