using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NL2SQL.Core.Models
{
    /// <summary>
    /// Base business table information
    /// </summary>
    public class BusinessTableInfo
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string BusinessPurpose { get; set; }
        public string BusinessContext { get; set; }
        public string PrimaryUseCase { get; set; }
        public string CommonQueryPatterns { get; set; }
        public string BusinessRules { get; set; }
        public string DomainClassification { get; set; }
        public string NaturalLanguageAliases { get; set; }
        public string UsagePatterns { get; set; }
        public string DataQualityIndicators { get; set; }
        public string RelationshipSemantics { get; set; }
        public float ImportanceScore { get; set; }
        public float UsageFrequency { get; set; }
        public string BusinessOwner { get; set; }
        public string DataGovernancePolicies { get; set; }
        public string SemanticDescription { get; set; }
        public string BusinessProcesses { get; set; }
        public string AnalyticalUseCases { get; set; }
        public string ReportingCategories { get; set; }
        public string SemanticRelationships { get; set; }
        public string QueryComplexityHints { get; set; }
        public string BusinessGlossaryTerms { get; set; }
        public float SemanticCoverageScore { get; set; }
        public string LLMContextHints { get; set; }
        public string VectorSearchKeywords { get; set; }
        public string RelatedBusinessTerms { get; set; }
        public string BusinessFriendlyName { get; set; }
        public string NaturalLanguageDescription { get; set; }
        public string RelationshipContext { get; set; }
        public List<BusinessColumnInfo> Columns { get; set; } = new List<BusinessColumnInfo>();
    }

    /// <summary>
    /// Base business column information
    /// </summary>
    public class BusinessColumnInfo
    {
        public int Id { get; set; }
        public int TableInfoId { get; set; }
        public string ColumnName { get; set; }
        public string BusinessMeaning { get; set; }
        public string BusinessContext { get; set; }
        public string DataExamples { get; set; }
        public string ValidationRules { get; set; }
        public bool IsKeyColumn { get; set; }
        public string NaturalLanguageAliases { get; set; }
        public string ValueExamples { get; set; }
        public string DataLineage { get; set; }
        public string CalculationRules { get; set; }
        public string SemanticTags { get; set; }
        public string BusinessDataType { get; set; }
        public string ConstraintsAndRules { get; set; }
        public float DataQualityScore { get; set; }
        public float UsageFrequency { get; set; }
        public string PreferredAggregation { get; set; }
        public string RelatedBusinessTerms { get; set; }
        public bool IsSensitiveData { get; set; }
        public bool IsCalculatedField { get; set; }
        public string SemanticContext { get; set; }
        public string ConceptualRelationships { get; set; }
        public string DomainSpecificTerms { get; set; }
        public string QueryIntentMapping { get; set; }
        public string BusinessQuestionTypes { get; set; }
        public string SemanticSynonyms { get; set; }
        public string AnalyticalContext { get; set; }
        public string BusinessMetrics { get; set; }
        public float SemanticRelevanceScore { get; set; }
        public string LLMPromptHints { get; set; }
        public string VectorSearchTags { get; set; }
        public string BusinessPurpose { get; set; }
        public string BusinessFriendlyName { get; set; }
        public string NaturalLanguageDescription { get; set; }
        public string BusinessRules { get; set; }
        public string RelationshipContext { get; set; }
        public float ImportanceScore { get; set; }
    }

    /// <summary>
    /// Business glossary term
    /// </summary>
    public class BusinessGlossary
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string BusinessContext { get; set; }
        public string Synonyms { get; set; }
        public string RelatedTerms { get; set; }
        public string Category { get; set; }
        public string Domain { get; set; }
        public string Examples { get; set; }
        public string MappedTables { get; set; }
        public string MappedColumns { get; set; }
        public string HierarchicalRelations { get; set; }
        public string PreferredCalculation { get; set; }
        public string DisambiguationRules { get; set; }
        public string BusinessOwner { get; set; }
        public float ConfidenceScore { get; set; }
        public float AmbiguityScore { get; set; }
        public string ContextualVariations { get; set; }
        public string QueryPatterns { get; set; }
        public string LLMPromptTemplates { get; set; }
        public string DisambiguationContext { get; set; }
        public string SemanticRelationships { get; set; }
        public string ConceptualLevel { get; set; }
        public string CrossDomainMappings { get; set; }
        public float SemanticStability { get; set; }
        public string InferenceRules { get; set; }
        public string BusinessPurpose { get; set; }
        public string RelatedBusinessTerms { get; set; }
        public string BusinessFriendlyName { get; set; }
        public string NaturalLanguageDescription { get; set; }
        public string BusinessRules { get; set; }
        public float ImportanceScore { get; set; }
        public float UsageFrequency { get; set; }
        public string RelationshipContext { get; set; }
    }

    /// <summary>
    /// Business domain definition
    /// </summary>
    public class BusinessDomain
    {
        public int Id { get; set; }
        public string DomainName { get; set; }
        public string Description { get; set; }
        public string RelatedTables { get; set; }
        public string KeyConcepts { get; set; }
        public string CommonQueries { get; set; }
        public string BusinessOwner { get; set; }
        public string RelatedDomains { get; set; }
        public float ImportanceScore { get; set; }
        public string BusinessPurpose { get; set; }
        public string RelatedBusinessTerms { get; set; }
        public string BusinessFriendlyName { get; set; }
        public string NaturalLanguageDescription { get; set; }
        public string BusinessRules { get; set; }
        public string RelationshipContext { get; set; }
        public float UsageFrequency { get; set; }
    }

    /// <summary>
    /// Table relationship information
    /// </summary>
    public class TableRelationship
    {
        public int Id { get; set; }
        public string SourceTable { get; set; }
        public string TargetTable { get; set; }
        public string RelationshipType { get; set; }
        public string JoinCondition { get; set; }
        public string BusinessMeaning { get; set; }
        public bool IsRequired { get; set; }
        public float Confidence { get; set; }
    }

    /// <summary>
    /// Database schema context
    /// </summary>
    public class DatabaseSchema
    {
        public List<TableInfo> Tables { get; set; } = new List<TableInfo>();
        public List<TableRelationship> Relationships { get; set; } = new List<TableRelationship>();
        public string SchemaName { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Table information for schema context
    /// </summary>
    public class TableInfo
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
        public string BusinessPurpose { get; set; }
        public float RelevanceScore { get; set; }
    }

    /// <summary>
    /// Column information for schema context
    /// </summary>
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public string BusinessMeaning { get; set; }
        public float RelevanceScore { get; set; }
    }

    /// <summary>
    /// Calculation rule definition
    /// </summary>
    public class CalculationRule
    {
        public string CalculationId { get; set; }
        public string Name { get; set; }
        public string Formula { get; set; }
        public List<string> RequiredInputs { get; set; } = new List<string>();
        public string OutputType { get; set; }
        public string Description { get; set; }
        public bool IsStandardMetric { get; set; }
    }

    /// <summary>
    /// Compliance check result
    /// </summary>
    public class ComplianceCheck
    {
        public bool IsCompliant { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public string ComplianceLevel { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    /// <summary>
    /// Recommended query suggestion
    /// </summary>
    public class RecommendedQuery
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NaturalLanguage { get; set; }
        public string Category { get; set; }
        public float Relevance { get; set; }
        public string SQL { get; set; }
    }

    /// <summary>
    /// Validation result
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public string Details { get; set; }
    }

    /// <summary>
    /// Security validation result
    /// </summary>
    public class SecurityResult
    {
        public bool IsSecure { get; set; }
        public List<string> SecurityIssues { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public string RiskLevel { get; set; }
    }

    /// <summary>
    /// Performance validation result
    /// </summary>
    public class PerformanceResult
    {
        public bool IsOptimal { get; set; }
        public List<string> PerformanceIssues { get; set; } = new List<string>();
        public List<string> OptimizationSuggestions { get; set; } = new List<string>();
        public TimeSpan EstimatedExecutionTime { get; set; }
    }

    /// <summary>
    /// Optimization context
    /// </summary>
    public class OptimizationContext
    {
        public string DatabaseType { get; set; }
        public int ExpectedRowCount { get; set; }
        public List<string> AvailableIndexes { get; set; } = new List<string>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Query feedback for learning
    /// </summary>
    public class QueryFeedback
    {
        public string QueryId { get; set; }
        public bool IsCorrect { get; set; }
        public string UserCorrection { get; set; }
        public int UserRating { get; set; }
        public string Comments { get; set; }
        public DateTime FeedbackTimestamp { get; set; }
        public string UserId { get; set; }
    }

    // Placeholder classes for analytics
    public class StreamingResult { }
    public class AlertRule { }
    public class DashboardConfig { }
    public class ScheduledReport { }
    public class Schedule { }
    public class QueryImprovement { }
    public class ModelPerformanceMetrics { }
}
