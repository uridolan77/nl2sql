using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Semantic search service for finding relevant tables and columns using vector embeddings
    /// </summary>
    public class SemanticSearchService : ISemanticSearchService
    {
        private readonly ILogger<SemanticSearchService> _logger;
        private readonly IVectorEmbeddingService _embeddingService;
        private readonly IMemoryCache _cache;
        
        private List<TableMetadata> _tables = new();
        private List<ColumnMetadata> _columns = new();
        private bool _isInitialized = false;

        public SemanticSearchService(
            ILogger<SemanticSearchService> logger,
            IVectorEmbeddingService embeddingService,
            IMemoryCache cache)
        {
            _logger = logger;
            _embeddingService = embeddingService;
            _cache = cache;
        }

        public async Task InitializeAsync(List<TableMetadata> tables, List<ColumnMetadata> columns)
        {
            _logger.LogInformation("Initializing semantic search with {TableCount} tables and {ColumnCount} columns", 
                tables.Count, columns.Count);

            _tables = tables;
            _columns = columns;

            // Generate embeddings for all tables and columns
            await GenerateTableEmbeddingsAsync();
            await GenerateColumnEmbeddingsAsync();

            _isInitialized = true;
            _logger.LogInformation("Semantic search initialization completed");
        }

        public async Task<List<SemanticMatch<TableMetadata>>> FindSimilarTablesAsync(string query, int topK = 5)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Semantic search service not initialized");

            _logger.LogDebug("Finding similar tables for query: {Query}", query);

            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            var matches = new List<SemanticMatch<TableMetadata>>();

            foreach (var table in _tables)
            {
                var similarity = _embeddingService.CalculateCosineSimilarity(queryEmbedding, table.Embedding);
                
                var match = new SemanticMatch<TableMetadata>
                {
                    Item = table,
                    SimilarityScore = similarity,
                    MatchReason = DetermineTableMatchReason(query, table),
                    MatchedTerms = FindMatchedTerms(query, table.Keywords)
                };

                matches.Add(match);
            }

            var result = matches
                .OrderByDescending(m => m.SimilarityScore)
                .Take(topK)
                .ToList();

            _logger.LogDebug("Found {Count} similar tables with scores: {Scores}", 
                result.Count, string.Join(", ", result.Select(r => r.SimilarityScore.ToString("F3"))));

            return result;
        }

        public async Task<List<SemanticMatch<ColumnMetadata>>> FindSimilarColumnsAsync(string query, int topK = 10)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Semantic search service not initialized");

            _logger.LogDebug("Finding similar columns for query: {Query}", query);

            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
            var matches = new List<SemanticMatch<ColumnMetadata>>();

            foreach (var column in _columns)
            {
                var similarity = _embeddingService.CalculateCosineSimilarity(queryEmbedding, column.Embedding);
                
                var match = new SemanticMatch<ColumnMetadata>
                {
                    Item = column,
                    SimilarityScore = similarity,
                    MatchReason = DetermineColumnMatchReason(query, column),
                    MatchedTerms = FindMatchedTerms(query, column.Keywords.Concat(column.Synonyms).ToList())
                };

                matches.Add(match);
            }

            var result = matches
                .OrderByDescending(m => m.SimilarityScore)
                .Take(topK)
                .ToList();

            _logger.LogDebug("Found {Count} similar columns with scores: {Scores}", 
                result.Count, string.Join(", ", result.Select(r => r.SimilarityScore.ToString("F3"))));

            return result;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            return await _embeddingService.GenerateEmbeddingAsync(text);
        }

        public async Task<float> CalculateSimilarityAsync(string text1, string text2)
        {
            var embedding1 = await _embeddingService.GenerateEmbeddingAsync(text1);
            var embedding2 = await _embeddingService.GenerateEmbeddingAsync(text2);
            return _embeddingService.CalculateCosineSimilarity(embedding1, embedding2);
        }

        public async Task<List<TableColumnRecommendation>> RecommendTableColumnsAsync(
            string query, 
            EntityExtractionResult entities)
        {
            _logger.LogInformation("Generating table-column recommendations for query: {Query}", query);

            var recommendations = new List<TableColumnRecommendation>();

            // Get similar tables first
            var similarTables = await FindSimilarTablesAsync(query, 10);

            foreach (var tableMatch in similarTables)
            {
                var table = tableMatch.Item;
                var recommendation = new TableColumnRecommendation
                {
                    TableName = table.TableName,
                    OverallScore = tableMatch.SimilarityScore,
                    Reasoning = $"Table matched with similarity {tableMatch.SimilarityScore:F3}: {tableMatch.MatchReason}"
                };

                // Find relevant columns for this table
                var tableColumns = _columns.Where(c => c.TableName == table.TableName).ToList();
                var columnMatches = new List<SemanticMatch<ColumnMetadata>>();

                foreach (var column in tableColumns)
                {
                    var similarity = _embeddingService.CalculateCosineSimilarity(
                        await _embeddingService.GenerateEmbeddingAsync(query), 
                        column.Embedding);

                    columnMatches.Add(new SemanticMatch<ColumnMetadata>
                    {
                        Item = column,
                        SimilarityScore = similarity,
                        MatchReason = DetermineColumnMatchReason(query, column)
                    });
                }

                // Select top columns for this table
                var topColumns = columnMatches
                    .OrderByDescending(c => c.SimilarityScore)
                    .Take(5)
                    .Where(c => c.SimilarityScore > 0.3f) // Minimum threshold
                    .ToList();

                recommendation.RecommendedColumns = topColumns.Select(c => c.Item.ColumnName).ToList();

                // Create entity matches
                recommendation.EntityMatches = CreateEntityMatches(entities, table, topColumns);

                // Adjust overall score based on entity matches
                if (recommendation.EntityMatches.Any())
                {
                    var entityBonus = recommendation.EntityMatches.Average(em => em.MatchScore) * 0.2f;
                    recommendation.OverallScore += entityBonus;
                }

                if (recommendation.RecommendedColumns.Any())
                {
                    recommendations.Add(recommendation);
                }
            }

            var result = recommendations
                .OrderByDescending(r => r.OverallScore)
                .Take(5)
                .ToList();

            _logger.LogInformation("Generated {Count} table-column recommendations", result.Count);
            return result;
        }

        private async Task GenerateTableEmbeddingsAsync()
        {
            _logger.LogDebug("Generating embeddings for {Count} tables", _tables.Count);

            foreach (var table in _tables)
            {
                if (table.Embedding.Length == 0)
                {
                    // Combine table name, business purpose, and keywords for embedding
                    var combinedText = $"{table.TableName} {table.BusinessPurpose} {string.Join(" ", table.Keywords)}";
                    table.Embedding = await _embeddingService.GenerateEmbeddingAsync(combinedText);
                }
            }
        }

        private async Task GenerateColumnEmbeddingsAsync()
        {
            _logger.LogDebug("Generating embeddings for {Count} columns", _columns.Count);

            foreach (var column in _columns)
            {
                if (column.Embedding.Length == 0)
                {
                    // Combine column name, business meaning, and synonyms for embedding
                    var combinedText = $"{column.ColumnName} {column.BusinessMeaning} {string.Join(" ", column.Synonyms)} {string.Join(" ", column.Keywords)}";
                    column.Embedding = await _embeddingService.GenerateEmbeddingAsync(combinedText);
                }
            }
        }

        private string DetermineTableMatchReason(string query, TableMetadata table)
        {
            var queryLower = query.ToLowerInvariant();
            var reasons = new List<string>();

            if (queryLower.Contains(table.TableName.ToLowerInvariant()))
                reasons.Add("exact table name match");

            if (table.BusinessPurpose != null && 
                table.BusinessPurpose.ToLowerInvariant().Split(' ').Any(word => queryLower.Contains(word) && word.Length > 3))
                reasons.Add("business purpose alignment");

            if (table.Keywords.Any(keyword => queryLower.Contains(keyword.ToLowerInvariant())))
                reasons.Add("keyword match");

            if (table.Domain != null && queryLower.Contains(table.Domain.ToLowerInvariant()))
                reasons.Add("domain match");

            return reasons.Any() ? string.Join(", ", reasons) : "semantic similarity";
        }

        private string DetermineColumnMatchReason(string query, ColumnMetadata column)
        {
            var queryLower = query.ToLowerInvariant();
            var reasons = new List<string>();

            if (queryLower.Contains(column.ColumnName.ToLowerInvariant()))
                reasons.Add("exact column name match");

            if (column.BusinessMeaning != null && 
                column.BusinessMeaning.ToLowerInvariant().Split(' ').Any(word => queryLower.Contains(word) && word.Length > 3))
                reasons.Add("business meaning alignment");

            if (column.Synonyms.Any(synonym => queryLower.Contains(synonym.ToLowerInvariant())))
                reasons.Add("synonym match");

            if (column.Keywords.Any(keyword => queryLower.Contains(keyword.ToLowerInvariant())))
                reasons.Add("keyword match");

            return reasons.Any() ? string.Join(", ", reasons) : "semantic similarity";
        }

        private List<string> FindMatchedTerms(string query, List<string> terms)
        {
            var queryLower = query.ToLowerInvariant();
            return terms.Where(term => queryLower.Contains(term.ToLowerInvariant())).ToList();
        }

        private List<EntityMatch> CreateEntityMatches(
            EntityExtractionResult entities, 
            TableMetadata table, 
            List<SemanticMatch<ColumnMetadata>> columnMatches)
        {
            var matches = new List<EntityMatch>();

            // Match gambling entities
            foreach (var entity in entities.GamblingEntities)
            {
                var bestColumn = columnMatches
                    .Where(c => entity.RelatedColumns.Contains(c.Item.ColumnName) || 
                               c.Item.BusinessMeaning?.ToLowerInvariant().Contains(entity.Text.ToLowerInvariant()) == true)
                    .OrderByDescending(c => c.SimilarityScore)
                    .FirstOrDefault();

                if (bestColumn != null)
                {
                    matches.Add(new EntityMatch
                    {
                        Entity = entity,
                        TableName = table.TableName,
                        ColumnName = bestColumn.Item.ColumnName,
                        MatchScore = bestColumn.SimilarityScore,
                        MatchType = entity.RelatedColumns.Contains(bestColumn.Item.ColumnName) ? "exact" : "semantic"
                    });
                }
            }

            // Match other entity types similarly...
            // (Implementation for other entity types would follow the same pattern)

            return matches;
        }
    }
}
