using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Enhanced;
using NL2SQL.Core.Interfaces;

namespace NL2SQL.Core.Interfaces.Enhanced
{
    /// <summary>
    /// Advanced query analyzer interface
    /// </summary>
    public interface IAdvancedQueryAnalyzer
    {
        Task<EnhancedQueryAnalysis> AnalyzeAsync(string query, QueryContext context);
        Task<List<EntityMention>> ExtractEntitiesAsync(string query);
        Task<Models.QueryIntent> DetectIntentAsync(string query);
        Task<TemporalContext> ExtractTemporalContextAsync(string query);
        Task<List<BusinessConcept>> MapToBusinessConceptsAsync(string query);
    }

    /// <summary>
    /// Gambling domain service interface
    /// </summary>
    public interface IGamblingDomainService
    {
        Task<List<GamblingMetric>> GetAvailableMetricsAsync();
        Task<GamblingMetric> GetMetricDefinitionAsync(string metricName);
        Task<CalculationRule> GetMetricCalculationAsync(string metricName);
        Task<List<BusinessRule>> GetApplicableRulesAsync(QueryContext context);
        Task<ComplianceCheck> ValidateComplianceAsync(string sql);
        Task<List<RecommendedQuery>> GetRecommendedQueriesAsync(QueryContext context);
        Task<BusinessConcept> GetBusinessConceptAsync(string conceptId);
        Task<List<string>> GetAmbiguousTermsAsync(string query);
        Task<List<string>> GetTablesForEntityAsync(string entityType);
        Task<List<string>> GetTablesForConceptAsync(string conceptId);
        Task<List<JoinRequirement>> GetOptimalJoinPathsAsync(List<string> tables);
    }

    /// <summary>
    /// Semantic analysis service interface
    /// </summary>
    public interface ISemanticAnalysisService
    {
        Task<List<EntityMention>> ExtractEntitiesAsync(string query);
        Task<Models.QueryIntent> DetectIntentAsync(string query);
        Task<List<BusinessConcept>> MapToBusinessConceptsAsync(string query);
        Task<float> CalculateSemanticSimilarityAsync(string text1, string text2);
        Task<VectorEmbedding> GenerateEmbeddingAsync(string text);
        Task<List<string>> FindSimilarTermsAsync(string term, float threshold = 0.8f);
    }

    /// <summary>
    /// Temporal analysis service interface
    /// </summary>
    public interface ITemporalAnalysisService
    {
        Task<TemporalContext> ExtractTemporalContextAsync(string query, Dictionary<string, TemporalExpression> patterns);
        Task<DateTimeRange> ResolveDateRangeAsync(string temporalExpression);
        Task<string> ConvertToSQLDateExpressionAsync(TemporalExpression expression);
        Task<TemporalGranularity> DetectGranularityAsync(string query);
    }

    /// <summary>
    /// Intelligent schema builder interface
    /// </summary>
    public interface IIntelligentSchemaBuilder
    {
        Task<DatabaseSchema> BuildContextAsync(EnhancedQueryAnalysis analysis);
        Task<List<TableRelevanceScore>> ScoreTableRelevanceAsync(EnhancedQueryAnalysis analysis);
        Task<List<ColumnRelevanceScore>> ScoreColumnRelevanceAsync(EnhancedQueryAnalysis analysis);
        Task<List<JoinPath>> FindOptimalJoinPathsAsync(List<string> requiredTables);
        Task<BusinessContext> EnrichWithBusinessContextAsync(DatabaseSchema schema);
    }

    /// <summary>
    /// Dynamic prompt builder interface
    /// </summary>
    public interface IDynamicPromptBuilder
    {
        Task<string> BuildPromptAsync(EnhancedQueryAnalysis analysis, DatabaseSchema schema);
        Task<PromptTemplate> SelectOptimalTemplateAsync(Models.QueryIntent intent);
        Task<string> EnhanceWithExamplesAsync(string basePrompt, EnhancedQueryAnalysis analysis);
        Task<string> AddBusinessContextAsync(string prompt, BusinessContext context);
        Task<ValidationResult> ValidatePromptQualityAsync(string prompt);
    }

