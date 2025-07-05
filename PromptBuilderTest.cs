using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using NL2SQL.Core.Data;
using NL2SQL.Core.Interfaces;
using NL2SQL.Infrastructure.Configuration;

namespace TestPromptBuilder;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔧 Testing Prompt Builder Module");
        Console.WriteLine("================================");

        // Build the host with all services
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add Entity Framework DbContext
                services.AddDbContext<BusinessMetadataDbContext>(options =>
                    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"));

                // Add memory cache
                services.AddMemoryCache();

                // Add NL2SQL Infrastructure services (includes Prompt Builder)
                services.AddNL2SQLInfrastructure();
            })
            .Build();

        try
        {
            using var scope = host.Services.CreateScope();
            var promptBuilder = scope.ServiceProvider.GetRequiredService<IPromptBuilderService>();
            var businessRuleService = scope.ServiceProvider.GetRequiredService<IBusinessRuleService>();
            var placeholderResolver = scope.ServiceProvider.GetRequiredService<IPlaceholderResolverService>();

            Console.WriteLine("✅ All services resolved successfully");

            // Test 1: Get available template keys
            Console.WriteLine("\n📋 Test 1: Get Available Template Keys");
            try
            {
                var templateKeys = await promptBuilder.GetAvailableTemplateKeysAsync();
                Console.WriteLine($"   Found {templateKeys.Count} template keys:");
                foreach (var key in templateKeys)
                {
                    Console.WriteLine($"   - {key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }

            // Test 2: Test business rule service
            Console.WriteLine("\n🔧 Test 2: Business Rule Service");
            try
            {
                var financialRules = await businessRuleService.GetRulesByCategoryAsync("FINANCIAL", "QUERY_GENERATION");
                Console.WriteLine($"   Found {financialRules.Count} financial rules:");
                foreach (var rule in financialRules)
                {
                    Console.WriteLine($"   - {rule.RuleName}: {rule.RuleContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }

            // Test 3: Test placeholder extraction
            Console.WriteLine("\n🔍 Test 3: Placeholder Extraction");
            try
            {
                var sampleTemplate = "Hello {USER_QUESTION}, using {DATABASE_NAME} with {SCHEMA_DEFINITION}";
                var placeholders = placeholderResolver.ExtractPlaceholderKeys(sampleTemplate);
                Console.WriteLine($"   Found {placeholders.Count} placeholders:");
                foreach (var placeholder in placeholders)
                {
                    Console.WriteLine($"   - {placeholder}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }

            // Test 4: Test individual placeholder resolution
            Console.WriteLine("\n🔧 Test 4: Individual Placeholder Resolution");
            try
            {
                var databaseName = await placeholderResolver.ResolvePlaceholderAsync("DATABASE_NAME", "test query", "QUERY_GENERATION");
                Console.WriteLine($"   DATABASE_NAME resolved to: '{databaseName}'");

                var userQuestion = await placeholderResolver.ResolvePlaceholderAsync("USER_QUESTION", "Show me deposits", "QUERY_GENERATION");
                Console.WriteLine($"   USER_QUESTION resolved to: '{userQuestion}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }

            // Test 5: Template validation
            Console.WriteLine("\n🔍 Test 5: Template Validation");
            try
            {
                var validation = await promptBuilder.ValidateTemplateAsync("basicquerygeneration");
                Console.WriteLine($"   Template is valid: {validation.IsValid}");
                if (!validation.IsValid)
                {
                    Console.WriteLine($"   Missing placeholders ({validation.MissingPlaceholders.Count}):");
                    foreach (var missing in validation.MissingPlaceholders)
                    {
                        Console.WriteLine($"     - {missing}");
                    }
                }
                if (validation.Warnings.Any())
                {
                    Console.WriteLine($"   Warnings:");
                    foreach (var warning in validation.Warnings)
                    {
                        Console.WriteLine($"     - {warning}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
            }

            // Test 6: Build a simple prompt (this will show placeholder resolution in action)
            Console.WriteLine("\n🏗️ Test 6: Build Complete Prompt");
            try
            {
                var userQuery = "Show me total deposits for yesterday";
                var intentType = "QUERY_GENERATION";
                
                var prompt = await promptBuilder.BuildPromptAsync("basicquerygeneration", userQuery, intentType);
                
                Console.WriteLine($"   ✅ Prompt built successfully!");
                Console.WriteLine($"   📏 Prompt length: {prompt.Length} characters");
                
                // Show first 500 characters
                var preview = prompt.Length > 500 ? prompt.Substring(0, 500) + "..." : prompt;
                Console.WriteLine($"   🔍 Preview:\n{preview}");
                
                // Check for unresolved placeholders
                var unresolvedPlaceholders = System.Text.RegularExpressions.Regex.Matches(prompt, @"\{([A-Z_]+)\}");
                if (unresolvedPlaceholders.Count > 0)
                {
                    Console.WriteLine($"   ⚠️ Found {unresolvedPlaceholders.Count} unresolved placeholders:");
                    foreach (System.Text.RegularExpressions.Match match in unresolvedPlaceholders)
                    {
                        Console.WriteLine($"     - {match.Value}");
                    }
                }
                else
                {
                    Console.WriteLine($"   ✅ All placeholders resolved successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\n🎯 Prompt Builder Module Test Complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fatal Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
