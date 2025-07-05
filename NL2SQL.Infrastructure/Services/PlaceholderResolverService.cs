using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Data;
using NL2SQL.Core.Interfaces;
using System.Text.RegularExpressions;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Service for resolving placeholders in prompt templates
    /// </summary>
    public class PlaceholderResolverService : IPlaceholderResolverService
    {
        private readonly BusinessMetadataDbContext _context;
        private readonly IBusinessRuleService _businessRuleService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PlaceholderResolverService> _logger;

        // Regex pattern to match placeholders like {PLACEHOLDER_NAME}
        private static readonly Regex PlaceholderRegex = new(@"\{([A-Z_]+)\}", RegexOptions.Compiled);

        public PlaceholderResolverService(
            BusinessMetadataDbContext context,
            IBusinessRuleService businessRuleService,
            IMemoryCache cache,
            ILogger<PlaceholderResolverService> logger)
        {
            _context = context;
            _businessRuleService = businessRuleService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> ResolveAllPlaceholdersAsync(string templateContent, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null)
        {
            try
            {
                _logger.LogDebug("Resolving all placeholders for intent: {IntentType}", intentType);

                var resolvedContent = templateContent;
                var placeholders = ExtractPlaceholderKeys(templateContent);

                foreach (var placeholder in placeholders)
                {
                    var resolvedValue = await ResolvePlaceholderAsync(placeholder, userQuery, intentType, additionalContext);
                    resolvedContent = resolvedContent.Replace($"{{{placeholder}}}", resolvedValue);
                }

                _logger.LogDebug("Successfully resolved {Count} placeholders", placeholders.Count);
                return resolvedContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving placeholders");
                throw;
            }
        }

        public async Task<string> ResolvePlaceholderAsync(string placeholderKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null)
        {
            try
            {
                _logger.LogDebug("Resolving placeholder: {PlaceholderKey}", placeholderKey);

                // Check cache first
                var cacheKey = $"placeholder_{placeholderKey}_{intentType}";
                if (_cache.TryGetValue(cacheKey, out string? cachedValue) && !string.IsNullOrEmpty(cachedValue))
                {
                    return cachedValue;
                }

                // Get placeholder configuration
                var placeholderConfig = await _context.PromptPlaceholders
                    .Where(p => p.PlaceholderKey == placeholderKey && p.IsActive)
                    .FirstOrDefaultAsync();

                string resolvedValue;

                if (placeholderConfig != null)
                {
                    resolvedValue = await ResolveConfiguredPlaceholderAsync(placeholderConfig, userQuery, intentType, additionalContext);
                    
                    // Cache the result
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(placeholderConfig.CacheMinutes)
                    };
                    _cache.Set(cacheKey, resolvedValue, cacheOptions);
                }
                else
                {
                    // Handle built-in placeholders
                    resolvedValue = await ResolveBuiltInPlaceholderAsync(placeholderKey, userQuery, intentType, additionalContext);
                }

                _logger.LogDebug("Resolved placeholder {PlaceholderKey} to {Length} characters", placeholderKey, resolvedValue.Length);
                return resolvedValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving placeholder: {PlaceholderKey}", placeholderKey);
                return $"[ERROR: Could not resolve {placeholderKey}]";
            }
        }

        public List<string> ExtractPlaceholderKeys(string templateContent)
        {
            var matches = PlaceholderRegex.Matches(templateContent);
            return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
        }

        public async Task<bool> CanResolvePlaceholderAsync(string placeholderKey)
        {
            try
            {
                // Check if it's a configured placeholder
                var hasConfig = await _context.PromptPlaceholders
                    .AnyAsync(p => p.PlaceholderKey == placeholderKey && p.IsActive);

                if (hasConfig)
                    return true;

                // Check if it's a built-in placeholder
                return IsBuiltInPlaceholder(placeholderKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if placeholder can be resolved: {PlaceholderKey}", placeholderKey);
                return false;
            }
        }

        private async Task<string> ResolveConfiguredPlaceholderAsync(
            NL2SQL.Core.Data.Entities.PromptPlaceholder config, 
            string userQuery, 
            string intentType, 
            Dictionary<string, object>? additionalContext)
        {
            switch (config.DataSource.ToUpper())
            {
                case "STATIC":
                    return config.StaticContent ?? string.Empty;

                case "TABLE":
                case "FUNCTION":
                    if (!string.IsNullOrEmpty(config.SourceQuery))
                    {
                        // Execute the SQL query to get content
                        // Note: This would need proper SQL execution with parameters
                        // For now, return placeholder
                        return $"[DYNAMIC CONTENT FROM {config.DataSource}]";
                    }
                    break;

                case "COMPUTED":
                    return await ComputePlaceholderValueAsync(config.PlaceholderKey, userQuery, intentType, additionalContext);
            }

            return string.Empty;
        }

        private async Task<string> ResolveBuiltInPlaceholderAsync(string placeholderKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext)
        {
            return placeholderKey switch
            {
                "USER_QUESTION" => userQuery,
                "CONTEXT" => additionalContext?.GetValueOrDefault("context")?.ToString() ?? string.Empty,
                "DATABASE_NAME" => "DailyActionsDB", // Could be configurable
                "DOMAIN_RULES" => await GetDomainRulesAsync(intentType),
                "FINANCIAL_RULES" => await GetFinancialRulesAsync(),
                "DATE_HANDLING_RULES" => await GetDateHandlingRulesAsync(),
                "FRAUD_PREVENTION_RULES" => await GetFraudPreventionRulesAsync(),
                "OUTPUT_FORMATTING_RULES" => await GetOutputFormattingRulesAsync(),
                "FRAUD_FILTERS" => await GetFraudFiltersAsync(),
                "VALIDATION_CHECKLIST" => await GetValidationChecklistAsync(),
                _ => $"[PLACEHOLDER {placeholderKey} NOT IMPLEMENTED]"
            };
        }

        private async Task<string> ComputePlaceholderValueAsync(string placeholderKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext)
        {
            // Implement computed placeholder logic based on the key
            return placeholderKey switch
            {
                "SCHEMA_DEFINITION" => await BuildSchemaDefinitionAsync(userQuery),
                "EXAMPLES" => await BuildExamplesAsync(intentType, userQuery),
                "BUSINESS_DOMAIN_CONTEXT" => await BuildBusinessDomainContextAsync(),
                "GAMING_KPI_DEFINITIONS" => await BuildGamingKPIDefinitionsAsync(),
                "COMPLIANCE_CONTEXT" => await BuildComplianceContextAsync(),
                "REGULATORY_CONSTRAINTS" => await BuildRegulatoryConstraintsAsync(),
                _ => string.Empty
            };
        }

        private bool IsBuiltInPlaceholder(string placeholderKey)
        {
            var builtInPlaceholders = new[]
            {
                "USER_QUESTION", "CONTEXT", "DATABASE_NAME", "DOMAIN_RULES",
                "FINANCIAL_RULES", "DATE_HANDLING_RULES", "FRAUD_PREVENTION_RULES",
                "OUTPUT_FORMATTING_RULES", "FRAUD_FILTERS", "VALIDATION_CHECKLIST",
                "SCHEMA_DEFINITION", "EXAMPLES", "BUSINESS_DOMAIN_CONTEXT",
                "GAMING_KPI_DEFINITIONS", "COMPLIANCE_CONTEXT", "REGULATORY_CONSTRAINTS"
            };

            return builtInPlaceholders.Contains(placeholderKey);
        }

        // Placeholder implementation methods - these would be fully implemented
        private async Task<string> GetDomainRulesAsync(string intentType)
        {
            var rules = await _businessRuleService.GetRulesByCategoryAsync("DOMAIN", intentType);
            return _businessRuleService.FormatRulesForPrompt(rules);
        }

        private async Task<string> GetFinancialRulesAsync()
        {
            var rules = await _businessRuleService.GetRulesByCategoryAsync("FINANCIAL");
            return _businessRuleService.FormatRulesForPrompt(rules);
        }

        private async Task<string> GetDateHandlingRulesAsync()
        {
            var rules = await _businessRuleService.GetRulesByCategoryAsync("DATE_HANDLING");
            return _businessRuleService.FormatRulesForPrompt(rules);
        }

        private async Task<string> GetFraudPreventionRulesAsync()
        {
            var rules = await _businessRuleService.GetRulesByCategoryAsync("FRAUD_PREVENTION");
            return _businessRuleService.FormatRulesForPrompt(rules);
        }

        private async Task<string> GetOutputFormattingRulesAsync()
        {
            var rules = await _businessRuleService.GetRulesByCategoryAsync("OUTPUT_FORMATTING");
            return _businessRuleService.FormatRulesForPrompt(rules);
        }

        private async Task<string> GetFraudFiltersAsync()
        {
            return "IsSuspicious = 0 AND IsTestAccount = 0";
        }

        private async Task<string> GetValidationChecklistAsync()
        {
            return @"Before finalizing the query, verify:
- [ ] Uses correct column names from schema
- [ ] Includes mandatory safety declarations
- [ ] All tables have WITH (NOLOCK) hints
- [ ] Monetary values are properly rounded and aliased
- [ ] Date logic follows standardized patterns
- [ ] Includes fraud prevention filters where applicable
- [ ] Query structure follows the template format";
        }

        private async Task<string> BuildSchemaDefinitionAsync(string userQuery)
        {
            // This would implement intelligent schema selection based on the query
            return "[DYNAMIC SCHEMA BASED ON QUERY]";
        }

        private async Task<string> BuildExamplesAsync(string intentType, string userQuery)
        {
            // This would select relevant examples from ExampleQueries table
            return "[RELEVANT EXAMPLES FOR INTENT TYPE]";
        }

        private async Task<string> BuildBusinessDomainContextAsync()
        {
            // This would build context from BusinessDomain table
            return "[BUSINESS DOMAIN CONTEXT FROM DATABASE]";
        }

        private async Task<string> BuildGamingKPIDefinitionsAsync()
        {
            // This would build KPI definitions from KPIDefinitions table
            return "[GAMING KPI DEFINITIONS FROM DATABASE]";
        }

        private async Task<string> BuildComplianceContextAsync()
        {
            // This would build compliance context from ComplianceRules table
            return "[COMPLIANCE CONTEXT FROM DATABASE]";
        }

        private async Task<string> BuildRegulatoryConstraintsAsync()
        {
            // This would build regulatory constraints from ComplianceRules table
            return "[REGULATORY CONSTRAINTS FROM DATABASE]";
        }
    }
}
