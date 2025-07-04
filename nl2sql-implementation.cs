// Models/BusinessMetadata.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NL2Sql.Models
{
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
        
        // Navigation property
        public BusinessTableInfo TableInfo { get; set; }
    }

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
        
        // Navigation property
        public List<BusinessColumnInfo> Columns { get; set; } = new List<BusinessColumnInfo>();
    }

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

    public class TableRelationship
    {
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
        public string RelationshipType { get; set; } // ONE_TO_MANY, MANY_TO_MANY, etc.
        public string BusinessMeaning { get; set; }
    }

    public class DatabaseSchema
    {
        public string SchemaName { get; set; }
        public List<BusinessTableInfo> Tables { get; set; } = new List<BusinessTableInfo>();
        public List<TableRelationship> Relationships { get; set; } = new List<TableRelationship>();
        public List<BusinessDomain> Domains { get; set; } = new List<BusinessDomain>();
        public List<BusinessGlossary> GlossaryTerms { get; set; } = new List<BusinessGlossary>();
    }
}

// Services/IMetadataRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public interface IMetadataRepository
    {
        Task<List<BusinessTableInfo>> GetAllTablesAsync();
        Task<BusinessTableInfo> GetTableWithColumnsAsync(string tableName);
        Task<List<BusinessDomain>> GetDomainsAsync();
        Task<List<BusinessGlossary>> GetGlossaryTermsAsync();
        Task<List<TableRelationship>> GetTableRelationshipsAsync(string tableName);
        Task<List<BusinessTableInfo>> GetTablesByDomainAsync(string domainName);
        Task<List<BusinessGlossary>> GetGlossaryTermsByTablesAsync(List<string> tableNames);
    }
}

