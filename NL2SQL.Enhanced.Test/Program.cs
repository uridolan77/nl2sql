using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NL2SQL.Core.Configuration;
using NL2SQL.Core.Data;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Advanced;
using NL2SQL.Infrastructure.Configuration;
using NL2SQL.Infrastructure.Services;

namespace NL2SQL.Enhanced.Test
{
    class Program
    {
        private static readonly string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

        static async Task Main(string[] args)
        {
            Console.WriteLine("🎰 NL2SQL Enhanced Test - Real Database Schema Only");
            Console.WriteLine("====================================================");

            // Build host with dependency injection
            var host = CreateHostBuilder(args).Build();

            try
            {
                // Test database connectivity first - REQUIRED
                if (!await TestDatabaseConnectivityAsync(host.Services))
                {
                    Console.WriteLine("❌ Database connection failed. Cannot proceed without real database schema.");
                    Console.WriteLine("   Please ensure the database is accessible and contains BusinessTableInfo/BusinessColumnInfo tables.");
                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                    return;
                }

                // Initialize services with REAL database data only
                await InitializeServicesAsync(host.Services);

                // Test advanced NLP pipeline with real schema
                await TestAdvancedNLPPipeline(host.Services);

                // Test semantic search with real schema
                await TestSemanticSearch(host.Services);

                // Test gambling domain knowledge
                await TestGamblingDomainKnowledge(host.Services);

                // Test complete enhanced system with real schema
                await TestCompleteEnhancedSystem(host.Services);

                Console.WriteLine("\n🎉 All tests completed successfully with REAL database schema!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Add logging
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });

                    // Add HttpClient for vector embedding service
                    services.AddHttpClient();

                    // Add Entity Framework DbContext for business metadata
                    services.AddDbContext<BusinessMetadataDbContext>(options =>
                        options.UseSqlServer(ConnectionString));

                    // Add NL2SQL Core services
                    services.AddNL2SQL(context.Configuration);

                    // Add NL2SQL Infrastructure services
                    services.AddNL2SQLInfrastructure();

                    // Configure vector embedding options
                    services.Configure<VectorEmbeddingOptions>(options =>
                    {
                        options.EmbeddingDimension = 384;
                        options.CacheExpirationHours = 24;
                        // options.ApiEndpoint = "https://api.openai.com/v1/embeddings"; // Optional
                        // options.ApiKey = "your-api-key"; // Optional
                    });

                    // Register advanced NLP services
                    services.AddScoped<IGamblingDomainKnowledge, GamblingDomainKnowledge>();
                    services.AddScoped<IVectorEmbeddingService, VectorEmbeddingService>();
                    services.AddScoped<IAdvancedEntityExtractor, AdvancedEntityExtractor>();
                    services.AddScoped<ISemanticSearchService, SemanticSearchService>();
                    services.AddScoped<IAdvancedNLPPipeline, AdvancedNLPPipeline>();
                });

