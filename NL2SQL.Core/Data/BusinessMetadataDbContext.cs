using Microsoft.EntityFrameworkCore;
using NL2SQL.Core.Data.Entities;

namespace NL2SQL.Core.Data
{
    /// <summary>
    /// Entity Framework DbContext for business metadata tables
    /// </summary>
    public class BusinessMetadataDbContext : DbContext
    {
        public BusinessMetadataDbContext(DbContextOptions<BusinessMetadataDbContext> options)
            : base(options)
        {
        }

        public DbSet<BusinessDomain> BusinessDomain { get; set; }
    public DbSet<BusinessTableInfo> BusinessTableInfo { get; set; }
        public DbSet<BusinessColumnInfo> BusinessColumnInfo { get; set; }
    public DbSet<BusinessGlossary> BusinessGlossary { get; set; }
    public DbSet<QuerySuggestions> QuerySuggestions { get; set; }
        public DbSet<PromptTemplate> PromptTemplates { get; set; }
        public DbSet<BusinessRule> BusinessRules { get; set; }
        public DbSet<KPIDefinition> KPIDefinitions { get; set; }
        public DbSet<ComplianceRule> ComplianceRules { get; set; }
        public DbSet<ExampleQuery> ExampleQueries { get; set; }
        public DbSet<PromptPlaceholder> PromptPlaceholders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BusinessTableInfo
            modelBuilder.Entity<BusinessTableInfo>(entity =>
            {
                entity.ToTable("BusinessTableInfo", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TableName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(5,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Index for performance
                entity.HasIndex(e => e.TableName).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ImportanceScore);
            });

            // Configure BusinessColumnInfo
            modelBuilder.Entity<BusinessColumnInfo>(entity =>
            {
                entity.ToTable("BusinessColumnInfo", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ColumnName).IsRequired().HasMaxLength(128);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(18,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsKeyColumn).HasDefaultValue(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for performance
                entity.HasIndex(e => new { e.TableInfoId, e.ColumnName }).IsUnique();
                entity.HasIndex(e => e.TableInfoId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ImportanceScore);

                // Foreign key relationship to BusinessTableInfo
                entity.HasOne(e => e.BusinessTable)
                      .WithMany()
                      .HasForeignKey(e => e.TableInfoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BusinessDomain
            modelBuilder.Entity<BusinessDomain>(entity =>
            {
                entity.ToTable("BusinessDomain", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DomainName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(5,4)").HasDefaultValue(1.0m);
                entity.Property(e => e.UsageFrequency).HasColumnType("decimal(18,2)").HasDefaultValue(0.0m);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for performance
                entity.HasIndex(e => e.DomainName).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ImportanceScore);
            });

            // Configure BusinessGlossary
            modelBuilder.Entity<BusinessGlossary>(entity =>
            {
                entity.ToTable("BusinessGlossary", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Term).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Definition).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(18,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.UsageFrequency).HasColumnType("decimal(18,2)").HasDefaultValue(0.0m);
                entity.Property(e => e.ConfidenceScore).HasColumnType("decimal(5,4)").HasDefaultValue(1.0m);
                entity.Property(e => e.AmbiguityScore).HasColumnType("decimal(5,4)").HasDefaultValue(0.0m);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for performance
                entity.HasIndex(e => e.Term).IsUnique();
                entity.HasIndex(e => e.Domain);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ImportanceScore);
            });

            // Configure QuerySuggestions
            modelBuilder.Entity<QuerySuggestions>(entity =>
            {
                entity.ToTable("QuerySuggestions", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QueryText).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Relevance).HasColumnType("decimal(3,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.Confidence).HasColumnType("decimal(3,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for performance
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Relevance);
            });

            // Configure PromptTemplates
            modelBuilder.Entity<PromptTemplate>(entity =>
            {
                entity.ToTable("PromptTemplates", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Version).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Content).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.SuccessRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UsageCount).HasDefaultValue(0);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(18,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.UsageFrequency).HasColumnType("decimal(18,2)").HasDefaultValue(0.0m);
                entity.Property(e => e.Priority).HasDefaultValue(1);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.TemplateKey).HasMaxLength(100);
                entity.Property(e => e.IntentType).HasMaxLength(50);
                entity.Property(e => e.Tags).HasMaxLength(500);

                // Indexes for performance
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.TemplateKey);
                entity.HasIndex(e => e.IntentType);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.ImportanceScore);
                entity.HasIndex(e => new { e.Name, e.Version }).IsUnique();
            });

            // Configure BusinessRules
            modelBuilder.Entity<BusinessRule>(entity =>
            {
                entity.ToTable("BusinessRules", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RuleKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RuleName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RuleContent).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.RuleCategory).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IntentType).HasMaxLength(50);
                entity.Property(e => e.Priority).HasDefaultValue(1);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Condition).HasMaxLength(1000);
                entity.Property(e => e.Action).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.UpdatedBy).HasMaxLength(256);

                // Indexes for performance
                entity.HasIndex(e => new { e.RuleCategory, e.IntentType }).HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.RuleKey).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Priority);
            });

            // Configure KPIDefinitions
            modelBuilder.Entity<KPIDefinition>(entity =>
            {
                entity.ToTable("KPIDefinitions", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KPIKey).IsRequired().HasMaxLength(50);
                entity.Property(e => e.KPIName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Definition).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.CalculationFormula).HasColumnType("nvarchar(max)");
                entity.Property(e => e.BusinessContext).HasMaxLength(1000);
                entity.Property(e => e.Industry).IsRequired().HasMaxLength(50).HasDefaultValue("Gaming");
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ImportanceScore).HasColumnType("decimal(18,2)").HasDefaultValue(1.0m);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.UpdatedBy).HasMaxLength(256);

                // Indexes for performance
                entity.HasIndex(e => e.Category).HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.KPIKey).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ImportanceScore);
            });

            // Configure ComplianceRules
            modelBuilder.Entity<ComplianceRule>(entity =>
            {
                entity.ToTable("ComplianceRules", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RuleKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RuleName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RuleContent).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.ComplianceType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Jurisdiction).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.UpdatedBy).HasMaxLength(256);

                // Indexes for performance
                entity.HasIndex(e => e.ComplianceType).HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.RuleKey).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.EffectiveDate);
            });

            // Configure ExampleQueries
            modelBuilder.Entity<ExampleQuery>(entity =>
            {
                entity.ToTable("ExampleQueries", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExampleKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NaturalLanguageQuery).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.SQLQuery).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.IntentType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Complexity).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SchemaElements).HasColumnType("nvarchar(max)");
                entity.Property(e => e.BusinessConcepts).HasColumnType("nvarchar(max)");
                entity.Property(e => e.IsValidated).HasDefaultValue(false);
                entity.Property(e => e.SuccessRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.UsageCount).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.UpdatedBy).HasMaxLength(256);

                // Indexes for performance
                entity.HasIndex(e => e.IntentType).HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.ExampleKey).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Complexity);
                entity.HasIndex(e => e.IsValidated);
            });

            // Configure PromptPlaceholders
            modelBuilder.Entity<PromptPlaceholder>(entity =>
            {
                entity.ToTable("PromptPlaceholders", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlaceholderKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PlaceholderName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DataSource).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SourceQuery).HasColumnType("nvarchar(max)");
                entity.Property(e => e.StaticContent).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CacheMinutes).HasDefaultValue(60);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(256);
                entity.Property(e => e.UpdatedBy).HasMaxLength(256);

                // Indexes for performance
                entity.HasIndex(e => e.PlaceholderKey).IsUnique().HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DataSource);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be overridden by dependency injection configuration
                optionsBuilder.UseSqlServer();
            }
        }
    }
}
