using NL2SQL.Core.Data.Entities;

namespace NL2SQL.Core.Interfaces
{
    /// <summary>
    /// Service for retrieving and managing business rules
    /// </summary>
    public interface IBusinessRuleService
    {
        /// <summary>
        /// Gets business rules by category and intent type
        /// </summary>
        /// <param name="category">Rule category (e.g., "FINANCIAL", "DATE_HANDLING")</param>
        /// <param name="intentType">Intent type (e.g., "QUERY_GENERATION")</param>
        /// <returns>List of applicable business rules</returns>
        Task<List<BusinessRule>> GetRulesByCategoryAsync(string category, string? intentType = null);

        /// <summary>
        /// Gets all active business rules for an intent type
        /// </summary>
        /// <param name="intentType">Intent type</param>
        /// <returns>List of business rules grouped by category</returns>
        Task<Dictionary<string, List<BusinessRule>>> GetRulesByIntentAsync(string intentType);

        /// <summary>
        /// Formats business rules for prompt inclusion
        /// </summary>
        /// <param name="rules">List of business rules</param>
        /// <returns>Formatted rules text for prompt</returns>
        string FormatRulesForPrompt(List<BusinessRule> rules);

        /// <summary>
        /// Gets rules that match specific conditions
        /// </summary>
        /// <param name="userQuery">User's query to match against conditions</param>
        /// <param name="intentType">Intent type</param>
        /// <returns>List of matching business rules</returns>
        Task<List<BusinessRule>> GetMatchingRulesAsync(string userQuery, string intentType);
    }
}
