using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Enhanced;
using NL2SQL.Core.Interfaces.Enhanced;
using System.Text.Json;

namespace NL2SQL.Core.Repositories.Enhanced
{
    /// <summary>
    /// Enhanced metadata repository with semantic capabilities
    /// </summary>
    public class EnhancedMetadataRepository : IEnhancedMetadataRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EnhancedMetadataRepository> _logger;

        public EnhancedMetadataRepository(string connectionString, ILogger<EnhancedMetadataRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<EnhancedBusinessTableInfo>> GetEnhancedTableInfoAsync()
        {
            const string sql = @"
                SELECT 
                    Id, TableName, SchemaName, BusinessPurpose, BusinessContext, PrimaryUseCase,
                    CommonQueryPatterns, BusinessRules, DomainClassification, NaturalLanguageAliases,
                    UsagePatterns, DataQualityIndicators, RelationshipSemantics, ImportanceScore,
                    UsageFrequency, BusinessOwner, DataGovernancePolicies, SemanticDescription,
                    BusinessProcesses, AnalyticalUseCases, ReportingCategories, SemanticRelationships,
                    QueryComplexityHints, BusinessGlossaryTerms, SemanticCoverageScore, LLMContextHints,
                    VectorSearchKeywords, RelatedBusinessTerms, BusinessFriendlyName, 
                    NaturalLanguageDescription, RelationshipContext, DataGovernanceLevel, LastBusinessReview
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessTableInfo]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var tables = new List<EnhancedBusinessTableInfo>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var table = new EnhancedBusinessTableInfo
                {
                    Id = reader.GetInt32("Id"),
                    TableName = reader.GetString("TableName"),
                    SchemaName = reader.GetString("SchemaName"),
                    BusinessPurpose = reader.IsDBNull("BusinessPurpose") ? null : reader.GetString("BusinessPurpose"),
                    BusinessContext = reader.IsDBNull("BusinessContext") ? null : reader.GetString("BusinessContext"),
                    PrimaryUseCase = reader.IsDBNull("PrimaryUseCase") ? null : reader.GetString("PrimaryUseCase"),
                    CommonQueryPatterns = reader.IsDBNull("CommonQueryPatterns") ? null : reader.GetString("CommonQueryPatterns"),
                    BusinessRules = reader.IsDBNull("BusinessRules") ? null : reader.GetString("BusinessRules"),
                    DomainClassification = reader.IsDBNull("DomainClassification") ? null : reader.GetString("DomainClassification"),
                    NaturalLanguageAliases = reader.IsDBNull("NaturalLanguageAliases") ? null : reader.GetString("NaturalLanguageAliases"),
                    UsagePatterns = reader.IsDBNull("UsagePatterns") ? null : reader.GetString("UsagePatterns"),
                    DataQualityIndicators = reader.IsDBNull("DataQualityIndicators") ? null : reader.GetString("DataQualityIndicators"),
                    RelationshipSemantics = reader.IsDBNull("RelationshipSemantics") ? null : reader.GetString("RelationshipSemantics"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : reader.GetFloat("ImportanceScore"),
                    UsageFrequency = reader.IsDBNull("UsageFrequency") ? 0f : reader.GetFloat("UsageFrequency"),
                    BusinessOwner = reader.IsDBNull("BusinessOwner") ? null : reader.GetString("BusinessOwner"),
                    DataGovernancePolicies = reader.IsDBNull("DataGovernancePolicies") ? null : reader.GetString("DataGovernancePolicies"),
                    SemanticDescription = reader.IsDBNull("SemanticDescription") ? null : reader.GetString("SemanticDescription"),
                    BusinessProcesses = reader.IsDBNull("BusinessProcesses") ? null : reader.GetString("BusinessProcesses"),
                    AnalyticalUseCases = reader.IsDBNull("AnalyticalUseCases") ? null : reader.GetString("AnalyticalUseCases"),
                    ReportingCategories = reader.IsDBNull("ReportingCategories") ? null : reader.GetString("ReportingCategories"),
                    SemanticRelationships = reader.IsDBNull("SemanticRelationships") ? null : reader.GetString("SemanticRelationships"),
                    QueryComplexityHints = reader.IsDBNull("QueryComplexityHints") ? null : reader.GetString("QueryComplexityHints"),
                    BusinessGlossaryTerms = reader.IsDBNull("BusinessGlossaryTerms") ? null : reader.GetString("BusinessGlossaryTerms"),
                    SemanticCoverageScore = reader.IsDBNull("SemanticCoverageScore") ? 0f : reader.GetFloat("SemanticCoverageScore"),
                    LLMContextHints = reader.IsDBNull("LLMContextHints") ? null : reader.GetString("LLMContextHints"),
                    VectorSearchKeywords = reader.IsDBNull("VectorSearchKeywords") ? null : reader.GetString("VectorSearchKeywords"),
                    RelatedBusinessTerms = reader.IsDBNull("RelatedBusinessTerms") ? null : reader.GetString("RelatedBusinessTerms"),
                    BusinessFriendlyName = reader.IsDBNull("BusinessFriendlyName") ? null : reader.GetString("BusinessFriendlyName"),
                    NaturalLanguageDescription = reader.IsDBNull("NaturalLanguageDescription") ? null : reader.GetString("NaturalLanguageDescription"),
                    RelationshipContext = reader.IsDBNull("RelationshipContext") ? null : reader.GetString("RelationshipContext")
                };

                // Parse semantic tags from business glossary terms
                if (!string.IsNullOrEmpty(table.BusinessGlossaryTerms))
                {
                    table.SemanticTags = ParseSemanticTags(table.BusinessGlossaryTerms);
                }

                // Parse query patterns
                if (!string.IsNullOrEmpty(table.CommonQueryPatterns))
                {
                    table.CommonQueryPatterns = ParseQueryPatterns(table.CommonQueryPatterns);
                }

                // Set domain context
                table.DomainContext = CreateDomainContext(table.DomainClassification);

                // Calculate semantic relevance score
                table.SemanticRelevanceScore = CalculateSemanticRelevance(table);

                tables.Add(table);
            }

            // Load columns for each table
            foreach (var table in tables)
            {
                table.Columns = await GetBusinessColumnInfoAsync(table.Id);
            }

            return tables;
        }

