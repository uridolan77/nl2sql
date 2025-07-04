using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NL2SQL.Test
{
    class Program
    {
        private static readonly string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

        static async Task Main(string[] args)
        {
            Console.WriteLine("🎰 NL2SQL Simple Test - Gambling Industry");
            Console.WriteLine("==========================================");

            try
            {
                // Test database connection
                await TestDatabaseConnection();

                // Load and display business metadata
                await LoadBusinessMetadata();

                // Test simple NL2SQL functionality
                await TestSimpleNL2SQL();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task TestDatabaseConnection()
        {
            Console.WriteLine("🔗 Testing database connection...");

            try
            {
                using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync();
                Console.WriteLine("✅ Database connection successful!");

                // Test a simple query
                using var command = new SqlCommand("SELECT COUNT(*) FROM [BIReportingCopilot_Dev].[dbo].[BusinessTableInfo]", connection);
                var count = await command.ExecuteScalarAsync();
                Console.WriteLine($"📊 Found {count} business tables in metadata");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database connection failed: {ex.Message}");
                throw;
            }

            Console.WriteLine();
        }

        static async Task LoadBusinessMetadata()
        {
            Console.WriteLine("📊 Loading business metadata...");

            try
            {
                using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync();

                // Load business tables
                var tables = await LoadBusinessTables(connection);
                Console.WriteLine($"✅ Loaded {tables.Count} business tables");

                // Display top 5 tables by importance
                var topTables = tables.OrderByDescending(t => t.ImportanceScore).Take(5);
                Console.WriteLine("\n🏆 Top 5 Most Important Tables:");
                foreach (var table in topTables)
                {
                    Console.WriteLine($"  • {table.TableName} (Score: {table.ImportanceScore:F2})");
                    Console.WriteLine($"    Purpose: {table.BusinessPurpose}");
                    Console.WriteLine($"    Domain: {table.DomainClassification}");
                }

                // Load business glossary
                var glossaryTerms = await LoadBusinessGlossary(connection);
                Console.WriteLine($"\n✅ Loaded {glossaryTerms.Count} business glossary terms");

                // Display gambling-specific terms
                var gamblingTerms = glossaryTerms.Where(g =>
                    g.Domain?.Contains("Financial") == true ||
                    g.Term?.ToLower().Contains("ggr") == true ||
                    g.Term?.ToLower().Contains("ngr") == true ||
                    g.Term?.ToLower().Contains("rtp") == true)
                    .Take(5);

                Console.WriteLine("\n💰 Key Gambling Terms:");
                foreach (var term in gamblingTerms)
                {
                    Console.WriteLine($"  • {term.Term}: {term.Definition?.Substring(0, Math.Min(100, term.Definition?.Length ?? 0))}...");
                }

                // Load business domains
                var domains = await LoadBusinessDomains(connection);
                Console.WriteLine($"\n✅ Loaded {domains.Count} business domains");
                foreach (var domain in domains.OrderByDescending(d => d.ImportanceScore))
                {
                    Console.WriteLine($"  • {domain.DomainName} (Score: {domain.ImportanceScore:F2})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading metadata: {ex.Message}");
                throw;
            }

            Console.WriteLine();
        }

        static async Task TestSimpleNL2SQL()
        {
            Console.WriteLine("🎲 Testing Simple NL2SQL Functionality...");

            var testQueries = new[]
            {
                "Show me the total deposits for last month",
                "What are the top 10 players by deposits?",
                "How many players registered yesterday?",
                "Show daily revenue for this week",
                "Calculate GGR for this month"
            };

            foreach (var query in testQueries)
            {
                Console.WriteLine($"\n🔎 Processing: \"{query}\"");

                try
                {
                    var analysis = AnalyzeQuery(query);
                    Console.WriteLine($"  🎯 Intent: {analysis.Intent}");
                    Console.WriteLine($"  🏷️ Entities: {string.Join(", ", analysis.Entities)}");
                    Console.WriteLine($"  📊 Complexity: {analysis.Complexity}");

                    var sql = GenerateSimpleSQL(analysis);
                    Console.WriteLine($"  ✅ Generated SQL: {sql}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ❌ Error: {ex.Message}");
                }
            }

            Console.WriteLine();
        }

        static async Task<List<BusinessTable>> LoadBusinessTables(SqlConnection connection)
        {
            const string sql = @"
                SELECT TOP 20
                    TableName, BusinessPurpose, DomainClassification, ImportanceScore, UsageFrequency
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessTableInfo]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC, UsageFrequency DESC";

            var tables = new List<BusinessTable>();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tables.Add(new BusinessTable
                {
                    TableName = reader.GetString("TableName"),
                    BusinessPurpose = reader.IsDBNull("BusinessPurpose") ? null : reader.GetString("BusinessPurpose"),
                    DomainClassification = reader.IsDBNull("DomainClassification") ? null : reader.GetString("DomainClassification"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : (float)reader.GetDecimal("ImportanceScore"),
                    UsageFrequency = reader.IsDBNull("UsageFrequency") ? 0f : (float)reader.GetDecimal("UsageFrequency")
                });
            }

            return tables;
        }

        static async Task<List<BusinessGlossaryTerm>> LoadBusinessGlossary(SqlConnection connection)
        {
            const string sql = @"
                SELECT TOP 20
                    Term, Definition, Domain, Category
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessGlossary]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC";

            var terms = new List<BusinessGlossaryTerm>();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                terms.Add(new BusinessGlossaryTerm
                {
                    Term = reader.GetString("Term"),
                    Definition = reader.IsDBNull("Definition") ? null : reader.GetString("Definition"),
                    Domain = reader.IsDBNull("Domain") ? null : reader.GetString("Domain"),
                    Category = reader.IsDBNull("Category") ? null : reader.GetString("Category")
                });
            }

            return terms;
        }

        static async Task<List<BusinessDomain>> LoadBusinessDomains(SqlConnection connection)
        {
            const string sql = @"
                SELECT
                    DomainName, Description, ImportanceScore
                FROM [BIReportingCopilot_Dev].[dbo].[BusinessDomain]
                WHERE IsActive = 1
                ORDER BY ImportanceScore DESC";

            var domains = new List<BusinessDomain>();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                domains.Add(new BusinessDomain
                {
                    DomainName = reader.GetString("DomainName"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    ImportanceScore = reader.IsDBNull("ImportanceScore") ? 0f : (float)reader.GetDecimal("ImportanceScore")
                });
            }

            return domains;
        }

        static QueryAnalysis AnalyzeQuery(string query)
        {
            var analysis = new QueryAnalysis
            {
                OriginalQuery = query,
                Entities = new List<string>(),
                Intent = QueryIntent.Select,
                Complexity = QueryComplexity.Simple
            };

            var queryLower = query.ToLowerInvariant();

            // Simple entity extraction
            if (queryLower.Contains("player") || queryLower.Contains("customer"))
                analysis.Entities.Add("Player");
            if (queryLower.Contains("deposit") || queryLower.Contains("payment"))
                analysis.Entities.Add("Deposit");
            if (queryLower.Contains("game") || queryLower.Contains("slot"))
                analysis.Entities.Add("Game");
            if (queryLower.Contains("revenue") || queryLower.Contains("ggr") || queryLower.Contains("ngr"))
                analysis.Entities.Add("Revenue");

            // Simple intent detection
            if (queryLower.Contains("top") || queryLower.Contains("best"))
                analysis.Intent = QueryIntent.TopN;
            else if (queryLower.Contains("total") || queryLower.Contains("sum") || queryLower.Contains("calculate"))
                analysis.Intent = QueryIntent.Aggregate;
            else if (queryLower.Contains("count") || queryLower.Contains("how many"))
                analysis.Intent = QueryIntent.Count;

            // Simple complexity assessment
            var wordCount = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            analysis.Complexity = wordCount switch
            {
                <= 5 => QueryComplexity.Simple,
                <= 10 => QueryComplexity.Medium,
                _ => QueryComplexity.Complex
            };

            return analysis;
        }

        static string GenerateSimpleSQL(QueryAnalysis analysis)
        {
            // Very simple SQL generation based on analysis
            return analysis.Intent switch
            {
                QueryIntent.TopN => "SELECT TOP 10 PlayerID, SUM(Deposits) as TotalDeposits FROM tbl_Daily_actions_players GROUP BY PlayerID ORDER BY TotalDeposits DESC",
                QueryIntent.Aggregate when analysis.Entities.Contains("Revenue") => "SELECT SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive) as GGR FROM tbl_Daily_actions WHERE Date >= DATEADD(month, -1, GETDATE())",
                QueryIntent.Aggregate => "SELECT SUM(Deposits) as TotalDeposits FROM tbl_Daily_actions WHERE Date >= DATEADD(month, -1, GETDATE())",
                QueryIntent.Count => "SELECT COUNT(DISTINCT PlayerID) as PlayerCount FROM tbl_Daily_actions WHERE Date >= DATEADD(day, -1, GETDATE())",
                _ => "SELECT * FROM tbl_Daily_actions WHERE Date >= DATEADD(day, -7, GETDATE())"
            };
        }
    }

    // Simple data models
    public class BusinessTable
    {
        public string TableName { get; set; }
        public string BusinessPurpose { get; set; }
        public string DomainClassification { get; set; }
        public float ImportanceScore { get; set; }
        public float UsageFrequency { get; set; }
    }

    public class BusinessGlossaryTerm
    {
        public string Term { get; set; }
        public string Definition { get; set; }
        public string Domain { get; set; }
        public string Category { get; set; }
    }

    public class BusinessDomain
    {
        public string DomainName { get; set; }
        public string Description { get; set; }
        public float ImportanceScore { get; set; }
    }

    public class QueryAnalysis
    {
        public string OriginalQuery { get; set; }
        public List<string> Entities { get; set; } = new List<string>();
        public QueryIntent Intent { get; set; }
        public QueryComplexity Complexity { get; set; }
    }

    public enum QueryIntent
    {
        Select,
        Aggregate,
        TopN,
        Count
    }

    public enum QueryComplexity
    {
        Simple,
        Medium,
        Complex
    }
}
