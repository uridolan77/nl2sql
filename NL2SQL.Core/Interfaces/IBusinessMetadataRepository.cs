using NL2SQL.Core.Data.Entities;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Core.Interfaces
{
    /// <summary>
    /// Repository interface for accessing business metadata from the database
    /// </summary>
    public interface IBusinessMetadataRepository
    {
        /// <summary>
        /// Load all active table metadata from the database
        /// </summary>
        /// <returns>List of table metadata</returns>
        Task<List<TableMetadata>> GetTableMetadataAsync();

        /// <summary>
        /// Load all active column metadata from the database
        /// </summary>
        /// <returns>List of column metadata</returns>
        Task<List<ColumnMetadata>> GetColumnMetadataAsync();

        /// <summary>
        /// Load table metadata for a specific table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>Table metadata or null if not found</returns>
        Task<TableMetadata?> GetTableMetadataAsync(string tableName);

        /// <summary>
        /// Load column metadata for a specific table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>List of column metadata for the table</returns>
        Task<List<ColumnMetadata>> GetColumnMetadataAsync(string tableName);

        /// <summary>
        /// Load column metadata for a specific column
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columnName">Name of the column</param>
        /// <returns>Column metadata or null if not found</returns>
        Task<ColumnMetadata?> GetColumnMetadataAsync(string tableName, string columnName);

        /// <summary>
        /// Get tables ordered by importance score
        /// </summary>
        /// <param name="limit">Maximum number of tables to return</param>
        /// <returns>List of table metadata ordered by importance</returns>
        Task<List<TableMetadata>> GetTopTablesByImportanceAsync(int limit = 50);

        /// <summary>
        /// Get columns ordered by importance score
        /// </summary>
        /// <param name="limit">Maximum number of columns to return</param>
        /// <returns>List of column metadata ordered by importance</returns>
        Task<List<ColumnMetadata>> GetTopColumnsByImportanceAsync(int limit = 200);

        /// <summary>
        /// Search tables by business purpose or domain
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching table metadata</returns>
        Task<List<TableMetadata>> SearchTablesAsync(string searchTerm);

        /// <summary>
        /// Search columns by business meaning or context
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching column metadata</returns>
        Task<List<ColumnMetadata>> SearchColumnsAsync(string searchTerm);

        /// <summary>
        /// Test database connectivity
        /// </summary>
        /// <returns>True if database is accessible, false otherwise</returns>
        Task<bool> TestConnectionAsync();

        /// <summary>
        /// Get database statistics
        /// </summary>
        /// <returns>Statistics about the metadata in the database</returns>
        Task<DatabaseStatistics> GetDatabaseStatisticsAsync();
    }

    /// <summary>
    /// Database statistics model
    /// </summary>
    public class DatabaseStatistics
    {
        public int TotalTables { get; set; }
        public int ActiveTables { get; set; }
        public int TotalColumns { get; set; }
        public int ActiveColumns { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> TopDomains { get; set; } = new();
        public decimal AverageTableImportance { get; set; }
        public decimal AverageColumnImportance { get; set; }
    }
}
