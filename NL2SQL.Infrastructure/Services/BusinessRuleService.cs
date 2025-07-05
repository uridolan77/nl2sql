using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Data;
using NL2SQL.Core.Data.Entities;
using NL2SQL.Core.Interfaces;
using System.Text;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Service for retrieving and managing business rules
    /// </summary>
    public class BusinessRuleService : IBusinessRuleService
    {
        private readonly BusinessMetadataDbContext _context;
        private readonly ILogger<BusinessRuleService> _logger;

        public BusinessRuleService(
            BusinessMetadataDbContext context,
            ILogger<BusinessRuleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BusinessRule>> GetRulesByCategoryAsync(string category, string? intentType = null)
        {
            try
            {
                _logger.LogDebug("Getting business rules for category: {Category}, intent: {IntentType}", category, intentType);

                var query = _context.BusinessRules
                    .Where(r => r.IsActive && r.RuleCategory == category);

                if (!string.IsNullOrEmpty(intentType))
                {
                    query = query.Where(r => r.IntentType == null || r.IntentType == intentType);
                }

                var rules = await query
                    .OrderBy(r => r.Priority)
                    .ThenBy(r => r.RuleName)
                    .ToListAsync();

                _logger.LogDebug("Found {Count} business rules for category: {Category}", rules.Count, category);
                return rules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rules for category: {Category}", category);
                throw;
            }
        }

        public async Task<Dictionary<string, List<BusinessRule>>> GetRulesByIntentAsync(string intentType)
        {
            try
            {
                _logger.LogDebug("Getting all business rules for intent: {IntentType}", intentType);

                var rules = await _context.BusinessRules
                    .Where(r => r.IsActive && (r.IntentType == null || r.IntentType == intentType))
                    .OrderBy(r => r.RuleCategory)
                    .ThenBy(r => r.Priority)
                    .ThenBy(r => r.RuleName)
                    .ToListAsync();

                var groupedRules = rules
                    .GroupBy(r => r.RuleCategory)
                    .ToDictionary(g => g.Key, g => g.ToList());

                _logger.LogDebug("Found {Count} rule categories for intent: {IntentType}", groupedRules.Count, intentType);
                return groupedRules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rules for intent: {IntentType}", intentType);
                throw;
            }
        }

        public string FormatRulesForPrompt(List<BusinessRule> rules)
        {
            if (!rules.Any())
                return string.Empty;

            var sb = new StringBuilder();

            // Group rules by category for better organization
            var groupedRules = rules.GroupBy(r => r.RuleCategory);

            foreach (var categoryGroup in groupedRules)
            {
                if (sb.Length > 0)
                    sb.AppendLine();

                sb.AppendLine($"## {categoryGroup.Key} RULES:");

                foreach (var rule in categoryGroup.OrderBy(r => r.Priority))
                {
                    sb.AppendLine($"- **{rule.RuleName}**: {rule.RuleContent}");
                    
                    if (!string.IsNullOrEmpty(rule.Condition))
                    {
                        sb.AppendLine($"  - Condition: {rule.Condition}");
                    }
                    
                    if (!string.IsNullOrEmpty(rule.Action))
                    {
                        sb.AppendLine($"  - Action: {rule.Action}");
                    }
                }
            }

            return sb.ToString();
        }

        public async Task<List<BusinessRule>> GetMatchingRulesAsync(string userQuery, string intentType)
        {
            try
            {
                _logger.LogDebug("Getting matching business rules for query: {Query}, intent: {IntentType}", userQuery, intentType);

                var allRules = await _context.BusinessRules
                    .Where(r => r.IsActive && (r.IntentType == null || r.IntentType == intentType))
                    .ToListAsync();

                var matchingRules = new List<BusinessRule>();
                var queryLower = userQuery.ToLowerInvariant();

                foreach (var rule in allRules)
                {
                    // Check if rule condition matches the user query
                    if (string.IsNullOrEmpty(rule.Condition))
                    {
                        // No condition means rule always applies
                        matchingRules.Add(rule);
                    }
                    else
                    {
                        // Simple keyword matching for conditions
                        var conditionKeywords = rule.Condition.ToLowerInvariant()
                            .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(k => k.Trim().Trim('"', '\''))
                            .Where(k => !string.IsNullOrEmpty(k));

                        foreach (var keyword in conditionKeywords)
                        {
                            if (queryLower.Contains(keyword))
                            {
                                matchingRules.Add(rule);
                                break; // Only add once per rule
                            }
                        }
                    }
                }

                // Remove duplicates and sort by priority
                var uniqueRules = matchingRules
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .OrderBy(r => r.Priority)
                    .ThenBy(r => r.RuleName)
                    .ToList();

                _logger.LogDebug("Found {Count} matching business rules", uniqueRules.Count);
                return uniqueRules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting matching business rules");
                throw;
            }
        }
    }
}
