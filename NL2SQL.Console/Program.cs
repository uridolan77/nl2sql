using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Services;
using NL2SQL.Core.Repositories;
using NL2SQL.Core.Models;
using System;
using System.Threading.Tasks;

namespace NL2SQL.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("🎰 NL2SQL Gambling Industry Library - Basic Test");
            System.Console.WriteLine("=================================================");

            // Build host with dependency injection
            var host = CreateHostBuilder(args).Build();

            try
            {
                // Test the basic NL2SQL service
                await TestBasicNL2SQLService(host.Services);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Error: {ex.Message}");
                System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            System.Console.WriteLine("\nPress any key to exit...");
            System.Console.ReadKey();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    // Add basic services
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                        ?? "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

                    services.AddScoped<IMetadataRepository>(provider =>
                        new SqlServerMetadataRepository(connectionString, provider.GetRequiredService<ILogger<SqlServerMetadataRepository>>()));

                    services.AddScoped<INaturalLanguageAnalyzer, SimpleNaturalLanguageAnalyzer>();
                    services.AddScoped<ISchemaContextBuilder, SchemaContextBuilder>();
                    services.AddScoped<ISqlPromptBuilder, SqlPromptBuilder>();
                    services.AddScoped<ILLMService>(provider =>
                        new OpenAILLMService("mock-key", provider.GetRequiredService<ILogger<OpenAILLMService>>()));
                    services.AddScoped<INL2SqlService, NL2SqlService>();

                    // Add logging
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });
                });

        static async Task TestBasicNL2SQLService(IServiceProvider services)
        {
            System.Console.WriteLine("🔍 Testing Basic NL2SQL Service...\n");

            // Get services
            var metadataRepo = services.GetRequiredService<IMetadataRepository>();
            var nl2SqlService = services.GetRequiredService<INL2SqlService>();

            // Test 1: Load and display business metadata
            await TestMetadataLoading(metadataRepo);

            // Test 2: Test basic NL2SQL functionality
            await TestBasicNL2SQL(nl2SqlService);
        }

        static async Task TestMetadataLoading(IMetadataRepository metadataRepo)
        {
            System.Console.WriteLine("📊 Testing Metadata Loading...");
            
            try
            {
                // Load business tables
                var tables = await metadataRepo.GetTableInfoAsync();
                System.Console.WriteLine($"✅ Loaded {tables.Count} business tables");

                // Display top 5 tables by importance
                var topTables = tables.OrderByDescending(t => t.ImportanceScore).Take(5);
                System.Console.WriteLine("\n🏆 Top 5 Most Important Tables:");
                foreach (var table in topTables)
                {
                    System.Console.WriteLine($"  • {table.TableName} (Score: {table.ImportanceScore:F2}) - {table.BusinessPurpose}");
                }

                // Show some column details for the first table
                if (tables.Any())
                {
                    var firstTable = tables.First();
                    System.Console.WriteLine($"\n📊 Columns in {firstTable.TableName}:");
                    foreach (var column in firstTable.Columns.Take(5))
                    {
                        System.Console.WriteLine($"  • {column.ColumnName}: {column.BusinessMeaning}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Error loading metadata: {ex.Message}");
            }

            System.Console.WriteLine();
        }

        static async Task TestBasicNL2SQL(INL2SqlService nl2SqlService)
        {
            System.Console.WriteLine("🎲 Testing Basic NL2SQL Service...");

            var testQueries = new[]
            {
                "Show me the total deposits for last month",
                "What are the top 10 players by deposits?",
                "How many players registered yesterday?",
                "Show daily revenue for this week"
            };

            foreach (var query in testQueries)
            {
                try
                {
                    System.Console.WriteLine($"\n🔎 Processing: \"{query}\"");

                    var result = await nl2SqlService.GenerateSqlAsync(query);

                    if (result.Success)
                    {
                        System.Console.WriteLine($"  ✅ Generated SQL: {result.GeneratedSql}");
                        System.Console.WriteLine($"  📊 Confidence: {result.Confidence:F2}");
                        System.Console.WriteLine($"  ⏱️ Processing Time: {result.ProcessingTime.TotalMilliseconds:F0}ms");

                        if (result.Analysis != null)
                        {
                            System.Console.WriteLine($"  🎯 Intent: {result.Analysis.Intent}");
                            System.Console.WriteLine($"  🏷️ Entities: {string.Join(", ", result.Analysis.Entities)}");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine($"  ❌ Error: {result.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"  ❌ Exception: {ex.Message}");
                }
            }

            System.Console.WriteLine();
        }

    }
}
