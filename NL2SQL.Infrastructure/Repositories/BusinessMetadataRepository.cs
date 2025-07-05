using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Data;
using NL2SQL.Core.Data.Entities;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for accessing business metadata from the database
    /// </summary>
    public class BusinessMetadataRepository : IBusinessMetadataRepository
    {
        private readonly BusinessMetadataDbContext _context;
        private readonly ILogger<BusinessMetadataRepository> _logger;

        public BusinessMetadataRepository(
            BusinessMetadataDbContext context,
            ILogger<BusinessMetadataRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TableMetadata>> GetTableMetadataAsync()
        {
            try
            {
                _logger.LogInformation("Loading all active table metadata from database");

                var tables = await _context.BusinessTableInfo
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.ImportanceScore)
                    .ThenBy(t => t.TableName)
                    .ToListAsync();

                var result = tables.Select(MapToTableMetadata).ToList();

                _logger.LogInformation("Successfully loaded {Count} table metadata records", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading table metadata from database");
                throw;
            }
        }

        public async Task<List<ColumnMetadata>> GetColumnMetadataAsync()
        {
            try
            {
                _logger.LogInformation("Loading all active column metadata from database");

                var columns = await _context.BusinessColumnInfo
                    .Include(c => c.BusinessTable)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.TableInfoId)
                    .ThenByDescending(c => c.ImportanceScore)
                    .ThenBy(c => c.ColumnName)
                    .ToListAsync();

                var result = columns.Select(MapToColumnMetadata).ToList();

                _logger.LogInformation("Successfully loaded {Count} column metadata records", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column metadata from database");
                throw;
            }
        }

        public async Task<TableMetadata?> GetTableMetadataAsync(string tableName)
        {
            try
            {
                var table = await _context.BusinessTableInfo
                    .Where(t => t.IsActive && t.TableName == tableName)
                    .FirstOrDefaultAsync();

                return table != null ? MapToTableMetadata(table) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading table metadata for {TableName}", tableName);
                throw;
            }
        }

        public async Task<List<ColumnMetadata>> GetColumnMetadataAsync(string tableName)
        {
            try
            {
                var columns = await _context.BusinessColumnInfo
                    .Include(c => c.BusinessTable)
                    .Where(c => c.IsActive && c.BusinessTable != null && c.BusinessTable.TableName == tableName)
                    .OrderByDescending(c => c.ImportanceScore)
                    .ThenBy(c => c.ColumnName)
                    .ToListAsync();

                return columns.Select(MapToColumnMetadata).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column metadata for table {TableName}", tableName);
                throw;
            }
        }

        public async Task<ColumnMetadata?> GetColumnMetadataAsync(string tableName, string columnName)
        {
            try
            {
                var column = await _context.BusinessColumnInfo
                    .Include(c => c.BusinessTable)
                    .Where(c => c.IsActive && c.BusinessTable != null && c.BusinessTable.TableName == tableName && c.ColumnName == columnName)
                    .FirstOrDefaultAsync();

                return column != null ? MapToColumnMetadata(column) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading column metadata for {TableName}.{ColumnName}", tableName, columnName);
                throw;
            }
        }

        public async Task<List<TableMetadata>> GetTopTablesByImportanceAsync(int limit = 50)
        {
            try
            {
                var tables = await _context.BusinessTableInfo
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.ImportanceScore)
                    .ThenBy(t => t.TableName)
                    .Take(limit)
                    .ToListAsync();

                return tables.Select(MapToTableMetadata).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading top {Limit} tables by importance", limit);
                throw;
            }
        }

        public async Task<List<ColumnMetadata>> GetTopColumnsByImportanceAsync(int limit = 200)
        {
            try
            {
                var columns = await _context.BusinessColumnInfo
                    .Include(c => c.BusinessTable)
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.ImportanceScore)
                    .ThenBy(c => c.TableInfoId)
                    .ThenBy(c => c.ColumnName)
                    .Take(limit)
                    .ToListAsync();

                return columns.Select(MapToColumnMetadata).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading top {Limit} columns by importance", limit);
                throw;
            }
        }

        public async Task<List<TableMetadata>> SearchTablesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<TableMetadata>();

                var term = searchTerm.ToLower();

                var tables = await _context.BusinessTableInfo
                    .Where(t => t.IsActive && (
                        t.TableName.ToLower().Contains(term) ||
                        (t.BusinessPurpose != null && t.BusinessPurpose.ToLower().Contains(term)) ||
                        (t.DomainClassification != null && t.DomainClassification.ToLower().Contains(term)) ||
                        (t.BusinessContext != null && t.BusinessContext.ToLower().Contains(term))
                    ))
                    .OrderByDescending(t => t.ImportanceScore)
                    .ToListAsync();

                return tables.Select(MapToTableMetadata).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tables with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<List<ColumnMetadata>> SearchColumnsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<ColumnMetadata>();

                var term = searchTerm.ToLower();

                var columns = await _context.BusinessColumnInfo
                    .Include(c => c.BusinessTable)
                    .Where(c => c.IsActive && (
                        c.ColumnName.ToLower().Contains(term) ||
                        (c.BusinessMeaning != null && c.BusinessMeaning.ToLower().Contains(term)) ||
                        (c.BusinessContext != null && c.BusinessContext.ToLower().Contains(term)) ||
                        (c.SemanticTags != null && c.SemanticTags.ToLower().Contains(term))
                    ))
                    .OrderByDescending(c => c.ImportanceScore)
                    .ToListAsync();

                return columns.Select(MapToColumnMetadata).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching columns with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database connection test failed");
                return false;
            }
        }

        public async Task<DatabaseStatistics> GetDatabaseStatisticsAsync()
        {
            try
            {
                var totalTables = await _context.BusinessTableInfo.CountAsync();
                var activeTables = await _context.BusinessTableInfo.CountAsync(t => t.IsActive);
                var totalColumns = await _context.BusinessColumnInfo.CountAsync();
                var activeColumns = await _context.BusinessColumnInfo.CountAsync(c => c.IsActive);

                var topDomains = await _context.BusinessTableInfo
                    .Where(t => t.IsActive && t.DomainClassification != null)
                    .GroupBy(t => t.DomainClassification)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key!)
                    .ToListAsync();

                var avgTableImportance = await _context.BusinessTableInfo
                    .Where(t => t.IsActive)
                    .AverageAsync(t => t.ImportanceScore);

                var avgColumnImportance = await _context.BusinessColumnInfo
                    .Where(c => c.IsActive)
                    .AverageAsync(c => c.ImportanceScore);

                return new DatabaseStatistics
                {
                    TotalTables = totalTables,
                    ActiveTables = activeTables,
                    TotalColumns = totalColumns,
                    ActiveColumns = activeColumns,
                    LastUpdated = DateTime.UtcNow,
                    TopDomains = topDomains,
                    AverageTableImportance = avgTableImportance,
                    AverageColumnImportance = avgColumnImportance
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database statistics");
                throw;
            }
        }

        private static TableMetadata MapToTableMetadata(NL2SQL.Core.Data.Entities.BusinessTableInfo entity)
        {
            return new TableMetadata
            {
                TableName = entity.TableName,
                BusinessPurpose = entity.BusinessPurpose ?? string.Empty,
                Domain = entity.DomainClassification ?? "General",
                ImportanceScore = (float)entity.ImportanceScore,
                Keywords = GenerateTableKeywords(entity.TableName, entity.BusinessPurpose ?? string.Empty)
            };
        }

        private static ColumnMetadata MapToColumnMetadata(NL2SQL.Core.Data.Entities.BusinessColumnInfo entity)
        {
            return new ColumnMetadata
            {
                TableName = entity.BusinessTable?.TableName ?? "Unknown",
                ColumnName = entity.ColumnName,
                BusinessMeaning = entity.BusinessMeaning ?? string.Empty,
                DataType = entity.BusinessDataType ?? "varchar",
                Synonyms = GenerateColumnSynonyms(entity.ColumnName),
                Keywords = GenerateColumnKeywords(entity.ColumnName, entity.BusinessMeaning ?? string.Empty)
            };
        }

        private static List<string> GenerateTableKeywords(string tableName, string businessPurpose)
        {
            var keywords = new List<string> { tableName.ToLower() };
            
            if (!string.IsNullOrEmpty(businessPurpose))
            {
                keywords.AddRange(businessPurpose.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
            
            return keywords.Distinct().ToList();
        }

        private static List<string> GenerateColumnSynonyms(string columnName)
        {
            var synonyms = new List<string> { columnName.ToLower() };
            
            // Add common variations
            if (columnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
            {
                synonyms.Add(columnName.Substring(0, columnName.Length - 2).ToLower());
                synonyms.Add(columnName.ToLower().Replace("id", "identifier"));
            }
            
            return synonyms.Distinct().ToList();
        }

        private static List<string> GenerateColumnKeywords(string columnName, string businessMeaning)
        {
            var keywords = new List<string> { columnName.ToLower() };
            
            if (!string.IsNullOrEmpty(businessMeaning))
            {
                keywords.AddRange(businessMeaning.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
            
            return keywords.Distinct().ToList();
        }
    }
}
