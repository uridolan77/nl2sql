namespace NL2SQL.Core.Interfaces
{
    /// <summary>
    /// Service for resolving placeholders in prompt templates
    /// </summary>
    public interface IPlaceholderResolverService
    {
        /// <summary>
        /// Resolves all placeholders in a template content
        /// </summary>
        /// <param name="templateContent">Template content with placeholders</param>
        /// <param name="userQuery">User's natural language query</param>
        /// <param name="intentType">Detected intent type</param>
        /// <param name="additionalContext">Additional context data</param>
        /// <returns>Template content with all placeholders resolved</returns>
        Task<string> ResolveAllPlaceholdersAsync(string templateContent, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null);

        /// <summary>
        /// Resolves a specific placeholder
        /// </summary>
        /// <param name="placeholderKey">Placeholder key (e.g., "BUSINESS_DOMAIN_CONTEXT")</param>
        /// <param name="userQuery">User's natural language query</param>
        /// <param name="intentType">Detected intent type</param>
        /// <param name="additionalContext">Additional context data</param>
        /// <returns>Resolved placeholder content</returns>
        Task<string> ResolvePlaceholderAsync(string placeholderKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null);

        /// <summary>
        /// Extracts all placeholder keys from template content
        /// </summary>
        /// <param name="templateContent">Template content</param>
        /// <returns>List of placeholder keys found</returns>
        List<string> ExtractPlaceholderKeys(string templateContent);

        /// <summary>
        /// Checks if a placeholder can be resolved
        /// </summary>
        /// <param name="placeholderKey">Placeholder key to check</param>
        /// <returns>True if placeholder can be resolved</returns>
        Task<bool> CanResolvePlaceholderAsync(string placeholderKey);
    }
}
