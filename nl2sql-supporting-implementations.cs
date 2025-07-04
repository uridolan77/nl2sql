// Services/SimpleNaturalLanguageAnalyzer.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public class SimpleNaturalLanguageAnalyzer : INaturalLanguageAnalyzer
    {
        private readonly HashSet<string> _stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
            "of", "with", "by", "from", "up", "about", "into", "through", "during",
            "before", "after", "above", "below", "between", "under", "over",
            "is", "are", "was", "were", "been", "be", "have", "has", "had",
            "do", "does", "did", "will", "would", "shall", "should", "may", "might",
            "must", "can", "could", "me", "show", "find", "get", "give", "tell"
        };

        private readonly Dictionary<string, string> _timeKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "today", "CURRENT_DATE" },
            { "yesterday", "DATEADD(day, -1, CURRENT_DATE)" },
            { "tomorrow", "DATEADD(day, 1, CURRENT_DATE)" },
            { "last month", "DATEADD(month, -1, CURRENT_DATE)" },
            { "this month", "MONTH(CURRENT_DATE)" },
            { "last year", "YEAR(DATEADD(year, -1, CURRENT_DATE))" },
            { "this year", "YEAR(CURRENT_DATE)" },
            { "last week", "DATEADD(week, -1, CURRENT_DATE)" },
            { "this week", "DATEADD(week, 0, CURRENT_DATE)" }
        };

        private readonly Dictionary<string, string> _aggregationKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "total", "SUM" },
            { "average", "AVG" },
            { "avg", "AVG" },
            { "count", "COUNT" },
            { "number of", "COUNT" },
            { "maximum", "MAX" },
            { "max", "MAX" },
            { "minimum", "MIN" },
            { "min", "MIN" },
            { "sum", "SUM" }
        };

        public Task<QueryAnalysis> AnalyzeQueryAsync(string query)
        {
            var analysis = new QueryAnalysis
            {
                NormalizedQuery = NormalizeQuery(query),
                KeyTerms = ExtractKeyTerms(query),
                Entities = ExtractEntities(query),
                IntentType = DetermineIntent(query),
                TimeReferences = ExtractTimeReferences(query),
                NumericReferences = ExtractNumericReferences(query)
            };

            return Task.FromResult(analysis);
        }

        private string NormalizeQuery(string query)
        {
            // Convert to lowercase and remove extra whitespace
            var normalized = Regex.Replace(query.ToLower(), @"\s+", " ").Trim();
            
            // Remove punctuation except for important SQL-related characters
            normalized = Regex.Replace(normalized, @"[^\w\s\-\.\'\,]", " ");
            
            return normalized;
        }

        private List<string> ExtractKeyTerms(string query)
        {
            var terms = new List<string>();
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var cleanWord = word.Trim().ToLower();
                
                // Skip stop words
                if (_stopWords.Contains(cleanWord))
                    continue;

                // Skip very short words unless they might be abbreviations
                if (cleanWord.Length < 3 && !cleanWord.All(char.IsUpper))
                    continue;

                terms.Add(cleanWord);
            }

            // Also extract multi-word phrases
            terms.AddRange(ExtractPhrases(query));

            return terms.Distinct().ToList();
        }

        private List<string> ExtractPhrases(string query)
        {
            var phrases = new List<string>();
            
            // Common business phrases
            var businessPhrases = new[]
            {
                "year to date", "month to date", "quarter to date",
                "fiscal year", "calendar year", "rolling average",
                "running total", "year over year", "month over month"
            };

            foreach (var phrase in businessPhrases)
            {
                if (query.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    phrases.Add(phrase);
                }
            }

            return phrases;
        }

        private List<string> ExtractEntities(string query)
        {
            var entities = new List<string>();
            
            // Extract quoted values
            var quotedPattern = @"['""]([^'""]+)['""]";
            var matches = Regex.Matches(query, quotedPattern);
            
            foreach (Match match in matches)
            {
                entities.Add(match.Groups[1].Value);
            }

            // Extract capitalized words (potential entity names)
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (word.Length > 0 && char.IsUpper(word[0]) && !_stopWords.Contains(word.ToLower()))
                {
                    entities.Add(word);
                }
            }

            return entities.Distinct().ToList();
        }

        private string DetermineIntent(string query)
        {
            var lowerQuery = query.ToLower();

            if (ContainsAggregation(lowerQuery))
            {
                return "AGGREGATE";
            }
            
            if (lowerQuery.Contains("top") || lowerQuery.Contains("first") || lowerQuery.Contains("last"))
            {
                return "TOP_N";
            }
            
            if (lowerQuery.Contains("between") || lowerQuery.Contains("from") && lowerQuery.Contains("to"))
            {
                return "RANGE";
            }
            
            if (lowerQuery.Contains("group by") || lowerQuery.Contains("by") && ContainsAggregation(lowerQuery))
            {
                return "GROUP";
            }
            
            if (lowerQuery.Contains("compare") || lowerQuery.Contains("versus") || lowerQuery.Contains("vs"))
            {
                return "COMPARE";
            }

            return "SELECT";
        }

        private bool ContainsAggregation(string query)
        {
            return _aggregationKeywords.Keys.Any(keyword => query.Contains(keyword));
        }

        private List<string> ExtractTimeReferences(string query)
        {
            var timeRefs = new List<string>();
            
            foreach (var timeKeyword in _timeKeywords.Keys)
            {
                if (query.Contains(timeKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    timeRefs.Add(timeKeyword);
                }
            }

            // Extract date patterns
            var datePattern = @"\b(\d{1,2}[/-]\d{1,2}[/-]\d{2,4}|\d{4}[/-]\d{1,2}[/-]\d{1,2})\b";
            var dateMatches = Regex.Matches(query, datePattern);
            
            foreach (Match match in dateMatches)
            {
                timeRefs.Add(match.Value);
            }

            // Extract month names
            var months = new[] { "january", "february", "march", "april", "may", "june", 
                               "july", "august", "september", "october", "november", "december" };
            
            foreach (var month in months)
            {
                if (query.Contains(month, StringComparison.OrdinalIgnoreCase))
                {
                    timeRefs.Add(month);
                }
            }

            return timeRefs;
        }

        private List<string> ExtractNumericReferences(string query)
        {
            var numericRefs = new List<string>();
            
            // Extract numbers
            var numberPattern = @"\b\d+(\.\d+)?\b";
            var matches = Regex.Matches(query, numberPattern);
            
            foreach (Match match in matches)
            {
                numericRefs.Add(match.Value);
            }

            // Extract number words
            var numberWords = new[] { "one", "two", "three", "four", "five", "six", 
                                    "seven", "eight", "nine", "ten", "hundred", "thousand", "million" };
            
            foreach (var word in numberWords)
            {
                if (query.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    numericRefs.Add(word);
                }
            }

            return numericRefs;
        }
    }
}