// Services/SchemaContextBuilder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public class SchemaContextBuilder
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly INaturalLanguageAnalyzer _nlAnalyzer;

        public SchemaContextBuilder(
            IMetadataRepository metadataRepository,
            INaturalLanguageAnalyzer nlAnalyzer)
        {
            _metadataRepository = metadataRepository;
            _nlAnalyzer = nlAnalyzer;
        }

        public async Task<DatabaseSchema> BuildRelevantSchemaAsync(string naturalLanguageQuery)
        {
            // Extract key terms and entities from the query
            var queryAnalysis = await _nlAnalyzer.AnalyzeQueryAsync(naturalLanguageQuery);
            
            // Find relevant tables based on query analysis
            var relevantTables = await FindRelevantTablesAsync(queryAnalysis);
            
            // Load complete table information including columns
            var enrichedTables = await EnrichTablesWithMetadataAsync(relevantTables);
            
            // Find relationships between relevant tables
            var relationships = await GetRelevantRelationshipsAsync(enrichedTables);
            
            // Get relevant glossary terms
            var glossaryTerms = await GetRelevantGlossaryTermsAsync(enrichedTables, queryAnalysis);
            
            // Build the schema context
            return new DatabaseSchema
            {
                Tables = enrichedTables,
                Relationships = relationships,
                GlossaryTerms = glossaryTerms,
                Domains = await GetRelevantDomainsAsync(enrichedTables)
            };
        }

        private async Task<List<BusinessTableInfo>> FindRelevantTablesAsync(QueryAnalysis analysis)
        {
            var allTables = await _metadataRepository.GetAllTablesAsync();
            var relevantTables = new List<BusinessTableInfo>();
            var scores = new Dictionary<BusinessTableInfo, float>();

            foreach (var table in allTables)
            {
                float score = CalculateTableRelevanceScore(table, analysis);
                if (score > 0.3f) // Threshold for relevance
                {
                    scores[table] = score;
                    relevantTables.Add(table);
                }
            }

            // Sort by relevance score and take top tables
            return relevantTables
                .OrderByDescending(t => scores[t])
                .Take(10) // Limit to prevent context explosion
                .ToList();
        }

        private float CalculateTableRelevanceScore(BusinessTableInfo table, QueryAnalysis analysis)
        {
            float score = 0f;
            var tableContext = BuildTableContext(table);

            // Check for exact term matches
            foreach (var term in analysis.KeyTerms)
            {
                if (tableContext.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1.0f;
                }
            }

            // Check for semantic matches in aliases
            if (!string.IsNullOrEmpty(table.NaturalLanguageAliases))
            {
                var aliases = table.NaturalLanguageAliases.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var alias in aliases)
                {
                    if (analysis.NormalizedQuery.Contains(alias.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        score += 0.8f;
                    }
                }
            }

            // Check business glossary terms
            if (!string.IsNullOrEmpty(table.BusinessGlossaryTerms))
            {
                var glossaryTerms = table.BusinessGlossaryTerms.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in glossaryTerms)
                {
                    if (analysis.NormalizedQuery.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        score += 0.6f;
                    }
                }
            }

            // Boost score based on table importance and usage frequency
            score *= (table.ImportanceScore * 0.5f + table.UsageFrequency * 0.5f);

            return score;
        }

        private string BuildTableContext(BusinessTableInfo table)
        {
            var context = new StringBuilder();
            
            context.Append(table.TableName).Append(" ");
            context.Append(table.BusinessFriendlyName ?? "").Append(" ");
            context.Append(table.BusinessPurpose ?? "").Append(" ");
            context.Append(table.NaturalLanguageDescription ?? "").Append(" ");
            context.Append(table.SemanticDescription ?? "").Append(" ");
            context.Append(table.BusinessContext ?? "").Append(" ");
            context.Append(table.DomainClassification ?? "").Append(" ");
            context.Append(table.RelatedBusinessTerms ?? "");

            return context.ToString();
        }

        private async Task<List<BusinessTableInfo>> EnrichTablesWithMetadataAsync(List<BusinessTableInfo> tables)
        {
            var enrichedTables = new List<BusinessTableInfo>();

            foreach (var table in tables)
            {
                var fullTable = await _metadataRepository.GetTableWithColumnsAsync(table.TableName);
                
                // Filter columns based on relevance
                fullTable.Columns = FilterRelevantColumns(fullTable.Columns);
                
                enrichedTables.Add(fullTable);
            }

            return enrichedTables;
        }

        private List<BusinessColumnInfo> FilterRelevantColumns(List<BusinessColumnInfo> columns)
        {
            // Prioritize columns with high importance scores and usage frequency
            return columns
                .Where(c => c.ImportanceScore > 0.3 || c.IsKeyColumn || c.UsageFrequency > 0.5)
                .OrderByDescending(c => c.ImportanceScore * c.UsageFrequency)
                .ToList();
        }

        private async Task<List<TableRelationship>> GetRelevantRelationshipsAsync(List<BusinessTableInfo> tables)
        {
            var relationships = new List<TableRelationship>();
            var tableNames = tables.Select(t => t.TableName).ToHashSet();

            foreach (var table in tables)
            {
                var tableRelationships = await _metadataRepository.GetTableRelationshipsAsync(table.TableName);
                
                // Only include relationships between tables in our relevant set
                relationships.AddRange(tableRelationships.Where(r => 
                    tableNames.Contains(r.FromTable) && tableNames.Contains(r.ToTable)));
            }

            return relationships.Distinct().ToList();
        }

        private async Task<List<BusinessGlossary>> GetRelevantGlossaryTermsAsync(
            List<BusinessTableInfo> tables, 
            QueryAnalysis analysis)
        {
            var tableNames = tables.Select(t => t.TableName).ToList();
            var glossaryTerms = await _metadataRepository.GetGlossaryTermsByTablesAsync(tableNames);

            // Filter glossary terms based on query relevance
            return glossaryTerms
                .Where(term => IsGlossaryTermRelevant(term, analysis))
                .OrderByDescending(term => term.UsageFrequency * term.ImportanceScore)
                .Take(20) // Limit glossary terms
                .ToList();
        }

        private bool IsGlossaryTermRelevant(BusinessGlossary term, QueryAnalysis analysis)
        {
            var termContext = $"{term.Term} {term.Synonyms} {term.BusinessContext}".ToLower();
            
            return analysis.KeyTerms.Any(keyTerm => 
                termContext.Contains(keyTerm.ToLower()) || 
                keyTerm.ToLower().Contains(term.Term.ToLower()));
        }

        private async Task<List<BusinessDomain>> GetRelevantDomainsAsync(List<BusinessTableInfo> tables)
        {
            var domains = await _metadataRepository.GetDomainsAsync();
            var relevantDomains = new List<BusinessDomain>();

            foreach (var domain in domains)
            {
                var relatedTables = domain.RelatedTables?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                
                if (tables.Any(t => relatedTables.Contains(t.TableName, StringComparer.OrdinalIgnoreCase)))
                {
                    relevantDomains.Add(domain);
                }
            }

            return relevantDomains;
        }
    }

    public interface INaturalLanguageAnalyzer
    {
        Task<QueryAnalysis> AnalyzeQueryAsync(string query);
    }

    public class QueryAnalysis
    {
        public string NormalizedQuery { get; set; }
        public List<string> KeyTerms { get; set; } = new List<string>();
        public List<string> Entities { get; set; } = new List<string>();
        public string IntentType { get; set; } // SELECT, AGGREGATE, JOIN, etc.
        public List<string> TimeReferences { get; set; } = new List<string>();
        public List<string> NumericReferences { get; set; } = new List<string>();
    }
}

