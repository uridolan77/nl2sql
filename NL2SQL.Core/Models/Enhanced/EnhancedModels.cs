using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using NL2SQL.Core.Models;

namespace NL2SQL.Core.Models.Enhanced
{
    /// <summary>
    /// Enhanced business table information with semantic capabilities
    /// </summary>
    public class EnhancedBusinessTableInfo : BusinessTableInfo
    {
        public List<SemanticTag> SemanticTags { get; set; } = new List<SemanticTag>();
        public Dictionary<string, float> ConceptRelevanceScores { get; set; } = new Dictionary<string, float>();
        public List<QueryPattern> CommonQueryPatterns { get; set; } = new List<QueryPattern>();
        public BusinessDomainContext DomainContext { get; set; }
        public List<RelationshipMapping> SemanticRelationships { get; set; } = new List<RelationshipMapping>();
        public VectorEmbedding TableEmbedding { get; set; }
        public DateTime LastSemanticUpdate { get; set; }
        public float SemanticRelevanceScore { get; set; }
    }

    /// <summary>
    /// Enhanced column information with semantic understanding
    /// </summary>
    public class EnhancedBusinessColumnInfo : BusinessColumnInfo
    {
        public VectorEmbedding SemanticEmbedding { get; set; }
        public List<BusinessConcept> MappedConcepts { get; set; } = new List<BusinessConcept>();
        public CalculationEngine CalculationRules { get; set; }
        public List<ValidationRule> BusinessValidations { get; set; } = new List<ValidationRule>();
        public SensitivityClassification DataSensitivity { get; set; }
        public List<string> NaturalLanguageSynonyms { get; set; } = new List<string>();
        public Dictionary<string, object> SemanticProperties { get; set; } = new Dictionary<string, object>();
        public float ColumnRelevanceScore { get; set; }
    }

    /// <summary>
    /// Query context for processing natural language queries
    /// </summary>
    public class QueryContext
    {
        [Required]
        public string UserId { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public BusinessDomain PrimaryDomain { get; set; }
        public DateTime QueryTimestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> SessionContext { get; set; } = new Dictionary<string, object>();
        public List<string> AccessibleTables { get; set; } = new List<string>();
        public ComplianceRequirements ComplianceContext { get; set; }
        public string QueryId { get; set; } = Guid.NewGuid().ToString();
        public QueryComplexity EstimatedComplexity { get; set; }
        public List<string> PreferredLanguages { get; set; } = new List<string> { "en" };
    }

