using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NL2SQL.Core.Models;
using NL2SQL.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace NL2SQL.Core.Services
{
    /// <summary>
    /// Simple natural language analyzer implementation
    /// </summary>
    public class SimpleNaturalLanguageAnalyzer : INaturalLanguageAnalyzer
    {
        private readonly ILogger<SimpleNaturalLanguageAnalyzer> _logger;

        public SimpleNaturalLanguageAnalyzer(ILogger<SimpleNaturalLanguageAnalyzer> logger)
        {
            _logger = logger;
        }

        public async Task<QueryAnalysis> AnalyzeQueryAsync(string query)
        {
            var analysis = new QueryAnalysis
            {
                OriginalQuery = query,
                Intent = await DetectIntentAsync(query),
                Entities = await ExtractEntitiesAsync(query),
                Keywords = await ExtractKeywordsAsync(query),
                Complexity = DetermineComplexity(query),
                Confidence = 0.8f
            };

            return analysis;
        }

        public async Task<List<string>> ExtractEntitiesAsync(string query)
        {
            var entities = new List<string>();
            var queryLower = query.ToLowerInvariant();

            // Simple entity extraction
            if (queryLower.Contains("player") || queryLower.Contains("customer"))
                entities.Add("Player");
            if (queryLower.Contains("game") || queryLower.Contains("slot"))
                entities.Add("Game");
            if (queryLower.Contains("deposit") || queryLower.Contains("payment"))
                entities.Add("Deposit");
            if (queryLower.Contains("withdrawal") || queryLower.Contains("cashout"))
                entities.Add("Withdrawal");

            return entities;
        }

        public async Task<QueryIntent> DetectIntentAsync(string query)
        {
            var queryLower = query.ToLowerInvariant();

            if (queryLower.Contains("top") || queryLower.Contains("best"))
                return QueryIntent.TopN;
            if (queryLower.Contains("total") || queryLower.Contains("sum"))
                return QueryIntent.Aggregate;
            if (queryLower.Contains("trend") || queryLower.Contains("over time"))
                return QueryIntent.Trend;
            if (queryLower.Contains("compare") || queryLower.Contains("vs"))
                return QueryIntent.Comparison;

            return QueryIntent.Select;
        }

        public async Task<List<string>> ExtractKeywordsAsync(string query)
        {
            // Simple keyword extraction
            var stopWords = new HashSet<string> { "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by" };
            
            return query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                       .Where(word => !stopWords.Contains(word.ToLowerInvariant()))
                       .Select(word => word.Trim('.', ',', '?', '!'))
                       .Where(word => !string.IsNullOrEmpty(word))
                       .ToList();
        }

        private QueryComplexity DetermineComplexity(string query)
        {
            var wordCount = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            
            return wordCount switch
            {
                <= 5 => QueryComplexity.Simple,
                <= 10 => QueryComplexity.Medium,
                <= 20 => QueryComplexity.Complex,
                _ => QueryComplexity.VeryComplex
            };
        }
    }

    /// <summary>
    /// Schema context builder implementation
    /// </summary>
    public class SchemaContextBuilder : ISchemaContextBuilder
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly ILogger<SchemaContextBuilder> _logger;

        public SchemaContextBuilder(IMetadataRepository metadataRepository, ILogger<SchemaContextBuilder> logger)
        {
            _metadataRepository = metadataRepository;
            _logger = logger;
        }

        public async Task<DatabaseSchema> BuildSchemaContextAsync(QueryAnalysis analysis)
        {
            var schema = new DatabaseSchema();
            var relevantTables = await GetRelevantTablesAsync(analysis);

            foreach (var tableName in relevantTables)
            {
                var tableInfo = await _metadataRepository.GetTableInfoAsync(tableName);
                if (tableInfo != null)
                {
                    var table = new TableInfo
                    {
                        TableName = tableInfo.TableName,
                        SchemaName = tableInfo.SchemaName,
                        BusinessPurpose = tableInfo.BusinessPurpose
                    };

                    var columns = await GetRelevantColumnsAsync(tableName, analysis);
                    foreach (var columnName in columns)
                    {
                        var columnInfo = tableInfo.Columns.FirstOrDefault(c => c.ColumnName == columnName);
                        if (columnInfo != null)
                        {
                            table.Columns.Add(new ColumnInfo
                            {
                                ColumnName = columnInfo.ColumnName,
                                BusinessMeaning = columnInfo.BusinessMeaning,
                                DataType = columnInfo.BusinessDataType
                            });
                        }
                    }

                    schema.Tables.Add(table);
                }
            }

            return schema;
        }

        public async Task<List<string>> GetRelevantTablesAsync(QueryAnalysis analysis)
        {
            var tables = new List<string>();

            // Simple table mapping based on entities
            foreach (var entity in analysis.Entities)
            {
                switch (entity.ToLowerInvariant())
                {
                    case "player":
                        tables.AddRange(new[] { "tbl_Daily_actions_players", "tbl_Daily_actions" });
                        break;
                    case "game":
                        tables.AddRange(new[] { "Games", "tbl_Daily_actions_games" });
                        break;
                    case "deposit":
                        tables.AddRange(new[] { "tbl_Daily_actions", "tbl_Daily_actionsGBP_transactions" });
                        break;
                    case "withdrawal":
                        tables.AddRange(new[] { "tbl_Withdrawal_requests", "tbl_Daily_actions" });
                        break;
                }
            }

            // Default tables if no specific entities found
            if (!tables.Any())
            {
                tables.Add("tbl_Daily_actions");
            }

            return tables.Distinct().ToList();
        }

        public async Task<List<string>> GetRelevantColumnsAsync(string tableName, QueryAnalysis analysis)
        {
            var columns = new List<string>();

            // Add common columns based on intent
            switch (analysis.Intent)
            {
                case QueryIntent.Aggregate:
                    columns.AddRange(new[] { "Date", "PlayerID", "Deposits", "Withdrawals", "BetsCasino", "WinsCasino" });
                    break;
                case QueryIntent.TopN:
                    columns.AddRange(new[] { "PlayerID", "Deposits", "TotalBets", "TotalWins" });
                    break;
                case QueryIntent.Trend:
                    columns.AddRange(new[] { "Date", "Deposits", "BetsCasino", "WinsCasino" });
                    break;
                default:
                    columns.AddRange(new[] { "Date", "PlayerID" });
                    break;
            }

            return columns.Distinct().ToList();
        }
    }

    /// <summary>
    /// SQL prompt builder implementation
    /// </summary>
    public class SqlPromptBuilder : ISqlPromptBuilder
    {
        private readonly ILogger<SqlPromptBuilder> _logger;

        public SqlPromptBuilder(ILogger<SqlPromptBuilder> logger)
        {
            _logger = logger;
        }

        public async Task<string> BuildPromptAsync(QueryAnalysis analysis, DatabaseSchema schema)
        {
            var prompt = $"Generate SQL for: {analysis.OriginalQuery}\n\n";
            prompt += "Available tables and columns:\n";

            foreach (var table in schema.Tables)
            {
                prompt += $"Table: {table.TableName}\n";
                prompt += $"Purpose: {table.BusinessPurpose}\n";
                prompt += "Columns:\n";
                
                foreach (var column in table.Columns)
                {
                    prompt += $"  - {column.ColumnName}: {column.BusinessMeaning}\n";
                }
                prompt += "\n";
            }

            prompt += "Please generate a SQL query that answers the question.";
            return prompt;
        }

        public async Task<string> AddBusinessContextAsync(string prompt, List<BusinessGlossary> glossary)
        {
            if (glossary?.Any() == true)
            {
                prompt += "\n\nBusiness Context:\n";
                foreach (var term in glossary.Take(5))
                {
                    prompt += $"- {term.Term}: {term.Definition}\n";
                }
            }
            return prompt;
        }

        public async Task<string> AddExamplesAsync(string prompt, List<string> examples)
        {
            if (examples?.Any() == true)
            {
                prompt += "\n\nExample queries:\n";
                foreach (var example in examples.Take(3))
                {
                    prompt += $"- {example}\n";
                }
            }
            return prompt;
        }
    }

    /// <summary>
    /// Mock LLM service for testing
    /// </summary>
    public class OpenAILLMService : ILLMService
    {
        private readonly string _apiKey;
        private readonly ILogger<OpenAILLMService> _logger;

        public OpenAILLMService(string apiKey, ILogger<OpenAILLMService> logger)
        {
            _apiKey = apiKey;
            _logger = logger;
        }

        public async Task<string> GenerateSqlAsync(string prompt)
        {
            // Mock implementation - in production this would call OpenAI API
            _logger.LogInformation("Generating SQL with prompt: {Prompt}", prompt.Substring(0, Math.Min(100, prompt.Length)));
            
            // Return a simple mock SQL query
            return "SELECT * FROM tbl_Daily_actions WHERE Date >= DATEADD(day, -30, GETDATE())";
        }

        public async Task<bool> ValidateConnectionAsync()
        {
            // Mock validation
            return !string.IsNullOrEmpty(_apiKey);
        }

        public async Task<string> ExplainSqlAsync(string sql)
        {
            return $"This query retrieves data from the database: {sql}";
        }
    }

    /// <summary>
    /// Main NL2SQL service implementation
    /// </summary>
    public class NL2SqlService : INL2SqlService
    {
        private readonly INaturalLanguageAnalyzer _analyzer;
        private readonly ISchemaContextBuilder _schemaBuilder;
        private readonly ISqlPromptBuilder _promptBuilder;
        private readonly ILLMService _llmService;
        private readonly ILogger<NL2SqlService> _logger;

        public NL2SqlService(
            INaturalLanguageAnalyzer analyzer,
            ISchemaContextBuilder schemaBuilder,
            ISqlPromptBuilder promptBuilder,
            ILLMService llmService,
            ILogger<NL2SqlService> logger)
        {
            _analyzer = analyzer;
            _schemaBuilder = schemaBuilder;
            _promptBuilder = promptBuilder;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<SqlGenerationResult> GenerateSqlAsync(string naturalLanguageQuery)
        {
            return await GenerateSqlAsync(naturalLanguageQuery, "anonymous");
        }

        public async Task<SqlGenerationResult> GenerateSqlAsync(string naturalLanguageQuery, string userId)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                // Analyze the query
                var analysis = await _analyzer.AnalyzeQueryAsync(naturalLanguageQuery);

                // Build schema context
                var schema = await _schemaBuilder.BuildSchemaContextAsync(analysis);

                // Build prompt
                var prompt = await _promptBuilder.BuildPromptAsync(analysis, schema);

                // Generate SQL
                var sql = await _llmService.GenerateSqlAsync(prompt);

                var processingTime = DateTime.UtcNow - startTime;

                return new SqlGenerationResult
                {
                    Success = true,
                    GeneratedSql = sql,
                    Explanation = "SQL generated successfully",
                    Confidence = analysis.Confidence,
                    Analysis = analysis,
                    ProcessingTime = processingTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SQL for query: {Query}", naturalLanguageQuery);
                
                return new SqlGenerationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ProcessingTime = DateTime.UtcNow - startTime
                };
            }
        }

        public async Task<bool> ValidateQueryAsync(string naturalLanguageQuery)
        {
            try
            {
                var analysis = await _analyzer.AnalyzeQueryAsync(naturalLanguageQuery);
                return analysis.Confidence > 0.5f;
            }
            catch
            {
                return false;
            }
        }
    }
}