// Services/SqlServerMetadataRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public class SqlServerMetadataRepository : IMetadataRepository
    {
        private readonly string _connectionString;

        public SqlServerMetadataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<BusinessTableInfo>> GetAllTablesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT 
                    Id, TableName, SchemaName, BusinessPurpose, BusinessContext,
                    PrimaryUseCase, CommonQueryPatterns, BusinessRules,
                    DomainClassification, NaturalLanguageAliases, UsagePatterns,
                    DataQualityIndicators, RelationshipSemantics, ImportanceScore,
                    UsageFrequency, BusinessOwner, DataGovernancePolicies,
                    SemanticDescription, BusinessProcesses, AnalyticalUseCases,
                    ReportingCategories, SemanticRelationships, QueryComplexityHints,
                    BusinessGlossaryTerms, SemanticCoverageScore, LLMContextHints,
                    VectorSearchKeywords, RelatedBusinessTerms, BusinessFriendlyName,
                    NaturalLanguageDescription, RelationshipContext
                FROM BusinessTableInfo
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var tables = await connection.QueryAsync<BusinessTableInfo>(query);
            return tables.ToList();
        }

        public async Task<BusinessTableInfo> GetTableWithColumnsAsync(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // Get table info
            var tableQuery = @"
                SELECT * FROM BusinessTableInfo 
                WHERE TableName = @TableName AND IsActive = 1";
            
            var table = await connection.QuerySingleOrDefaultAsync<BusinessTableInfo>(
                tableQuery, new { TableName = tableName });
            
            if (table == null) return null;

            // Get columns
            var columnQuery = @"
                SELECT 
                    c.*
                FROM BusinessColumnInfo c
                WHERE c.TableInfoId = @TableId AND c.IsActive = 1
                ORDER BY c.ImportanceScore DESC, c.UsageFrequency DESC";
            
            var columns = await connection.QueryAsync<BusinessColumnInfo>(
                columnQuery, new { TableId = table.Id });
            
            table.Columns = columns.ToList();
            return table;
        }

        public async Task<List<BusinessDomain>> GetDomainsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT * FROM BusinessDomain 
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC";
            
            var domains = await connection.QueryAsync<BusinessDomain>(query);
            return domains.ToList();
        }

        public async Task<List<BusinessGlossary>> GetGlossaryTermsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT * FROM BusinessGlossary 
                WHERE IsActive = 1
                ORDER BY UsageFrequency DESC, ImportanceScore DESC";
            
            var terms = await connection.QueryAsync<BusinessGlossary>(query);
            return terms.ToList();
        }

        public async Task<List<TableRelationship>> GetTableRelationshipsAsync(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // This assumes you have a table storing relationships
            // You might need to create this table or extract from foreign keys
            var query = @"
                SELECT 
                    FromTable, FromColumn, ToTable, ToColumn, 
                    RelationshipType, BusinessMeaning
                FROM TableRelationships
                WHERE FromTable = @TableName OR ToTable = @TableName";
            
            var relationships = await connection.QueryAsync<TableRelationship>(
                query, new { TableName = tableName });
            
            return relationships.ToList();
        }

        public async Task<List<BusinessTableInfo>> GetTablesByDomainAsync(string domainName)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT t.* 
                FROM BusinessTableInfo t
                WHERE t.DomainClassification = @DomainName 
                   AND t.IsActive = 1
                ORDER BY t.ImportanceScore DESC";
            
            var tables = await connection.QueryAsync<BusinessTableInfo>(
                query, new { DomainName = domainName });
            
            return tables.ToList();
        }

        public async Task<List<BusinessGlossary>> GetGlossaryTermsByTablesAsync(List<string> tableNames)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT DISTINCT g.*
                FROM BusinessGlossary g
                WHERE g.IsActive = 1
                  AND (
                    EXISTS (
                        SELECT 1 
                        WHERE g.MappedTables LIKE '%' + t.value + '%'
                        FROM STRING_SPLIT(@TableNames, ',') t
                    )
                    OR
                    EXISTS (
                        SELECT 1
                        FROM BusinessTableInfo ti
                        WHERE ti.TableName IN (SELECT value FROM STRING_SPLIT(@TableNames, ','))
                          AND g.RelatedBusinessTerms LIKE '%' + ti.BusinessFriendlyName + '%'
                    )
                  )
                ORDER BY g.UsageFrequency DESC, g.ImportanceScore DESC";
            
            var terms = await connection.QueryAsync<BusinessGlossary>(
                query, new { TableNames = string.Join(",", tableNames) });
            
            return terms.ToList();
        }
    }
}

