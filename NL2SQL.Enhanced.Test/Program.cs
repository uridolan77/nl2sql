using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;
using NL2SQL.Infrastructure.Services;

namespace NL2SQL.Enhanced.Test
{
    class Program
    {
        private static readonly string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

        static async Task Main(string[] args)
        {
            Console.WriteLine("🎰 NL2SQL Enhanced Test - Advanced NLP & Semantic Search");
            Console.WriteLine("=========================================================");

            // Build host with dependency injection
            var host = CreateHostBuilder(args).Build();

            try
            {
                // Initialize services
                await InitializeServicesAsync(host.Services);

                // Test advanced NLP pipeline
                await TestAdvancedNLPPipeline(host.Services);

                // Test semantic search
                await TestSemanticSearch(host.Services);

                // Test gambling domain knowledge
                await TestGamblingDomainKnowledge(host.Services);

                // Test complete enhanced system
                await TestCompleteEnhancedSystem(host.Services);
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

                    // Add caching
                    services.AddMemoryCache();

                    // Add HTTP client
                    services.AddHttpClient();

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

        static async Task InitializeServicesAsync(IServiceProvider services)
        {
            Console.WriteLine("🔧 Initializing enhanced NLP services...");

            var semanticSearch = services.GetRequiredService<ISemanticSearchService>();

            // Load mock metadata for semantic search
            var tables = GetMockTableMetadata();
            var columns = GetMockColumnMetadata();

            await semanticSearch.InitializeAsync(tables, columns);

            Console.WriteLine($"✅ Initialized semantic search with {tables.Count} tables and {columns.Count} columns");
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

        static List<TableMetadata> GetMockTableMetadata()
        {
            return new List<TableMetadata>
            {
                new TableMetadata
                {
                    TableName = "tbl_Daily_actions",
                    BusinessPurpose = "Daily player gambling activity and financial transactions",
                    Domain = "Gambling",
                    ImportanceScore = 0.95f,
                    Keywords = GenerateTableKeywords("tbl_Daily_actions", "Daily player gambling activity and financial transactions")
                },
                new TableMetadata
                {
                    TableName = "tbl_Daily_actions_players",
                    BusinessPurpose = "Player demographics and segmentation data",
                    Domain = "Player Management",
                    ImportanceScore = 0.90f,
                    Keywords = GenerateTableKeywords("tbl_Daily_actions_players", "Player demographics and segmentation data")
                },
                new TableMetadata
                {
                    TableName = "tbl_Games",
                    BusinessPurpose = "Game catalog and configuration",
                    Domain = "Game Management",
                    ImportanceScore = 0.85f,
                    Keywords = GenerateTableKeywords("tbl_Games", "Game catalog and configuration")
                },
                new TableMetadata
                {
                    TableName = "tbl_Transactions",
                    BusinessPurpose = "Financial transactions and payment processing",
                    Domain = "Finance",
                    ImportanceScore = 0.92f,
                    Keywords = GenerateTableKeywords("tbl_Transactions", "Financial transactions and payment processing")
                }
            };
        }

        static List<ColumnMetadata> GetMockColumnMetadata()
        {
            return new List<ColumnMetadata>
            {
                // tbl_Daily_actions columns
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions",
                    ColumnName = "PlayerID",
                    BusinessMeaning = "Unique player identifier",
                    DataType = "int",
                    Synonyms = GenerateColumnSynonyms("PlayerID"),
                    Keywords = GenerateColumnKeywords("PlayerID", "Unique player identifier")
                },
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions",
                    ColumnName = "BetsCasino",
                    BusinessMeaning = "Total casino bets amount",
                    DataType = "decimal",
                    Synonyms = GenerateColumnSynonyms("BetsCasino"),
                    Keywords = GenerateColumnKeywords("BetsCasino", "Total casino bets amount")
                },
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions",
                    ColumnName = "WinsCasino",
                    BusinessMeaning = "Total casino wins amount",
                    DataType = "decimal",
                    Synonyms = GenerateColumnSynonyms("WinsCasino"),
                    Keywords = GenerateColumnKeywords("WinsCasino", "Total casino wins amount")
                },
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions",
                    ColumnName = "Date",
                    BusinessMeaning = "Transaction date",
                    DataType = "datetime",
                    Synonyms = GenerateColumnSynonyms("Date"),
                    Keywords = GenerateColumnKeywords("Date", "Transaction date")
                },
                // tbl_Daily_actions_players columns
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions_players",
                    ColumnName = "PlayerID",
                    BusinessMeaning = "Unique player identifier",
                    DataType = "int",
                    Synonyms = GenerateColumnSynonyms("PlayerID"),
                    Keywords = GenerateColumnKeywords("PlayerID", "Unique player identifier")
                },
                new ColumnMetadata
                {
                    TableName = "tbl_Daily_actions_players",
                    ColumnName = "PlayerSegment",
                    BusinessMeaning = "Player tier classification (VIP, Regular, etc.)",
                    DataType = "varchar",
                    Synonyms = GenerateColumnSynonyms("PlayerSegment"),
                    Keywords = GenerateColumnKeywords("PlayerSegment", "Player tier classification")
                }
            };
        }

        static List<string> GenerateTableKeywords(string tableName, string businessPurpose)
        {
            var keywords = new List<string>();

            // Extract keywords from table name
            var nameWords = tableName.ToLowerInvariant()
                .Replace("tbl_", "")
                .Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            keywords.AddRange(nameWords);

            // Extract keywords from business purpose
            if (!string.IsNullOrEmpty(businessPurpose))
            {
                var purposeWords = businessPurpose.ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 3);
                keywords.AddRange(purposeWords);
            }

            // Add domain-specific keywords
            if (tableName.ToLowerInvariant().Contains("player"))
                keywords.AddRange(new[] { "customer", "user", "member", "gambler" });

            if (tableName.ToLowerInvariant().Contains("action"))
                keywords.AddRange(new[] { "activity", "transaction", "event", "behavior" });

            if (tableName.ToLowerInvariant().Contains("game"))
                keywords.AddRange(new[] { "gaming", "casino", "slot", "betting" });

            return keywords.Distinct().ToList();
        }

        static List<string> GenerateColumnSynonyms(string columnName)
        {
            var synonyms = new List<string>();
            var nameLower = columnName.ToLowerInvariant();

            var synonymMap = new Dictionary<string, string[]>
            {
                { "playerid", new[] { "customer_id", "user_id", "member_id" } },
                { "deposits", new[] { "funding", "payments_in", "top_ups" } },
                { "withdrawals", new[] { "cashouts", "payments_out", "payouts" } },
                { "bets", new[] { "wagers", "stakes", "plays" } },
                { "wins", new[] { "winnings", "payouts", "prizes" } },
                { "ggr", new[] { "gross_gaming_revenue", "gross_revenue" } },
                { "ngr", new[] { "net_gaming_revenue", "net_revenue" } },
                { "rtp", new[] { "return_to_player", "payout_percentage" } }
            };

            foreach (var mapping in synonymMap)
            {
                if (nameLower.Contains(mapping.Key))
                {
                    synonyms.AddRange(mapping.Value);
                }
            }

            return synonyms;
        }

        static List<string> GenerateColumnKeywords(string columnName, string businessMeaning)
        {
            var keywords = new List<string>();

            // Extract from column name
            var nameWords = columnName.ToLowerInvariant()
                .Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            keywords.AddRange(nameWords);

            // Extract from business meaning
            if (!string.IsNullOrEmpty(businessMeaning))
            {
                var meaningWords = businessMeaning.ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2);
                keywords.AddRange(meaningWords);
            }

            return keywords.Distinct().ToList();
        }
    }
}