    /// <summary>
    /// LLM provider manager interface
    /// </summary>
    public interface ILLMProviderManager
    {
        Task<LLMResponse> GenerateAsync(string prompt, LLMConfiguration config);
        Task<LLMProvider> SelectOptimalProviderAsync(Models.QueryComplexity complexity);
        Task<LLMResponse> ExecuteWithFallbackAsync(string prompt, List<LLMProvider> providers);
        Task<QualityMetrics> EvaluateResponseQualityAsync(LLMResponse response);
    }

    /// <summary>
    /// Intelligent cache interface
    /// </summary>
    public interface IIntelligentCache
    {
        Task<CacheResult<T>> GetAsync<T>(string key, CacheContext context);
        Task SetAsync<T>(string key, T value, CachePolicy policy);
        Task<List<string>> FindSimilarQueriesAsync(string query, float threshold);
        Task InvalidateByPatternAsync(string pattern);
        Task<CacheStatistics> GetStatisticsAsync();
    }

    /// <summary>
    /// SQL validator interface
    /// </summary>
    public interface ISQLValidator
    {
        Task<ValidationResult> ValidateSyntaxAsync(string sql);
        Task<SecurityResult> ValidateSecurityAsync(string sql);
        Task<PerformanceResult> ValidatePerformanceAsync(string sql);
        Task<string> OptimizeSQLAsync(string sql, OptimizationContext context);
        Task<string> SanitizeSQLAsync(string sql);
    }

    /// <summary>
    /// Real-time analytics interface
    /// </summary>
    public interface IRealtimeAnalytics
    {
        Task<StreamingResult> CreateStreamingQueryAsync(string nlQuery);
        Task<List<AlertRule>> GetActiveAlertsAsync();
        Task<DashboardConfig> GenerateDashboardAsync(List<string> queries);
        Task<ScheduledReport> ScheduleReportAsync(string nlQuery, Schedule schedule);
    }

    /// <summary>
    /// Adaptive learning system interface
    /// </summary>
    public interface IAdaptiveLearningSystem
    {
        Task LearnFromQueryAsync(string naturalLanguage, string generatedSQL, QueryFeedback feedback);
        Task<List<QueryImprovement>> GetImprovementSuggestionsAsync();
        Task<ModelPerformanceMetrics> GetPerformanceMetricsAsync();
        Task RetrainModelsAsync();
        Task<List<QueryPattern>> DiscoverNewPatternsAsync();
    }

    /// <summary>
    /// Enhanced metadata repository interface
    /// </summary>
    public interface IEnhancedMetadataRepository : IMetadataRepository
    {
        Task<List<EnhancedBusinessTableInfo>> GetEnhancedTableInfoAsync();
        Task<List<EnhancedBusinessColumnInfo>> GetEnhancedColumnInfoAsync(int tableId);
        Task<List<BusinessGlossary>> GetBusinessGlossaryAsync();
        Task<List<BusinessDomain>> GetBusinessDomainsAsync();
        Task<VectorEmbedding> GetTableEmbeddingAsync(string tableName);
        Task<VectorEmbedding> GetColumnEmbeddingAsync(string tableName, string columnName);
        Task<List<QueryPattern>> GetQueryPatternsAsync(string domain = null);
        Task<List<CalculationTemplate>> GetCalculationTemplatesAsync(string domain = null);
    }

}

namespace NL2SQL.Core.Models.Enhanced
{
    /// <summary>
    /// Table relevance score for schema selection
    /// </summary>
    public class TableRelevanceScore
    {
        public string TableName { get; set; }
        public float RelevanceScore { get; set; }
        public List<string> MatchingConcepts { get; set; } = new List<string>();
        public List<string> MatchingEntities { get; set; } = new List<string>();
        public string Reasoning { get; set; }
        public Dictionary<string, float> FeatureScores { get; set; } = new Dictionary<string, float>();
    }

