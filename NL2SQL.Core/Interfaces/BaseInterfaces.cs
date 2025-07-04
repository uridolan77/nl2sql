using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NL2SQL.Core.Models;

namespace NL2SQL.Core.Interfaces
{
    /// <summary>
    /// Base metadata repository interface
    /// </summary>
    public interface IMetadataRepository
    {
        Task<List<BusinessTableInfo>> GetTableInfoAsync();
        Task<BusinessTableInfo> GetTableInfoAsync(string tableName);
        Task<List<BusinessColumnInfo>> GetBusinessColumnInfoAsync(int tableId);
        Task<List<TableRelationship>> GetTableRelationshipsAsync();
    }

    /// <summary>
    /// Natural language analyzer interface
    /// </summary>
    public interface INaturalLanguageAnalyzer
    {
        Task<QueryAnalysis> AnalyzeQueryAsync(string query);
        Task<List<string>> ExtractEntitiesAsync(string query);
        Task<QueryIntent> DetectIntentAsync(string query);
        Task<List<string>> ExtractKeywordsAsync(string query);
    }

    /// <summary>
    /// Schema context builder interface
    /// </summary>
    public interface ISchemaContextBuilder
    {
        Task<DatabaseSchema> BuildSchemaContextAsync(QueryAnalysis analysis);
        Task<List<string>> GetRelevantTablesAsync(QueryAnalysis analysis);
        Task<List<string>> GetRelevantColumnsAsync(string tableName, QueryAnalysis analysis);
    }

    /// <summary>
    /// SQL prompt builder interface
    /// </summary>
    public interface ISqlPromptBuilder
    {
        Task<string> BuildPromptAsync(QueryAnalysis analysis, DatabaseSchema schema);
        Task<string> AddBusinessContextAsync(string prompt, List<BusinessGlossary> glossary);
        Task<string> AddExamplesAsync(string prompt, List<string> examples);
    }

    /// <summary>
    /// LLM service interface
    /// </summary>
    public interface ILLMService
    {
        Task<string> GenerateSqlAsync(string prompt);
        Task<bool> ValidateConnectionAsync();
        Task<string> ExplainSqlAsync(string sql);
    }

    /// <summary>
    /// Main NL2SQL service interface
    /// </summary>
    public interface INL2SqlService
    {
        Task<SqlGenerationResult> GenerateSqlAsync(string naturalLanguageQuery);
        Task<SqlGenerationResult> GenerateSqlAsync(string naturalLanguageQuery, string userId);
        Task<bool> ValidateQueryAsync(string naturalLanguageQuery);
    }
}

namespace NL2SQL.Core.Models
{
    /// <summary>
    /// Query analysis result
    /// </summary>
    public class QueryAnalysis
    {
        public string OriginalQuery { get; set; }
        public QueryIntent Intent { get; set; }
        public List<string> Entities { get; set; } = new List<string>();
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Tables { get; set; } = new List<string>();
        public List<string> Columns { get; set; } = new List<string>();
        public QueryComplexity Complexity { get; set; }
        public float Confidence { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// SQL generation result
    /// </summary>
    public class SqlGenerationResult
    {
        public bool Success { get; set; }
        public string GeneratedSql { get; set; }
        public string Explanation { get; set; }
        public float Confidence { get; set; }
        public string ErrorMessage { get; set; }
        public QueryAnalysis Analysis { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Query intent enumeration
    /// </summary>
    public enum QueryIntent
    {
        Select,
        Aggregate,
        Trend,
        Comparison,
        TopN,
        Distribution,
        Correlation,
        Forecast,
        Anomaly,
        Drill
    }

    /// <summary>
    /// Query complexity enumeration
    /// </summary>
    public enum QueryComplexity
    {
        Simple,
        Medium,
        Complex,
        VeryComplex
    }
}