        public async Task<List<EnhancedBusinessColumnInfo>> GetEnhancedColumnInfoAsync(int tableId)
        {
            const string sql = @"
                SELECT 
                    Id, TableInfoId, ColumnName, BusinessMeaning, BusinessContext, DataExamples,
                    ValidationRules, IsKeyColumn, NaturalLanguageAliases, ValueExamples, DataLineage,
                    CalculationRules, SemanticTags, BusinessDataType, ConstraintsAndRules,
                    DataQualityScore, UsageFrequency, PreferredAggregation, RelatedBusinessTerms,
                    IsSensitiveData, IsCalculatedField, SemanticContext, ConceptualRelationships,
                    DomainSpecificTerms, QueryIntentMapping, BusinessQuestionTypes, SemanticSynonyms,
                    AnalyticalContext, BusinessMetrics, SemanticRelevanceScore, LLMPromptHints,
                    VectorSearchTags, BusinessPurpose, BusinessFriendlyName, NaturalLanguageDescription,
                    BusinessRules, RelationshipContext, ImportanceScore
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessColumnInfo]
                WHERE TableInfoId = @TableId AND IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var columns = new List<EnhancedBusinessColumnInfo>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TableId", tableId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var column = new EnhancedBusinessColumnInfo
                {
                    Id = reader.GetInt32("Id"),
                    TableInfoId = reader.GetInt32("TableInfoId"),
                    ColumnName = reader.GetString("ColumnName"),
                    BusinessMeaning = reader.IsDBNull("BusinessMeaning") ? null : reader.GetString("BusinessMeaning"),
                    BusinessContext = reader.IsDBNull("BusinessContext") ? null : reader.GetString("BusinessContext"),
                    DataExamples = reader.IsDBNull("DataExamples") ? null : reader.GetString("DataExamples"),
                    ValidationRules = reader.IsDBNull("ValidationRules") ? null : reader.GetString("ValidationRules"),
                    IsKeyColumn = reader.GetBoolean("IsKeyColumn"),
                    NaturalLanguageAliases = reader.IsDBNull("NaturalLanguageAliases") ? null : reader.GetString("NaturalLanguageAliases"),
                    ValueExamples = reader.IsDBNull("ValueExamples") ? null : reader.GetString("ValueExamples"),
                    DataLineage = reader.IsDBNull("DataLineage") ? null : reader.GetString("DataLineage"),
                    CalculationRules = reader.IsDBNull("CalculationRules") ? null : reader.GetString("CalculationRules"),
                    SemanticTags = reader.IsDBNull("SemanticTags") ? null : reader.GetString("SemanticTags"),
                    BusinessDataType = reader.IsDBNull("BusinessDataType") ? null : reader.GetString("BusinessDataType"),
                    ConstraintsAndRules = reader.IsDBNull("ConstraintsAndRules") ? null : reader.GetString("ConstraintsAndRules"),
                    DataQualityScore = reader.IsDBNull("DataQualityScore") ? 0f : reader.GetFloat("DataQualityScore"),
                    UsageFrequency = reader.IsDBNull("UsageFrequency") ? 0f : reader.GetFloat("UsageFrequency"),
                    PreferredAggregation = reader.IsDBNull("PreferredAggregation") ? null : reader.GetString("PreferredAggregation"),
                    RelatedBusinessTerms = reader.IsDBNull("RelatedBusinessTerms") ? null : reader.GetString("RelatedBusinessTerms"),
                    IsSensitiveData = reader.GetBoolean("IsSensitiveData"),
                    IsCalculatedField = reader.GetBoolean("IsCalculatedField"),
                    SemanticContext = reader.IsDBNull("SemanticContext") ? null : reader.GetString("SemanticContext"),
                    ConceptualRelationships = reader.IsDBNull("ConceptualRelationships") ? null : reader.GetString("ConceptualRelationships"),
                    DomainSpecificTerms = reader.IsDBNull("DomainSpecificTerms") ? null : reader.GetString("DomainSpecificTerms"),
                    QueryIntentMapping = reader.IsDBNull("QueryIntentMapping") ? null : reader.GetString("QueryIntentMapping"),
                    BusinessQuestionTypes = reader.IsDBNull("BusinessQuestionTypes") ? null : reader.GetString("BusinessQuestionTypes"),
                    SemanticSynonyms = reader.IsDBNull("SemanticSynonyms") ? null : reader.GetString("SemanticSynonyms"),
                    AnalyticalContext = reader.IsDBNull("AnalyticalContext") ? null : reader.GetString("AnalyticalContext"),
                    BusinessMetrics = reader.IsDBNull("BusinessMetrics") ? null : reader.GetString("BusinessMetrics"),
                    SemanticRelevanceScore = reader.IsDBNull("SemanticRelevanceScore") ? 0f : reader.GetFloat("SemanticRelevanceScore"),
                    LLMPromptHints = reader.IsDBNull("LLMPromptHints") ? null : reader.GetString("LLMPromptHints"),
                    VectorSearchTags = reader.IsDBNull("VectorSearchTags") ? null : reader.GetString("VectorSearchTags"),
                    BusinessPurpose = reader.IsDBNull("BusinessPurpose") ? null : reader.GetString("BusinessPurpose"),
                    BusinessFriendlyName = reader.IsDBNull("BusinessFriendlyName") ? null : reader.GetString("BusinessFriendlyName"),
                    NaturalLanguageDescription = reader.IsDBNull("NaturalLanguageDescription") ? null : reader.GetString("NaturalLanguageDescription"),
                    BusinessRules = reader.IsDBNull("BusinessRules") ? null : reader.GetString("BusinessRules"),
                    RelationshipContext = reader.IsDBNull("RelationshipContext") ? null : reader.GetString("RelationshipContext"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : reader.GetFloat("ImportanceScore")
                };

                // Parse natural language synonyms
                if (!string.IsNullOrEmpty(column.SemanticSynonyms))
                {
                    column.NaturalLanguageSynonyms = ParseJsonArray(column.SemanticSynonyms);
                }

                // Parse mapped concepts
                if (!string.IsNullOrEmpty(column.RelatedBusinessTerms))
                {
                    column.MappedConcepts = await ParseBusinessConceptsAsync(column.RelatedBusinessTerms);
                }

                // Set data sensitivity
                column.DataSensitivity = CreateSensitivityClassification(column.IsSensitiveData);

                // Calculate column relevance score
                column.ColumnRelevanceScore = CalculateColumnRelevance(column);

                columns.Add(column);
            }