// Services/OpenAILLMService.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NL2Sql.Services
{
    public class OpenAILLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly OpenAIConfiguration _config;

        public OpenAILLMService(string apiKey, OpenAIConfiguration config = null)
        {
            _apiKey = apiKey;
            _config = config ?? new OpenAIConfiguration();
            _model = _config.Model;
            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GenerateSqlAsync(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a SQL expert. Generate only valid SQL queries without any explanation or markdown formatting." },
                    new { role = "user", content = prompt }
                },
                temperature = _config.Temperature,
                max_tokens = _config.MaxTokens,
                top_p = _config.TopP,
                frequency_penalty = _config.FrequencyPenalty,
                presence_penalty = _config.PresencePenalty
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions", content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

            var generatedSql = responseData?.choices?[0]?.message?.content?.Trim();
            
            // Clean up the SQL if it contains markdown formatting
            generatedSql = CleanSqlOutput(generatedSql);
            
            return generatedSql;
        }

        private string CleanSqlOutput(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Remove markdown code blocks
            sql = System.Text.RegularExpressions.Regex.Replace(
                sql, @"```sql\s*|```\s*", "", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove any leading/trailing whitespace
            sql = sql.Trim();

            return sql;
        }

        private class OpenAIResponse
        {
            public Choice[] choices { get; set; }
        }

        private class Choice
        {
            public Message message { get; set; }
        }

        private class Message
        {
            public string content { get; set; }
        }
    }

    public class OpenAIConfiguration
    {
        public string Model { get; set; } = "gpt-4-turbo-preview";
        public double Temperature { get; set; } = 0.1;
        public int MaxTokens { get; set; } = 2000;
        public double TopP { get; set; } = 1.0;
        public double FrequencyPenalty { get; set; } = 0.0;
        public double PresencePenalty { get; set; } = 0.0;
    }
}

