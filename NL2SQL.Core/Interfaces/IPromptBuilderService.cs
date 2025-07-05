using NL2SQL.Core.Models;

namespace NL2SQL.Core.Interfaces
{
    /// <summary>
    /// Main service for building dynamic prompts from templates and database content
    /// </summary>
    public interface IPromptBuilderService
    {
        /// <summary>
        /// Builds a complete prompt from template and user query
        /// </summary>
        /// <param name="templateKey">Template key to use (e.g., "basicquerygeneration")</param>
        /// <param name="userQuery">User's natural language query</param>
        /// <param name="intentType">Detected intent type (e.g., "QUERY_GENERATION")</param>
        /// <param name="additionalContext">Additional context information</param>
        /// <returns>Complete prompt ready for LLM</returns>
        Task<string> BuildPromptAsync(string templateKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null);

        /// <summary>
        /// Gets available template keys
        /// </summary>
        /// <returns>List of available template keys</returns>
        Task<List<string>> GetAvailableTemplateKeysAsync();

        /// <summary>
        /// Validates that all placeholders in a template can be resolved
        /// </summary>
        /// <param name="templateKey">Template key to validate</param>
        /// <returns>Validation result with any missing placeholders</returns>
        Task<PromptValidationResult> ValidateTemplateAsync(string templateKey);
    }

    /// <summary>
    /// Result of template validation
    /// </summary>
    public class PromptValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> MissingPlaceholders { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