        static async Task<bool> TestDatabaseConnectivityAsync(IServiceProvider services)
        {
            Console.WriteLine("🔌 Testing database connectivity...");

            try
            {
                var repository = services.GetRequiredService<IBusinessMetadataRepository>();
                var isConnected = await repository.TestConnectionAsync();

                if (isConnected)
                {
                    var stats = await repository.GetDatabaseStatisticsAsync();
                    Console.WriteLine($"✅ Database connection successful!");
                    Console.WriteLine($"   📊 Found {stats.ActiveTables} active tables and {stats.ActiveColumns} active columns");
                    Console.WriteLine($"   🏷️ Top domains: {string.Join(", ", stats.TopDomains.Take(3))}");
                    return true;
                }
                else
                {
                    Console.WriteLine("❌ Database connection failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connectivity test failed: {ex.Message}");
                return false;
            }
        }

        static async Task InitializeServicesAsync(IServiceProvider services)
        {
            Console.WriteLine("🔧 Initializing enhanced NLP services with REAL database metadata...");

            try
            {
                var repository = services.GetRequiredService<IBusinessMetadataRepository>();
                var semanticSearch = services.GetRequiredService<ISemanticSearchService>();

                Console.WriteLine("📊 Loading table metadata from database...");
                var tables = await repository.GetTopTablesByImportanceAsync(50);
                Console.WriteLine($"✅ Loaded {tables.Count} tables from BusinessTableInfo");

                Console.WriteLine("📋 Loading column metadata from database...");
                var columns = await repository.GetTopColumnsByImportanceAsync(200);
                Console.WriteLine($"✅ Loaded {columns.Count} columns from BusinessColumnInfo");

                if (tables.Count == 0 || columns.Count == 0)
                {
                    throw new InvalidOperationException("No table or column metadata found in database. Please ensure BusinessTableInfo and BusinessColumnInfo tables contain data.");
                }

                await semanticSearch.InitializeAsync(tables, columns);

                Console.WriteLine($"✅ Initialized semantic search with {tables.Count} REAL tables and {columns.Count} REAL columns");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing services: {ex.Message}");
                throw;
            }
        }

        static async Task TestAdvancedNLPPipeline(IServiceProvider services)
        {
            Console.WriteLine("\n🧠 Testing Advanced NLP Pipeline...");

            var pipeline = services.GetRequiredService<IAdvancedNLPPipeline>();

            var testQueries = new[]
            {
                "What was the total GGR for VIP players last month?",
                "Show me the top 10 slot games by revenue this year",
                "How many new players registered yesterday compared to last week?",
                "Calculate the average deposit amount for high rollers in Q1",
                "What's the trend in sports betting revenue over the past 6 months?"
            };

            foreach (var query in testQueries)
            {
                Console.WriteLine($"\n🔎 Processing: \"{query}\"");

                try
                {
                    var result = await pipeline.ProcessQueryAsync(query);

                    Console.WriteLine($"  📊 Overall Confidence: {result.OverallConfidence:F3}");
                    Console.WriteLine($"  ⏱️ Processing Time: {result.ProcessingTime.TotalMilliseconds:F0}ms");
                    Console.WriteLine($"  🎯 Primary Intent: {result.IntentAnalysis.PrimaryIntent} ({result.IntentAnalysis.Confidence:F3})");
                    
                    // Show entity extraction results
                    var totalEntities = result.EntityExtraction.GamblingEntities.Count +
                                       result.EntityExtraction.TemporalEntities.Count +
                                       result.EntityExtraction.FinancialEntities.Count +
                                       result.EntityExtraction.PlayerEntities.Count +
                                       result.EntityExtraction.GameEntities.Count +
                                       result.EntityExtraction.MetricEntities.Count;

                    Console.WriteLine($"  🏷️ Entities Extracted: {totalEntities}");

                    if (result.EntityExtraction.GamblingEntities.Any())
                    {
                        Console.WriteLine($"    💰 Gambling: {string.Join(", ", result.EntityExtraction.GamblingEntities.Select(e => e.Text))}");
                    }

                    if (result.EntityExtraction.TemporalEntities.Any())
                    {
                        Console.WriteLine($"    📅 Temporal: {string.Join(", ", result.EntityExtraction.TemporalEntities.Select(e => e.Text))}");
                    }

                    if (result.EntityExtraction.PlayerEntities.Any())
                    {
                        Console.WriteLine($"    👤 Player: {string.Join(", ", result.EntityExtraction.PlayerEntities.Select(e => e.Text))}");
                    }

                    // Show table recommendations
                    if (result.TableRecommendations.Any())
                    {
                        var topTable = result.TableRecommendations.First();
                        Console.WriteLine($"  📊 Top Table: {topTable.Item.TableName} (Score: {topTable.SimilarityScore:F3})");
                    }

                    // Show column recommendations
                    if (result.ColumnRecommendations.Any())
                    {
                        var topColumns = result.ColumnRecommendations.Take(3);
                        Console.WriteLine($"  📋 Top Columns: {string.Join(", ", topColumns.Select(c => c.Item.ColumnName))}");
                    }

                    Console.WriteLine($"  🔄 Processing Steps: {string.Join(" → ", result.ProcessingSteps)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ❌ Error: {ex.Message}");
                }
            }
        }

        static async Task TestSemanticSearch(IServiceProvider services)
        {
            Console.WriteLine("\n🔍 Testing Semantic Search...");

            var semanticSearch = services.GetRequiredService<ISemanticSearchService>();

            var searchQueries = new[]
            {
                "player revenue data",
                "gambling financial transactions",
                "game performance metrics",
                "customer deposit information",
                "sports betting statistics"
            };

            foreach (var query in searchQueries)
            {
                Console.WriteLine($"\n🔎 Searching: \"{query}\"");

                try
                {
                    var tableMatches = await semanticSearch.FindSimilarTablesAsync(query, 3);
                    var columnMatches = await semanticSearch.FindSimilarColumnsAsync(query, 5);

                    Console.WriteLine("  📊 Similar Tables:");
                    foreach (var match in tableMatches)
                    {
                        Console.WriteLine($"    • {match.Item.TableName} (Score: {match.SimilarityScore:F3}) - {match.MatchReason}");
                    }

                    Console.WriteLine("  📋 Similar Columns:");
                    foreach (var match in columnMatches)
                    {
                        Console.WriteLine($"    • {match.Item.TableName}.{match.Item.ColumnName} (Score: {match.SimilarityScore:F3})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ❌ Error: {ex.Message}");
                }
            }
        }

        static async Task TestGamblingDomainKnowledge(IServiceProvider services)
        {
            Console.WriteLine("\n🎲 Testing Gambling Domain Knowledge...");

            var domainKnowledge = services.GetRequiredService<IGamblingDomainKnowledge>();

            try
            {
                // Test gambling terms
                var terms = await domainKnowledge.GetGamblingTermsAsync();
                Console.WriteLine($"✅ Loaded {terms.Count} gambling terms");

                foreach (var term in terms.Take(3))
                {
                    Console.WriteLine($"  💰 {term.Key}: {term.Value.Definition.Substring(0, Math.Min(80, term.Value.Definition.Length))}...");
                }

                // Test metric calculations
                var calculations = await domainKnowledge.GetMetricCalculationsAsync();
                Console.WriteLine($"\n✅ Loaded {calculations.Count} metric calculations");

                foreach (var calc in calculations.Take(2))
                {
                    Console.WriteLine($"  📊 {calc.Key}: {calc.Value.Formula}");
                }

                // Test term mapping
                var testTerms = new List<string> { "ggr", "player", "deposit", "revenue" };
                var mappings = await domainKnowledge.MapTermsToColumnsAsync(testTerms);
                Console.WriteLine($"\n✅ Generated {mappings.Count} term-to-column mappings");

                foreach (var mapping in mappings.Take(5))
                {
                    Console.WriteLine($"  🔗 {mapping.Term} → {mapping.TableName}.{mapping.ColumnName} (Score: {mapping.MatchScore:F2})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        static async Task TestCompleteEnhancedSystem(IServiceProvider services)
        {
            Console.WriteLine("\n🚀 Testing Complete Enhanced System...");

            var pipeline = services.GetRequiredService<IAdvancedNLPPipeline>();
            var semanticSearch = services.GetRequiredService<ISemanticSearchService>();
            var entityExtractor = services.GetRequiredService<IAdvancedEntityExtractor>();

            var complexQuery = "Show me the GGR trend for VIP players who deposited more than £1000 last quarter, broken down by game category";

            Console.WriteLine($"🔎 Complex Query: \"{complexQuery}\"");

            try
            {
                // Step 1: Extract entities
                var entities = await entityExtractor.ExtractEntitiesAsync(complexQuery);
                Console.WriteLine($"  🏷️ Extracted {entities.GamblingEntities.Count + entities.PlayerEntities.Count + entities.FinancialEntities.Count + entities.TemporalEntities.Count} entities");

                // Step 2: Get table-column recommendations
                var recommendations = await semanticSearch.RecommendTableColumnsAsync(complexQuery, entities);
                Console.WriteLine($"  📊 Generated {recommendations.Count} table-column recommendations");

                foreach (var rec in recommendations.Take(3))
                {
                    Console.WriteLine($"    • {rec.TableName}: {string.Join(", ", rec.RecommendedColumns)} (Score: {rec.OverallScore:F3})");
                    Console.WriteLine($"      Reasoning: {rec.Reasoning}");
                }

                // Step 3: Process through complete pipeline
                var result = await pipeline.ProcessQueryAsync(complexQuery);
                Console.WriteLine($"  🧠 Pipeline Confidence: {result.OverallConfidence:F3}");
                Console.WriteLine($"  ⏱️ Total Processing Time: {result.ProcessingTime.TotalMilliseconds:F0}ms");

                // Generate suggested SQL structure
                Console.WriteLine("\n  💡 Suggested SQL Structure:");
                Console.WriteLine("    SELECT");
                Console.WriteLine("      GameCategory,");
                Console.WriteLine("      DATE_PART,");
                Console.WriteLine("      SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive) as GGR");
                Console.WriteLine("    FROM tbl_Daily_actions da");
                Console.WriteLine("    JOIN tbl_Daily_actions_players dap ON da.PlayerID = dap.PlayerID");
                Console.WriteLine("    JOIN Games g ON da.GameID = g.GameID");
                Console.WriteLine("    WHERE");
                Console.WriteLine("      dap.PlayerSegment = 'VIP'");
                Console.WriteLine("      AND dap.TotalDeposits > 1000");
                Console.WriteLine("      AND da.Date >= DATEADD(quarter, -1, GETDATE())");
                Console.WriteLine("    GROUP BY GameCategory, DATE_PART");
                Console.WriteLine("    ORDER BY DATE_PART, GameCategory");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Error: {ex.Message}");
            }
        }

        // All mock data methods removed - using real database only





        // All mock data methods removed - using Entity Framework repository only


        // All helper methods removed - using Entity Framework repository methods only
    }
}
