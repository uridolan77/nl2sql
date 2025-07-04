using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Core.Interfaces.Advanced
{
    /// <summary>
    /// Advanced entity extraction interface using NLP libraries and domain knowledge
    /// </summary>
    public interface IAdvancedEntityExtractor
    {
        /// <summary>
        /// Extract entities from natural language query using advanced NLP techniques
        /// </summary>
        Task<EntityExtractionResult> ExtractEntitiesAsync(string query);

        /// <summary>
        /// Extract gambling-specific entities (GGR, RTP, player types, etc.)
        /// </summary>
        Task<List<GamblingEntity>> ExtractGamblingEntitiesAsync(string query);

        /// <summary>
        /// Extract temporal expressions (dates, time ranges, periods)
        /// </summary>
        Task<List<TemporalEntity>> ExtractTemporalEntitiesAsync(string query);

        /// <summary>
        /// Extract financial entities (amounts, currencies, transaction types)
        /// </summary>
        Task<List<FinancialEntity>> ExtractFinancialEntitiesAsync(string query);

        /// <summary>
        /// Extract player-related entities (player types, segments, IDs)
        /// </summary>
        Task<List<PlayerEntity>> ExtractPlayerEntitiesAsync(string query);

        /// <summary>
        /// Extract game-related entities (game types, names, categories)
        /// </summary>
        Task<List<GameEntity>> ExtractGameEntitiesAsync(string query);

        /// <summary>
        /// Extract metric references (KPIs, calculations, aggregations)
        /// </summary>
        Task<List<MetricEntity>> ExtractMetricEntitiesAsync(string query);
    }

    /// <summary>
    /// Semantic search interface for table and column matching
    /// </summary>
    public interface ISemanticSearchService
    {
        /// <summary>
        /// Initialize the semantic search with database metadata
        /// </summary>
        Task InitializeAsync(List<TableMetadata> tables, List<ColumnMetadata> columns);

        /// <summary>
        /// Find semantically similar tables based on query intent
        /// </summary>
        Task<List<SemanticMatch<TableMetadata>>> FindSimilarTablesAsync(string query, int topK = 5);

        /// <summary>
        /// Find semantically similar columns based on query entities
        /// </summary>
        Task<List<SemanticMatch<ColumnMetadata>>> FindSimilarColumnsAsync(string query, int topK = 10);

        /// <summary>
        /// Generate vector embedding for text
        /// </summary>
        Task<float[]> GenerateEmbeddingAsync(string text);

        /// <summary>
        /// Calculate semantic similarity between two texts
        /// </summary>
        Task<float> CalculateSimilarityAsync(string text1, string text2);

        /// <summary>
        /// Find best table-column combinations for a query
        /// </summary>
        Task<List<TableColumnRecommendation>> RecommendTableColumnsAsync(string query, EntityExtractionResult entities);
    }

    /// <summary>
    /// Vector embedding service interface
    /// </summary>
    public interface IVectorEmbeddingService
    {
        /// <summary>
        /// Generate embeddings for text using sentence transformers
        /// </summary>
        Task<float[]> GenerateEmbeddingAsync(string text);

        /// <summary>
        /// Generate embeddings for multiple texts in batch
        /// </summary>
        Task<List<float[]>> GenerateEmbeddingsBatchAsync(List<string> texts);

        /// <summary>
        /// Calculate cosine similarity between two embeddings
        /// </summary>
        float CalculateCosineSimilarity(float[] embedding1, float[] embedding2);

        /// <summary>
        /// Find most similar embeddings from a collection
        /// </summary>
        Task<List<SimilarityResult>> FindMostSimilarAsync(float[] queryEmbedding, List<EmbeddingItem> candidates, int topK = 5);
    }

    /// <summary>
    /// Advanced NLP pipeline interface
    /// </summary>
    public interface IAdvancedNLPPipeline
    {
        /// <summary>
        /// Process query through complete NLP pipeline
        /// </summary>
        Task<NLPProcessingResult> ProcessQueryAsync(string query);

        /// <summary>
        /// Analyze query intent with high accuracy
        /// </summary>
        Task<QueryIntentAnalysis> AnalyzeIntentAsync(string query);

        /// <summary>
        /// Extract and resolve named entities
        /// </summary>
        Task<NamedEntityResult> ExtractNamedEntitiesAsync(string query);

        /// <summary>
        /// Perform dependency parsing for complex queries
        /// </summary>
        Task<DependencyParseResult> ParseDependenciesAsync(string query);

        /// <summary>
        /// Resolve coreferences and ambiguities
        /// </summary>
        Task<CoreferenceResult> ResolveCoreferencesAsync(string query);
    }

    /// <summary>
    /// Gambling domain knowledge service
    /// </summary>
    public interface IGamblingDomainKnowledge
    {
        /// <summary>
        /// Get gambling-specific term definitions and mappings
        /// </summary>
        Task<Dictionary<string, GamblingTermDefinition>> GetGamblingTermsAsync();

        /// <summary>
        /// Map natural language terms to database columns
        /// </summary>
        Task<List<TermColumnMapping>> MapTermsToColumnsAsync(List<string> terms);

        /// <summary>
        /// Get calculation formulas for gambling metrics
        /// </summary>
        Task<Dictionary<string, MetricCalculation>> GetMetricCalculationsAsync();

        /// <summary>
        /// Validate business rules for queries
        /// </summary>
        Task<BusinessRuleValidation> ValidateBusinessRulesAsync(string query, EntityExtractionResult entities);

        /// <summary>
        /// Get domain-specific query patterns
        /// </summary>
        Task<List<QueryPattern>> GetQueryPatternsAsync();
    }
}