    /// <summary>
    /// Column relevance score for schema selection
    /// </summary>
    public class ColumnRelevanceScore
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public float RelevanceScore { get; set; }
        public List<string> MatchingTerms { get; set; } = new List<string>();
        public string BusinessPurpose { get; set; }
        public bool IsRequired { get; set; }
        public Dictionary<string, float> FeatureScores { get; set; } = new Dictionary<string, float>();
    }

    /// <summary>
    /// Join path for multi-table queries
    /// </summary>
    public class JoinPath
    {
        public List<string> Tables { get; set; } = new List<string>();
        public List<JoinRequirement> Joins { get; set; } = new List<JoinRequirement>();
        public float PathScore { get; set; }
        public int PathLength { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Business context for prompt enhancement
    /// </summary>
    public class BusinessContext
    {
        public string Domain { get; set; }
        public List<BusinessRule> ApplicableRules { get; set; } = new List<BusinessRule>();
        public List<GamblingMetric> RelevantMetrics { get; set; } = new List<GamblingMetric>();
        public Dictionary<string, string> DomainTerminology { get; set; } = new Dictionary<string, string>();
        public List<string> ExampleQueries { get; set; } = new List<string>();
        public ComplianceRequirements ComplianceContext { get; set; }
    }

    /// <summary>
    /// Prompt template for different query types
    /// </summary>
    public class PromptTemplate
    {
        public string TemplateId { get; set; }
        public QueryIntent Intent { get; set; }
        public string Template { get; set; }
        public List<string> RequiredSections { get; set; } = new List<string>();
        public Dictionary<string, string> Placeholders { get; set; } = new Dictionary<string, string>();
        public float EffectivenessScore { get; set; }
    }

    /// <summary>
    /// LLM response with metadata
    /// </summary>
    public class LLMResponse
    {
        public string GeneratedSQL { get; set; }
        public string Explanation { get; set; }
        public float Confidence { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public string Provider { get; set; }
        public string Model { get; set; }
        public int TokensUsed { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// LLM provider configuration
    /// </summary>
    public class LLMProvider
    {
        public string ProviderId { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string ApiEndpoint { get; set; }
        public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
        public bool IsAvailable { get; set; }
        public float Priority { get; set; }
        public List<QueryComplexity> SupportedComplexities { get; set; } = new List<QueryComplexity>();
    }

    /// <summary>
    /// LLM configuration for requests
    /// </summary>
    public class LLMConfiguration
    {
        public float Temperature { get; set; } = 0.1f;
        public int MaxTokens { get; set; } = 2000;
        public float TopP { get; set; } = 0.9f;
        public int TopK { get; set; } = 50;
        public List<string> StopSequences { get; set; } = new List<string>();
        public Dictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Quality metrics for LLM responses
    /// </summary>
    public class QualityMetrics
    {
        public float SyntaxScore { get; set; }
        public float SemanticScore { get; set; }
        public float CompletenessScore { get; set; }
        public float PerformanceScore { get; set; }
        public float SecurityScore { get; set; }
        public float OverallScore { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
        public List<string> Suggestions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Cache result with metadata
    /// </summary>
    public class CacheResult<T>
    {
        public bool Hit { get; set; }
        public T Value { get; set; }
        public DateTime? CachedAt { get; set; }
        public TimeSpan? Age { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Cache context for intelligent caching
    /// </summary>
    public class CacheContext
    {
        public string UserId { get; set; }
        public string Domain { get; set; }
        public QueryComplexity Complexity { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Cache policy configuration
    /// </summary>
    public class CachePolicy
    {
        public TimeSpan TTL { get; set; }
        public bool SlidingExpiration { get; set; }
        public int Priority { get; set; }
        public List<string> InvalidationTriggers { get; set; } = new List<string>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Cache statistics
    /// </summary>
    public class CacheStatistics
    {
        public long TotalRequests { get; set; }
        public long CacheHits { get; set; }
        public long CacheMisses { get; set; }
        public float HitRate { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public long TotalSize { get; set; }
        public int EntryCount { get; set; }
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new Dictionary<string, object>();
    }
}