// Services/PromptBuilder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public class SqlPromptBuilder
    {
        private readonly PromptConfiguration _config;

        public SqlPromptBuilder(PromptConfiguration config)
        {
            _config = config;
        }

        public string BuildPrompt(string naturalLanguageQuery, DatabaseSchema schema)
        {
            var promptBuilder = new StringBuilder();

            // System instruction
            promptBuilder.AppendLine(BuildSystemInstruction());
            
            // Schema context
            promptBuilder.AppendLine(BuildSchemaContext(schema));
            
            // Business glossary
            if (schema.GlossaryTerms.Any())
            {
                promptBuilder.AppendLine(BuildGlossaryContext(schema.GlossaryTerms));
            }
            
            // Examples (few-shot learning)
            if (_config.IncludeExamples)
            {
                promptBuilder.AppendLine(BuildExamples(schema));
            }
            
            // Query instruction
            promptBuilder.AppendLine(BuildQueryInstruction(naturalLanguageQuery));
            
            // Chain of thought prompting
            if (_config.UseChainOfThought)
            {
                promptBuilder.AppendLine(BuildChainOfThoughtSection());
            }

            return promptBuilder.ToString();
        }

        private string BuildSystemInstruction()
        {
            return @"You are an expert SQL developer with deep knowledge of business data and analytics. 
Your task is to convert natural language questions into accurate, efficient SQL queries.

Important Guidelines:
- Generate ONLY valid SQL that can be executed directly
- Use proper JOIN syntax when combining tables
- Apply appropriate WHERE clauses based on the business context
- Use meaningful table and column aliases
- Consider data types and format dates/times appropriately
- Optimize for readability and performance
- Include appropriate aggregations when the question implies summarization
- Use business-friendly column aliases in the SELECT clause";
        }

        private string BuildSchemaContext(DatabaseSchema schema)
        {
            var schemaBuilder = new StringBuilder();
            
            schemaBuilder.AppendLine("\n### DATABASE SCHEMA ###");
            schemaBuilder.AppendLine($"Schema: {schema.SchemaName}");
            
            // Build domain context
            if (schema.Domains.Any())
            {
                schemaBuilder.AppendLine("\n## Business Domains:");
                foreach (var domain in schema.Domains)
                {
                    schemaBuilder.AppendLine($"- {domain.DomainName}: {domain.Description}");
                }
            }
            
            // Build table context
            schemaBuilder.AppendLine("\n## Tables:");
            foreach (var table in schema.Tables.OrderByDescending(t => t.ImportanceScore))
            {
                schemaBuilder.AppendLine(BuildTableContext(table));
            }
            
            // Build relationships
            if (schema.Relationships.Any())
            {
                schemaBuilder.AppendLine("\n## Table Relationships:");
                foreach (var rel in schema.Relationships)
                {
                    schemaBuilder.AppendLine($"- {rel.FromTable}.{rel.FromColumn} -> {rel.ToTable}.{rel.ToColumn} ({rel.RelationshipType})");
                    if (!string.IsNullOrEmpty(rel.BusinessMeaning))
                    {
                        schemaBuilder.AppendLine($"  Business meaning: {rel.BusinessMeaning}");
                    }
                }
            }

            return schemaBuilder.ToString();
        }

        private string BuildTableContext(BusinessTableInfo table)
        {
            var tableBuilder = new StringBuilder();
            
            tableBuilder.AppendLine($"\n### Table: {table.SchemaName}.{table.TableName}");
            
            // Business context
            if (!string.IsNullOrEmpty(table.BusinessFriendlyName))
            {
                tableBuilder.AppendLine($"Business Name: {table.BusinessFriendlyName}");
            }
            
            if (!string.IsNullOrEmpty(table.BusinessPurpose))
            {
                tableBuilder.AppendLine($"Purpose: {table.BusinessPurpose}");
            }
            
            if (!string.IsNullOrEmpty(table.NaturalLanguageDescription))
            {
                tableBuilder.AppendLine($"Description: {table.NaturalLanguageDescription}");
            }
            
            // Natural language aliases
            if (!string.IsNullOrEmpty(table.NaturalLanguageAliases))
            {
                tableBuilder.AppendLine($"Also known as: {table.NaturalLanguageAliases}");
            }
            
            // Common query patterns
            if (!string.IsNullOrEmpty(table.CommonQueryPatterns))
            {
                tableBuilder.AppendLine($"Common queries: {table.CommonQueryPatterns}");
            }
            
            // Columns
            tableBuilder.AppendLine("\nColumns:");
            foreach (var column in table.Columns.OrderByDescending(c => c.ImportanceScore))
            {
                tableBuilder.AppendLine(BuildColumnContext(column));
            }
            
            return tableBuilder.ToString();
        }

        private string BuildColumnContext(BusinessColumnInfo column)
        {
            var columnBuilder = new StringBuilder();
            
            columnBuilder.Append($"  - {column.ColumnName}");
            
            // Data type
            if (!string.IsNullOrEmpty(column.BusinessDataType))
            {
                columnBuilder.Append($" ({column.BusinessDataType})");
            }
            
            // Business name
            if (!string.IsNullOrEmpty(column.BusinessFriendlyName) && 
                column.BusinessFriendlyName != column.ColumnName)
            {
                columnBuilder.Append($" ['{column.BusinessFriendlyName}']");
            }
            
            // Key indicator
            if (column.IsKeyColumn)
            {
                columnBuilder.Append(" [KEY]");
            }
            
            // Business meaning
            if (!string.IsNullOrEmpty(column.BusinessMeaning))
            {
                columnBuilder.Append($"\n    Meaning: {column.BusinessMeaning}");
            }
            
            // Natural language aliases
            if (!string.IsNullOrEmpty(column.NaturalLanguageAliases))
            {
                columnBuilder.Append($"\n    Also called: {column.NaturalLanguageAliases}");
            }
            
            // Value examples
            if (!string.IsNullOrEmpty(column.ValueExamples))
            {
                columnBuilder.Append($"\n    Examples: {column.ValueExamples}");
            }
            
            // Calculation rules
            if (column.IsCalculatedField && !string.IsNullOrEmpty(column.CalculationRules))
            {
                columnBuilder.Append($"\n    Calculation: {column.CalculationRules}");
            }
            
            // Constraints
            if (!string.IsNullOrEmpty(column.ConstraintsAndRules))
            {
                columnBuilder.Append($"\n    Constraints: {column.ConstraintsAndRules}");
            }
            
            // LLM hints
            if (!string.IsNullOrEmpty(column.LLMPromptHints))
            {
                columnBuilder.Append($"\n    Hint: {column.LLMPromptHints}");
            }

            return columnBuilder.ToString();
        }

        private string BuildGlossaryContext(List<BusinessGlossary> glossaryTerms)
        {
            var glossaryBuilder = new StringBuilder();
            
            glossaryBuilder.AppendLine("\n### BUSINESS GLOSSARY ###");
            glossaryBuilder.AppendLine("Important business terms and their meanings:");
            
            foreach (var term in glossaryTerms.OrderByDescending(t => t.ImportanceScore))
            {
                glossaryBuilder.AppendLine($"\n- {term.Term}:");
                glossaryBuilder.AppendLine($"  Definition: {term.Definition}");
                
                if (!string.IsNullOrEmpty(term.Synonyms))
                {
                    glossaryBuilder.AppendLine($"  Synonyms: {term.Synonyms}");
                }
                
                if (!string.IsNullOrEmpty(term.PreferredCalculation))
                {
                    glossaryBuilder.AppendLine($"  Calculation: {term.PreferredCalculation}");
                }
                
                if (!string.IsNullOrEmpty(term.MappedColumns))
                {
                    glossaryBuilder.AppendLine($"  Maps to: {term.MappedColumns}");
                }
                
                if (!string.IsNullOrEmpty(term.LLMPromptTemplates))
                {
                    glossaryBuilder.AppendLine($"  SQL Pattern: {term.LLMPromptTemplates}");
                }
            }

            return glossaryBuilder.ToString();
        }

        private string BuildExamples(DatabaseSchema schema)
        {
            var exampleBuilder = new StringBuilder();
            
            exampleBuilder.AppendLine("\n### EXAMPLE QUERIES ###");
            
            // Get example queries from table metadata
            foreach (var table in schema.Tables.Where(t => !string.IsNullOrEmpty(t.CommonQueryPatterns)))
            {
                var patterns = table.CommonQueryPatterns.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pattern in patterns.Take(2)) // Limit examples per table
                {
                    if (pattern.Contains("=>"))
                    {
                        var parts = pattern.Split("=>", 2);
                        exampleBuilder.AppendLine($"\nQuestion: {parts[0].Trim()}");
                        exampleBuilder.AppendLine($"SQL: {parts[1].Trim()}");
                    }
                }
            }

            // Add general patterns
            exampleBuilder.AppendLine("\n## General Patterns:");
            exampleBuilder.AppendLine("- For 'top N' questions, use: SELECT TOP N ... ORDER BY ...");
            exampleBuilder.AppendLine("- For time-based queries, use appropriate date functions");
            exampleBuilder.AppendLine("- For aggregations, remember to GROUP BY non-aggregated columns");
            exampleBuilder.AppendLine("- Always use meaningful aliases for calculated fields");

            return exampleBuilder.ToString();
        }

        private string BuildQueryInstruction(string naturalLanguageQuery)
        {
            return $@"
### YOUR TASK ###
Convert the following natural language question into a SQL query:

Question: {naturalLanguageQuery}

Requirements:
- Generate ONLY the SQL query, no explanations
- The query must be executable against the schema provided
- Use appropriate JOINs based on the relationships defined
- Apply relevant business rules and calculations
- Consider the business context when interpreting ambiguous terms";
        }

        private string BuildChainOfThoughtSection()
        {
            return @"
### THINKING PROCESS ###
Before writing the SQL, consider:
1. What tables contain the requested information?
2. How are these tables related?
3. What filters should be applied?
4. What aggregations or calculations are needed?
5. What is the appropriate output format?

Now, generate the SQL query:";
        }
    }

    public class PromptConfiguration
    {
        public bool IncludeExamples { get; set; } = true;
        public bool UseChainOfThought { get; set; } = true;
        public int MaxExamplesPerTable { get; set; } = 2;
        public bool IncludeBusinessGlossary { get; set; } = true;
        public bool VerboseColumnDescriptions { get; set} = false;
    }
}

