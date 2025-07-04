using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NL2SQL.Core.Models.Advanced
{
    /// <summary>
    /// Result of advanced entity extraction
    /// </summary>
    public class EntityExtractionResult
    {
        public string OriginalQuery { get; set; } = string.Empty;
        public List<GamblingEntity> GamblingEntities { get; set; } = new();
        public List<TemporalEntity> TemporalEntities { get; set; } = new();
        public List<FinancialEntity> FinancialEntities { get; set; } = new();
        public List<PlayerEntity> PlayerEntities { get; set; } = new();
        public List<GameEntity> GameEntities { get; set; } = new();
        public List<MetricEntity> MetricEntities { get; set; } = new();
        public float OverallConfidence { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    /// <summary>
    /// Base entity class
    /// </summary>
    public abstract class BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public float Confidence { get; set; }
        public string Source { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Gambling-specific entity (GGR, RTP, etc.)
    /// </summary>
    public class GamblingEntity : BaseEntity
    {
        public GamblingEntityType EntityType { get; set; }
        public string StandardTerm { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public List<string> RelatedTables { get; set; } = new();
        public List<string> RelatedColumns { get; set; } = new();
        public string CalculationFormula { get; set; } = string.Empty;
    }

    /// <summary>
    /// Temporal entity (dates, periods, etc.)
    /// </summary>
    public class TemporalEntity : BaseEntity
    {
        public TemporalType TemporalType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public string SqlExpression { get; set; } = string.Empty;
        public bool IsRelative { get; set; }
        public string Granularity { get; set; } = string.Empty; // day, week, month, year
    }

    /// <summary>
    /// Financial entity (amounts, currencies, etc.)
    /// </summary>
    public class FinancialEntity : BaseEntity
    {
        public FinancialEntityType EntityType { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public List<string> RelatedColumns { get; set; } = new();
    }

    /// <summary>
    /// Player-related entity
    /// </summary>
    public class PlayerEntity : BaseEntity
    {
        public PlayerEntityType EntityType { get; set; }
        public string PlayerSegment { get; set; } = string.Empty;
        public string PlayerType { get; set; } = string.Empty;
        public int? SpecificPlayerId { get; set; }
        public List<string> Attributes { get; set; } = new();
    }

    /// <summary>
    /// Game-related entity
    /// </summary>
    public class GameEntity : BaseEntity
    {
        public GameEntityType EntityType { get; set; }
        public string GameCategory { get; set; } = string.Empty;
        public string GameType { get; set; } = string.Empty;
        public int? SpecificGameId { get; set; }
        public string Provider { get; set; } = string.Empty;
    }

    /// <summary>
    /// Metric entity (KPIs, calculations)
    /// </summary>
    public class MetricEntity : BaseEntity
    {
        public MetricEntityType EntityType { get; set; }
        public string MetricName { get; set; } = string.Empty;
        public string AggregationType { get; set; } = string.Empty; // SUM, AVG, COUNT, etc.
        public List<string> RequiredTables { get; set; } = new();
        public List<string> RequiredColumns { get; set; } = new();
        public string CalculationLogic { get; set; } = string.Empty;
    }

    /// <summary>
    /// Semantic match result
    /// </summary>
    public class SemanticMatch<T>
    {
        public T Item { get; set; } = default!;
        public float SimilarityScore { get; set; }
        public string MatchReason { get; set; } = string.Empty;
        public List<string> MatchedTerms { get; set; } = new();
    }

    /// <summary>
    /// Table metadata for semantic search
    /// </summary>
    public class TableMetadata
    {
        public string TableName { get; set; } = string.Empty;
        public string BusinessPurpose { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new();
        public float ImportanceScore { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }

    /// <summary>
    /// Column metadata for semantic search
    /// </summary>
    public class ColumnMetadata
    {
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string BusinessMeaning { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public List<string> Synonyms { get; set; } = new();
        public List<string> Keywords { get; set; } = new();
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }

    /// <summary>
    /// Table-column recommendation
    /// </summary>
    public class TableColumnRecommendation
    {
        public string TableName { get; set; } = string.Empty;
        public List<string> RecommendedColumns { get; set; } = new();
        public float OverallScore { get; set; }
        public string Reasoning { get; set; } = string.Empty;
        public List<EntityMatch> EntityMatches { get; set; } = new();
    }

    /// <summary>
    /// Entity match to table/column
    /// </summary>
    public class EntityMatch
    {
        public BaseEntity Entity { get; set; } = null!;
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public float MatchScore { get; set; }
        public string MatchType { get; set; } = string.Empty; // exact, semantic, synonym
    }

    /// <summary>
    /// Vector embedding item
    /// </summary>
    public class EmbeddingItem
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Similarity result
    /// </summary>
    public class SimilarityResult
    {
        public EmbeddingItem Item { get; set; } = null!;
        public float SimilarityScore { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// Complete NLP processing result
    /// </summary>
    public class NLPProcessingResult
    {
        public string OriginalQuery { get; set; } = string.Empty;
        public EntityExtractionResult EntityExtraction { get; set; } = new();
        public QueryIntentAnalysis IntentAnalysis { get; set; } = new();
        public List<SemanticMatch<TableMetadata>> TableRecommendations { get; set; } = new();
        public List<SemanticMatch<ColumnMetadata>> ColumnRecommendations { get; set; } = new();
        public float OverallConfidence { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public List<string> ProcessingSteps { get; set; } = new();
    }

    /// <summary>
    /// Query intent analysis result
    /// </summary>
    public class QueryIntentAnalysis
    {
        public QueryIntent PrimaryIntent { get; set; }
        public List<QueryIntent> SecondaryIntents { get; set; } = new();
        public float Confidence { get; set; }
        public string IntentDescription { get; set; } = string.Empty;
        public List<string> RequiredActions { get; set; } = new();
    }

    /// <summary>
    /// Named entity recognition result
    /// </summary>
    public class NamedEntityResult
    {
        public List<NamedEntity> Entities { get; set; } = new();
        public float OverallConfidence { get; set; }
        public string ModelUsed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Named entity
    /// </summary>
    public class NamedEntity
    {
        public string Text { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int StartChar { get; set; }
        public int EndChar { get; set; }
        public float Confidence { get; set; }
    }

    /// <summary>
    /// Dependency parsing result
    /// </summary>
    public class DependencyParseResult
    {
        public List<DependencyRelation> Relations { get; set; } = new();
        public List<Token> Tokens { get; set; } = new();
    }

    /// <summary>
    /// Dependency relation
    /// </summary>
    public class DependencyRelation
    {
        public int HeadIndex { get; set; }
        public int DependentIndex { get; set; }
        public string RelationType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Token information
    /// </summary>
    public class Token
    {
        public string Text { get; set; } = string.Empty;
        public string Lemma { get; set; } = string.Empty;
        public string PartOfSpeech { get; set; } = string.Empty;
        public int Index { get; set; }
    }

    /// <summary>
    /// Coreference resolution result
    /// </summary>
    public class CoreferenceResult
    {
        public List<CoreferenceChain> Chains { get; set; } = new();
        public string ResolvedQuery { get; set; } = string.Empty;
    }

    /// <summary>
    /// Coreference chain
    /// </summary>
    public class CoreferenceChain
    {
        public List<Mention> Mentions { get; set; } = new();
        public string ResolvedEntity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Mention in coreference
    /// </summary>
    public class Mention
    {
        public string Text { get; set; } = string.Empty;
        public int StartChar { get; set; }
        public int EndChar { get; set; }
    }

    // Enums
    public enum GamblingEntityType
    {
        GGR, NGR, RTP, HoldPercentage, HouseEdge, Bonus, Cashback, Commission, Rake
    }

    public enum TemporalType
    {
        Date, DateRange, Period, Relative, Absolute
    }

    public enum FinancialEntityType
    {
        Amount, Currency, Deposit, Withdrawal, Bet, Win, Bonus, Fee
    }

    public enum PlayerEntityType
    {
        Individual, Segment, Type, Status, Attribute
    }

    public enum GameEntityType
    {
        Category, Type, Specific, Provider, Platform
    }

    public enum MetricEntityType
    {
        Revenue, Volume, Count, Average, Percentage, Ratio
    }

    /// <summary>
    /// Gambling term definition
    /// </summary>
    public class GamblingTermDefinition
    {
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public List<string> Synonyms { get; set; } = new();
        public List<string> RelatedTables { get; set; } = new();
        public List<string> RelatedColumns { get; set; } = new();
        public string CalculationFormula { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    /// <summary>
    /// Term to column mapping
    /// </summary>
    public class TermColumnMapping
    {
        public string Term { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public float MatchScore { get; set; }
        public string MatchType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Metric calculation definition
    /// </summary>
    public class MetricCalculation
    {
        public string MetricName { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public List<string> RequiredTables { get; set; } = new();
        public List<string> RequiredColumns { get; set; } = new();
        public string SqlTemplate { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Business rule validation result
    /// </summary>
    public class BusinessRuleValidation
    {
        public bool IsValid { get; set; }
        public List<string> Violations { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
    }

    /// <summary>
    /// Query pattern for domain-specific queries
    /// </summary>
    public class QueryPattern
    {
        public string PatternName { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string SqlTemplate { get; set; } = string.Empty;
        public List<string> RequiredEntities { get; set; } = new();
        public float Confidence { get; set; }
        public string Domain { get; set; } = string.Empty;
    }
}