            return columns;
        }

        public async Task<List<BusinessGlossary>> GetBusinessGlossaryAsync()
        {
            const string sql = @"
                SELECT 
                    Id, Term, Definition, BusinessContext, Synonyms, RelatedTerms, Category,
                    Domain, Examples, MappedTables, MappedColumns, HierarchicalRelations,
                    PreferredCalculation, DisambiguationRules, BusinessOwner, ConfidenceScore,
                    AmbiguityScore, ContextualVariations, QueryPatterns, LLMPromptTemplates,
                    DisambiguationContext, SemanticRelationships, ConceptualLevel,
                    CrossDomainMappings, SemanticStability, InferenceRules, BusinessPurpose,
                    RelatedBusinessTerms, BusinessFriendlyName, NaturalLanguageDescription,
                    BusinessRules, ImportanceScore, UsageFrequency, RelationshipContext
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessGlossary]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var glossaryTerms = new List<BusinessGlossary>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var term = new BusinessGlossary
                {
                    Id = reader.GetInt32("Id"),
                    Term = reader.GetString("Term"),
                    Definition = reader.IsDBNull("Definition") ? null : reader.GetString("Definition"),
                    BusinessContext = reader.IsDBNull("BusinessContext") ? null : reader.GetString("BusinessContext"),
                    Synonyms = reader.IsDBNull("Synonyms") ? null : reader.GetString("Synonyms"),
                    RelatedTerms = reader.IsDBNull("RelatedTerms") ? null : reader.GetString("RelatedTerms"),
                    Category = reader.IsDBNull("Category") ? null : reader.GetString("Category"),
                    Domain = reader.IsDBNull("Domain") ? null : reader.GetString("Domain"),
                    Examples = reader.IsDBNull("Examples") ? null : reader.GetString("Examples"),
                    MappedTables = reader.IsDBNull("MappedTables") ? null : reader.GetString("MappedTables"),
                    MappedColumns = reader.IsDBNull("MappedColumns") ? null : reader.GetString("MappedColumns"),
                    HierarchicalRelations = reader.IsDBNull("HierarchicalRelations") ? null : reader.GetString("HierarchicalRelations"),
                    PreferredCalculation = reader.IsDBNull("PreferredCalculation") ? null : reader.GetString("PreferredCalculation"),
                    DisambiguationRules = reader.IsDBNull("DisambiguationRules") ? null : reader.GetString("DisambiguationRules"),
                    BusinessOwner = reader.IsDBNull("BusinessOwner") ? null : reader.GetString("BusinessOwner"),
                    ConfidenceScore = reader.IsDBNull("ConfidenceScore") ? 0f : reader.GetFloat("ConfidenceScore"),
                    AmbiguityScore = reader.IsDBNull("AmbiguityScore") ? 0f : reader.GetFloat("AmbiguityScore"),
                    ContextualVariations = reader.IsDBNull("ContextualVariations") ? null : reader.GetString("ContextualVariations"),
                    QueryPatterns = reader.IsDBNull("QueryPatterns") ? null : reader.GetString("QueryPatterns"),
                    LLMPromptTemplates = reader.IsDBNull("LLMPromptTemplates") ? null : reader.GetString("LLMPromptTemplates"),
                    DisambiguationContext = reader.IsDBNull("DisambiguationContext") ? null : reader.GetString("DisambiguationContext"),
                    SemanticRelationships = reader.IsDBNull("SemanticRelationships") ? null : reader.GetString("SemanticRelationships"),
                    ConceptualLevel = reader.IsDBNull("ConceptualLevel") ? null : reader.GetString("ConceptualLevel"),
                    CrossDomainMappings = reader.IsDBNull("CrossDomainMappings") ? null : reader.GetString("CrossDomainMappings"),
                    SemanticStability = reader.IsDBNull("SemanticStability") ? 0f : reader.GetFloat("SemanticStability"),
                    InferenceRules = reader.IsDBNull("InferenceRules") ? null : reader.GetString("InferenceRules"),
                    BusinessPurpose = reader.IsDBNull("BusinessPurpose") ? null : reader.GetString("BusinessPurpose"),
                    RelatedBusinessTerms = reader.IsDBNull("RelatedBusinessTerms") ? null : reader.GetString("RelatedBusinessTerms"),
                    BusinessFriendlyName = reader.IsDBNull("BusinessFriendlyName") ? null : reader.GetString("BusinessFriendlyName"),
                    NaturalLanguageDescription = reader.IsDBNull("NaturalLanguageDescription") ? null : reader.GetString("NaturalLanguageDescription"),
                    BusinessRules = reader.IsDBNull("BusinessRules") ? null : reader.GetString("BusinessRules"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : reader.GetFloat("ImportanceScore"),
                    UsageFrequency = reader.IsDBNull("UsageFrequency") ? 0f : reader.GetFloat("UsageFrequency"),
                    RelationshipContext = reader.IsDBNull("RelationshipContext") ? null : reader.GetString("RelationshipContext")
                };

                glossaryTerms.Add(term);
            }

            return glossaryTerms;
        }

        public async Task<List<BusinessDomain>> GetBusinessDomainsAsync()
        {
            const string sql = @"
                SELECT 
                    Id, DomainName, Description, RelatedTables, KeyConcepts, CommonQueries,
                    BusinessOwner, RelatedDomains, ImportanceScore, BusinessPurpose,
                    RelatedBusinessTerms, BusinessFriendlyName, NaturalLanguageDescription,
                    BusinessRules, RelationshipContext, UsageFrequency
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessDomain]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC";

            var domains = new List<BusinessDomain>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var domain = new BusinessDomain
                {
                    Id = reader.GetInt32("Id"),
                    DomainName = reader.GetString("DomainName"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    RelatedTables = reader.IsDBNull("RelatedTables") ? null : reader.GetString("RelatedTables"),
                    KeyConcepts = reader.IsDBNull("KeyConcepts") ? null : reader.GetString("KeyConcepts"),
                    CommonQueries = reader.IsDBNull("CommonQueries") ? null : reader.GetString("CommonQueries"),
                    BusinessOwner = reader.IsDBNull("BusinessOwner") ? null : reader.GetString("BusinessOwner"),
                    RelatedDomains = reader.IsDBNull("RelatedDomains") ? null : reader.GetString("RelatedDomains"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : reader.GetFloat("ImportanceScore"),
                    BusinessPurpose = reader.IsDBNull("BusinessPurpose") ? null : reader.GetString("BusinessPurpose"),
                    RelatedBusinessTerms = reader.IsDBNull("RelatedBusinessTerms") ? null : reader.GetString("RelatedBusinessTerms"),
                    BusinessFriendlyName = reader.IsDBNull("BusinessFriendlyName") ? null : reader.GetString("BusinessFriendlyName"),
                    NaturalLanguageDescription = reader.IsDBNull("NaturalLanguageDescription") ? null : reader.GetString("NaturalLanguageDescription"),
                    BusinessRules = reader.IsDBNull("BusinessRules") ? null : reader.GetString("BusinessRules"),
                    RelationshipContext = reader.IsDBNull("RelationshipContext") ? null : reader.GetString("RelationshipContext"),
                    UsageFrequency = reader.IsDBNull("UsageFrequency") ? 0f : reader.GetFloat("UsageFrequency")
                };

                domains.Add(domain);
            }

            return domains;
        }

        // Implementation of base interface methods
        public async Task<List<BusinessTableInfo>> GetTableInfoAsync()
        {
            var enhancedTables = await GetEnhancedTableInfoAsync();
            return enhancedTables.Cast<BusinessTableInfo>().ToList();
        }

        public async Task<BusinessTableInfo> GetTableInfoAsync(string tableName)
        {
            var tables = await GetEnhancedTableInfoAsync();
            return tables.FirstOrDefault(t => string.Equals(t.TableName, tableName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<BusinessColumnInfo>> GetBusinessColumnInfoAsync(int tableId)
        {
            var enhancedColumns = await GetEnhancedColumnInfoAsync(tableId);
            return enhancedColumns.Cast<BusinessColumnInfo>().ToList();
        }

        public async Task<List<TableRelationship>> GetTableRelationshipsAsync()
        {
            // Implementation would query relationship metadata
            return new List<TableRelationship>();
        }

        // Enhanced interface methods (placeholders for future implementation)
        public async Task<VectorEmbedding> GetTableEmbeddingAsync(string tableName)
        {
            // Placeholder - would retrieve or generate embeddings
            return null;
        }

        public async Task<VectorEmbedding> GetColumnEmbeddingAsync(string tableName, string columnName)
        {
            // Placeholder - would retrieve or generate embeddings
            return null;
        }

        public async Task<List<QueryPattern>> GetQueryPatternsAsync(string domain = null)
        {
            // Placeholder - would load query patterns from database
            return new List<QueryPattern>();
        }

        public async Task<List<CalculationTemplate>> GetCalculationTemplatesAsync(string domain = null)
        {
            // Placeholder - would load calculation templates
            return new List<CalculationTemplate>();
        }

        // Helper methods
        private List<SemanticTag> ParseSemanticTags(string tagsJson)
        {
            if (string.IsNullOrEmpty(tagsJson)) return new List<SemanticTag>();

            try
            {
                var tags = ParseJsonArray(tagsJson);
                return tags.Select(tag => new SemanticTag
                {
                    Name = tag,
                    Category = "Business",
                    Confidence = 0.8f,
                    Source = "Metadata"
                }).ToList();
            }
            catch
            {
                return new List<SemanticTag>();
            }
        }

        private List<QueryPattern> ParseQueryPatterns(string patternsJson)
        {
            // Simplified parsing - in production, this would parse actual query patterns
            return new List<QueryPattern>();
        }

        private BusinessDomainContext CreateDomainContext(string domainClassification)
        {
            return new BusinessDomainContext
            {
                DomainName = domainClassification ?? "General"
            };
        }

        private float CalculateSemanticRelevance(EnhancedBusinessTableInfo table)
        {
            var score = 0f;
            score += table.ImportanceScore * 0.4f;
            score += table.UsageFrequency * 0.3f;
            score += table.SemanticCoverageScore * 0.3f;
            return Math.Min(score, 1.0f);
        }

        private async Task<List<BusinessConcept>> ParseBusinessConceptsAsync(string conceptsJson)
        {
            if (string.IsNullOrEmpty(conceptsJson)) return new List<BusinessConcept>();

            try
            {
                var concepts = ParseJsonArray(conceptsJson);
                return concepts.Select(concept => new BusinessConcept
                {
                    ConceptId = concept,
                    Name = concept,
                    Type = ConceptType.Attribute,
                    Confidence = 0.8f
                }).ToList();
            }
            catch
            {
                return new List<BusinessConcept>();
            }
        }

        private SensitivityClassification CreateSensitivityClassification(bool isSensitive)
        {
            return new SensitivityClassification
            {
                Level = isSensitive ? SensitivityLevel.Confidential : SensitivityLevel.Internal,
                RequiresAuditLogging = isSensitive,
                RequiresDataMasking = isSensitive
            };
        }

        private float CalculateColumnRelevance(EnhancedBusinessColumnInfo column)
        {
            var score = 0f;
            score += column.ImportanceScore * 0.4f;
            score += column.UsageFrequency * 0.3f;
            score += column.SemanticRelevanceScore * 0.3f;
            return Math.Min(score, 1.0f);
        }

        private List<string> ParseJsonArray(string jsonArray)
        {
            if (string.IsNullOrEmpty(jsonArray)) return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(jsonArray) ?? new List<string>();
            }
            catch
            {
                // Fallback for non-JSON arrays
                return jsonArray.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim().Trim('"', '[', ']'))
                               .Where(s => !string.IsNullOrEmpty(s))
                               .ToList();
            }
        }
    }
}