// Services/NL2SqlService.cs
using System;
using System.Threading.Tasks;
using NL2Sql.Models;

namespace NL2Sql.Services
{
    public interface ILLMService
    {
        Task<string> GenerateSqlAsync(string prompt);
    }

    public class NL2SqlService
    {
        private readonly SchemaContextBuilder _contextBuilder;
        private readonly SqlPromptBuilder _promptBuilder;
        private readonly ILLMService _llmService;

        public NL2SqlService(
            SchemaContextBuilder contextBuilder,
            SqlPromptBuilder promptBuilder,
            ILLMService llmService)
        {
            _contextBuilder = contextBuilder;
            _promptBuilder = promptBuilder;
            _llmService = llmService;
        }

        public async Task<SqlGenerationResult> GenerateSqlAsync(string naturalLanguageQuery)
        {
            try
            {
                // Build relevant schema context
                var schemaContext = await _contextBuilder.BuildRelevantSchemaAsync(naturalLanguageQuery);
                
                // Build the prompt
                var prompt = _promptBuilder.BuildPrompt(naturalLanguageQuery, schemaContext);
                
                // Generate SQL using LLM
                var generatedSql = await _llmService.GenerateSqlAsync(prompt);
                
                return new SqlGenerationResult
                {
                    Success = true,
                    GeneratedSql = generatedSql,
                    SchemaContext = schemaContext,
                    PromptUsed = prompt
                };
            }
            catch (Exception ex)
            {
                return new SqlGenerationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    GeneratedSql = null
                };
            }
        }
    }

    public class SqlGenerationResult
    {
        public bool Success { get; set; }
        public string GeneratedSql { get; set; }
        public string ErrorMessage { get; set; }
        public DatabaseSchema SchemaContext { get; set; }
        public string PromptUsed { get; set; }
    }
}

// Example Usage
namespace NL2Sql.Example
{
    public class ExampleUsage
    {
        public async Task Example()
        {
            // Setup services (using dependency injection in real app)
            var metadataRepo = new SqlServerMetadataRepository(connectionString);
            var nlAnalyzer = new SimpleNaturalLanguageAnalyzer();
            var contextBuilder = new SchemaContextBuilder(metadataRepo, nlAnalyzer);
            
            var promptConfig = new PromptConfiguration
            {
                IncludeExamples = true,
                UseChainOfThought = true,
                IncludeBusinessGlossary = true
            };
            var promptBuilder = new SqlPromptBuilder(promptConfig);
            
            var llmService = new OpenAILLMService(apiKey);
            var nl2SqlService = new NL2SqlService(contextBuilder, promptBuilder, llmService);
            
            // Generate SQL
            var result = await nl2SqlService.GenerateSqlAsync(
                "Show me the top 10 customers by total revenue last month"
            );
            
            if (result.Success)
            {
                Console.WriteLine($"Generated SQL: {result.GeneratedSql}");
                
                // The prompt that was sent to the LLM (for debugging)
                Console.WriteLine($"Prompt used: {result.PromptUsed}");
            }
        }
    }
}