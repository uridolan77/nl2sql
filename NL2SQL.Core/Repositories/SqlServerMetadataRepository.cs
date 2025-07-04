using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Models;
using NL2SQL.Core.Interfaces;

namespace NL2SQL.Core.Repositories
{
    /// <summary>
    /// SQL Server implementation of metadata repository
    /// </summary>
    public class SqlServerMetadataRepository : IMetadataRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SqlServerMetadataRepository> _logger;

        public SqlServerMetadataRepository(string connectionString, ILogger<SqlServerMetadataRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<BusinessTableInfo>> GetTableInfoAsync()
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
                    NaturalLanguageDescription, RelationshipContext
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessTableInfo]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var tables = new List<BusinessTableInfo>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var table = new BusinessTableInfo
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

                    tables.Add(table);
                }

                // Load columns for each table
                foreach (var table in tables)
                {
                    table.Columns = await GetBusinessColumnInfoAsync(table.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading table information");
                throw;
            }

            return tables;
        }

        public async Task<BusinessTableInfo> GetTableInfoAsync(string tableName)
        {
            var tables = await GetTableInfoAsync();
            return tables.FirstOrDefault(t => string.Equals(t.TableName, tableName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<BusinessColumnInfo>> GetBusinessColumnInfoAsync(int tableId)
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

            var columns = new List<BusinessColumnInfo>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TableId", tableId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var column = new BusinessColumnInfo
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

                    columns.Add(column);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column information for table ID: {TableId}", tableId);
                throw;
            }

            return columns;
        }

        public async Task<List<TableRelationship>> GetTableRelationshipsAsync()
        {
            // Placeholder implementation - would query relationship metadata if available
            return new List<TableRelationship>();
        }
    }
}
