using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Data;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Models;
using System.Text.RegularExpressions;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Main service for building dynamic prompts from templates and database content
    /// </summary>
    public class PromptBuilderService : IPromptBuilderService
    {
        private readonly BusinessMetadataDbContext _context;
        private readonly IPlaceholderResolverService _placeholderResolver;
        private readonly ILogger<PromptBuilderService> _logger;

        public PromptBuilderService(
            BusinessMetadataDbContext context,
            IPlaceholderResolverService placeholderResolver,
            ILogger<PromptBuilderService> logger)
        {
            _context = context;
            _placeholderResolver = placeholderResolver;
            _logger = logger;
        }

        public async Task<string> BuildPromptAsync(string templateKey, string userQuery, string intentType, Dictionary<string, object>? additionalContext = null)
        {
            try
            {
                _logger.LogInformation("Building prompt for template: {TemplateKey}, intent: {IntentType}", templateKey, intentType);

                // Get the template
                var template = await _context.PromptTemplates
                    .Where(t => t.TemplateKey == templateKey && t.IsActive)
                    .OrderByDescending(t => t.Version)
                    .FirstOrDefaultAsync();

                if (template == null)
                {
                    throw new InvalidOperationException($"Template with key '{templateKey}' not found or inactive");
                }

                _logger.LogDebug("Found template: {TemplateName} v{Version}", template.Name, template.Version);

                // Update usage tracking
                template.UsageCount++;
                template.LastUsedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Resolve all placeholders
                var resolvedContent = await _placeholderResolver.ResolveAllPlaceholdersAsync(
                    template.Content, userQuery, intentType, additionalContext);

                _logger.LogInformation("Successfully built prompt for template: {TemplateKey}", templateKey);
                return resolvedContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building prompt for template: {TemplateKey}", templateKey);
                throw;
            }
        }

        public async Task<List<string>> GetAvailableTemplateKeysAsync()
        {
            try
            {
                var templateKeys = await _context.PromptTemplates
                    .Where(t => t.IsActive)
                    .Select(t => t.TemplateKey)
                    .Distinct()
                    .Where(k => k != null)
                    .ToListAsync();

                return templateKeys.Cast<string>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available template keys");
                throw;
            }
        }

        public async Task<PromptValidationResult> ValidateTemplateAsync(string templateKey)
        {
            try
            {
                var result = new PromptValidationResult();

                // Get the template
                var template = await _context.PromptTemplates
                    .Where(t => t.TemplateKey == templateKey && t.IsActive)
                    .OrderByDescending(t => t.Version)
                    .FirstOrDefaultAsync();

                if (template == null)
                {
                    result.IsValid = false;
                    result.MissingPlaceholders.Add($"Template '{templateKey}' not found");
                    return result;
                }

                // Extract placeholders from template
                var placeholders = _placeholderResolver.ExtractPlaceholderKeys(template.Content);

                // Check if each placeholder can be resolved
                var missingPlaceholders = new List<string>();
                foreach (var placeholder in placeholders)
                {
                    var canResolve = await _placeholderResolver.CanResolvePlaceholderAsync(placeholder);
                    if (!canResolve)
                    {
                        missingPlaceholders.Add(placeholder);
                    }
                }

                result.IsValid = missingPlaceholders.Count == 0;
                result.MissingPlaceholders = missingPlaceholders;

                if (!result.IsValid)
                {
                    result.Warnings.Add($"Template '{templateKey}' has {missingPlaceholders.Count} unresolvable placeholders");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating template: {TemplateKey}", templateKey);
                throw;
            }
        }
    }
}