// Extensions/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NL2Sql.Services;

namespace NL2Sql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNL2Sql(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Get configuration
            var connectionString = configuration.GetConnectionString("MetadataDb");
            var openAiKey = configuration["OpenAI:ApiKey"];
            var openAiConfig = configuration.GetSection("OpenAI").Get<OpenAIConfiguration>();

            // Register services
            services.AddSingleton<IMetadataRepository>(
                new SqlServerMetadataRepository(connectionString));
            
            services.AddSingleton<INaturalLanguageAnalyzer, SimpleNaturalLanguageAnalyzer>();
            
            services.AddSingleton<SchemaContextBuilder>();
            
            services.AddSingleton(new PromptConfiguration
            {
                IncludeExamples = true,
                UseChainOfThought = true,
                IncludeBusinessGlossary = true
            });
            
            services.AddSingleton<SqlPromptBuilder>();
            
            services.AddSingleton<ILLMService>(
                new OpenAILLMService(openAiKey, openAiConfig));
            
            services.AddScoped<NL2SqlService>();

            return services;
        }
    }
}

// Example: Program.cs for testing
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NL2Sql.Services;
using NL2Sql.Extensions;

namespace NL2Sql.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            // Setup DI
            var serviceProvider = new ServiceCollection()
                .AddNL2Sql(configuration)
                .BuildServiceProvider();

            // Get the service
            var nl2SqlService = serviceProvider.GetRequiredService<NL2SqlService>();

            // Test queries
            var testQueries = new[]
            {
                "Show me top 10 customers by total revenue",
                "What were the sales trends last month?",
                "Find all orders from customers in New York with order value over 1000",
                "Calculate the average order value by product category this year",
                "Which products have the highest profit margin?"
            };

            foreach (var query in testQueries)
            {
                Console.WriteLine($"\nQuery: {query}");
                Console.WriteLine(new string('-', 50));
                
                var result = await nl2SqlService.GenerateSqlAsync(query);
                
                if (result.Success)
                {
                    Console.WriteLine($"Generated SQL:\n{result.GeneratedSql}");
                }
                else
                {
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }
            }
        }
    }
}

// appsettings.json example
/*
{
  "ConnectionStrings": {
    "MetadataDb": "Server=localhost;Database=BIReportingCopilot_Dev;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "Model": "gpt-4-turbo-preview",
    "Temperature": 0.1,
    "MaxTokens": 2000
  }
}
*/