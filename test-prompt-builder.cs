// Simple test to demonstrate the Prompt Builder module
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using NL2SQL.Core.Data;
using NL2SQL.Core.Interfaces;
using NL2SQL.Infrastructure.Configuration;

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

                // Add NL2SQL Infrastructure services (includes Prompt Builder)
                services.AddNL2SQLInfrastructure();
            })
            .Build();

        try
        {
            using var scope = host.Services.CreateScope();
            var promptBuilder = scope.ServiceProvider.GetRequiredService<IPromptBuilderService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            Console.WriteLine("✅ Prompt Builder service resolved successfully");

            // Test 1: Get available template keys
            Console.WriteLine("\n📋 Testing: Get Available Template Keys");
            var templateKeys = await promptBuilder.GetAvailableTemplateKeysAsync();
            Console.WriteLine($"   Found {templateKeys.Count} template keys:");
            foreach (var key in templateKeys)
            {
                Console.WriteLine($"   - {key}");
            }

            // Test 2: Validate a template
            if (templateKeys.Contains("basicquerygeneration"))
            {
                Console.WriteLine("\n🔍 Testing: Template Validation");
                var validation = await promptBuilder.ValidateTemplateAsync("basicquerygeneration");
                Console.WriteLine($"   Template is valid: {validation.IsValid}");
                if (!validation.IsValid)
                {
                    Console.WriteLine($"   Missing placeholders: {string.Join(", ", validation.MissingPlaceholders)}");
                }
                if (validation.Warnings.Any())
                {
                    Console.WriteLine($"   Warnings: {string.Join(", ", validation.Warnings)}");
                }
            }

            // Test 3: Build a simple prompt
            Console.WriteLine("\n🏗️ Testing: Build Prompt");
            try
            {
                var userQuery = "Show me total deposits for yesterday";
                var intentType = "QUERY_GENERATION";
                
                var prompt = await promptBuilder.BuildPromptAsync("basicquerygeneration", userQuery, intentType);
                
                Console.WriteLine($"   ✅ Prompt built successfully!");
                Console.WriteLine($"   📏 Prompt length: {prompt.Length} characters");
                Console.WriteLine($"   🔍 First 200 characters:");
                Console.WriteLine($"   {prompt.Substring(0, Math.Min(200, prompt.Length))}...");
                
                // Check if placeholders were resolved
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
                Console.WriteLine($"   ❌ Error building prompt: {ex.Message}");
            }

            Console.WriteLine("\n🎯 Prompt Builder Module Test Complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
