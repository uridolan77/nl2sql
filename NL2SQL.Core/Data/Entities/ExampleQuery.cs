using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NL2SQL.Core.Data.Entities
{
    [Table("ExampleQueries", Schema = "dbo")]
    public class ExampleQuery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ExampleKey { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string NaturalLanguageQuery { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string SQLQuery { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string IntentType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Complexity { get; set; } = string.Empty; // SIMPLE, MEDIUM, COMPLEX

        [Column(TypeName = "nvarchar(max)")]
        public string? SchemaElements { get; set; } // JSON array of tables/columns used

        [Column(TypeName = "nvarchar(max)")]
        public string? BusinessConcepts { get; set; } // JSON array of business concepts

        [Required]
        public bool IsValidated { get; set; } = false;

        public DateTime? ValidationDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SuccessRate { get; set; }

        [Required]
        public int UsageCount { get; set; } = 0;

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