    /// <summary>
    /// Semantic tag for categorizing business entities
    /// </summary>
    public class SemanticTag
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public float Confidence { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Query pattern for common business questions
    /// </summary>
    public class QueryPattern
    {
        public string PatternId { get; set; }
        public string NaturalLanguagePattern { get; set; }
        public string SQLTemplate { get; set; }
        public List<string> RequiredTables { get; set; } = new List<string>();
        public List<string> RequiredColumns { get; set; } = new List<string>();
        public QueryIntent Intent { get; set; }
        public float UsageFrequency { get; set; }
        public List<string> Examples { get; set; } = new List<string>();
        public Dictionary<string, string> ParameterMappings { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Business domain context for query processing
    /// </summary>
    public class BusinessDomainContext
    {
        public string DomainName { get; set; }
        public List<string> KeyMetrics { get; set; } = new List<string>();
        public List<string> CommonDimensions { get; set; } = new List<string>();
        public Dictionary<string, string> DomainTerminology { get; set; } = new Dictionary<string, string>();
        public List<BusinessRule> DomainRules { get; set; } = new List<BusinessRule>();
        public List<CalculationTemplate> StandardCalculations { get; set; } = new List<CalculationTemplate>();
        public RegulatoryContext RegulatoryRequirements { get; set; }
    }

    /// <summary>
    /// Relationship mapping between business entities
    /// </summary>
    public class RelationshipMapping
    {
        public string SourceEntity { get; set; }
        public string TargetEntity { get; set; }
        public RelationshipType Type { get; set; }
        public string BusinessMeaning { get; set; }
        public float Strength { get; set; }
        public List<string> JoinConditions { get; set; } = new List<string>();
        public bool IsRequired { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Vector embedding for semantic similarity
    /// </summary>
    public class VectorEmbedding
    {
        public string Id { get; set; }
        public float[] Vector { get; set; }
        public int Dimensions { get; set; }
        public string Model { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        public float CosineSimilarity(VectorEmbedding other)
        {
            if (other?.Vector == null || Vector == null || Vector.Length != other.Vector.Length)
                return 0f;

            float dotProduct = 0f;
            float normA = 0f;
            float normB = 0f;

            for (int i = 0; i < Vector.Length; i++)
            {
                dotProduct += Vector[i] * other.Vector[i];
                normA += Vector[i] * Vector[i];
                normB += other.Vector[i] * other.Vector[i];
            }

            return dotProduct / (float)(Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }

    /// <summary>
    /// Business concept mapping
    /// </summary>
    public class BusinessConcept
    {
        public string ConceptId { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Domain { get; set; }
        public List<string> Synonyms { get; set; } = new List<string>();
        public List<string> RelatedConcepts { get; set; } = new List<string>();
        public ConceptType Type { get; set; }
        public float Confidence { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Calculation engine for business metrics
    /// </summary>
    public class CalculationEngine
    {
        public string CalculationId { get; set; }
        public string Name { get; set; }
        public string Formula { get; set; }
        public List<string> RequiredInputs { get; set; } = new List<string>();
        public string OutputType { get; set; }
        public List<ValidationRule> ValidationRules { get; set; } = new List<ValidationRule>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string Description { get; set; }
        public bool IsStandardMetric { get; set; }
    }

    /// <summary>
    /// Validation rule for business data
    /// </summary>
    public class ValidationRule
    {
        public string RuleId { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        public string ErrorMessage { get; set; }
        public ValidationSeverity Severity { get; set; }
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Data sensitivity classification
    /// </summary>
    public class SensitivityClassification
    {
        public SensitivityLevel Level { get; set; }
        public List<string> DataCategories { get; set; } = new List<string>();
        public List<string> AccessRestrictions { get; set; } = new List<string>();
        public bool RequiresAuditLogging { get; set; }
        public bool RequiresDataMasking { get; set; }
        public string ComplianceFramework { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Compliance requirements for queries
    /// </summary>
    public class ComplianceRequirements
    {
        public List<string> RequiredCompliances { get; set; } = new List<string>();
        public bool RequiresAuditTrail { get; set; }
        public bool RequiresDataMasking { get; set; }
        public List<string> RestrictedColumns { get; set; } = new List<string>();
        public List<string> RestrictedTables { get; set; } = new List<string>();
        public TimeSpan DataRetentionPeriod { get; set; }
        public Dictionary<string, object> AdditionalRequirements { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Business rule definition
    /// </summary>
    public class BusinessRule
    {
        public string RuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Expression { get; set; }
        public BusinessRuleType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public float Priority { get; set; }
        public List<string> ApplicableTables { get; set; } = new List<string>();
        public List<string> ApplicableColumns { get; set; } = new List<string>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Calculation template for standard business metrics
    /// </summary>
    public class CalculationTemplate
    {
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SQLTemplate { get; set; }
        public List<string> RequiredTables { get; set; } = new List<string>();
        public List<string> RequiredColumns { get; set; } = new List<string>();
        public Dictionary<string, string> ParameterMappings { get; set; } = new Dictionary<string, string>();
        public List<string> BusinessTerms { get; set; } = new List<string>();
        public string Domain { get; set; }
        public bool IsStandard { get; set; }
    }

    /// <summary>
    /// Regulatory context for compliance
    /// </summary>
    public class RegulatoryContext
    {
        public List<string> ApplicableRegulations { get; set; } = new List<string>();
        public List<string> RequiredDisclosures { get; set; } = new List<string>();
        public List<string> RestrictedOperations { get; set; } = new List<string>();
        public Dictionary<string, object> ComplianceParameters { get; set; } = new Dictionary<string, object>();
        public bool RequiresRegulatorApproval { get; set; }
        public TimeSpan ReportingFrequency { get; set; }
    }

    // Use enums from base models

    public enum RelationshipType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany,
        Hierarchical,
        Temporal,
        Semantic
    }

    public enum ConceptType
    {
        Metric,
        Dimension,
        Measure,
        Attribute,
        Entity,
        Event,
        Process
    }

    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public enum SensitivityLevel
    {
        Public,
        Internal,
        Confidential,
        Restricted,
        TopSecret
    }

    public enum BusinessRuleType
    {
        Validation,
        Calculation,
        Constraint,
        Transformation,
        Compliance,
        Security
    }


}
