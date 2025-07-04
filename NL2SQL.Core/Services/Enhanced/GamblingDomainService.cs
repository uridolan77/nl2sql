using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Enhanced;
using NL2SQL.Core.Interfaces.Enhanced;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace NL2SQL.Core.Services.Enhanced
{
    /// <summary>
    /// Gambling domain service with industry-specific knowledge
    /// </summary>
    public class GamblingDomainService : IGamblingDomainService
    {
        private readonly ILogger<GamblingDomainService> _logger;
        private readonly IEnhancedMetadataRepository _metadataRepository;
        private readonly IMemoryCache _cache;

        // Gambling-specific metric definitions
        private readonly Dictionary<string, GamblingMetric> _standardMetrics = new()
        {
            ["GGR"] = new GamblingMetric
            {
                MetricId = "GGR",
                Name = "Gross Gaming Revenue",
                Description = "Total amount wagered by players minus total winnings paid out, before deducting bonuses, taxes, or operational costs",
                CalculationFormula = "SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive)",
                RequiredTables = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_games" },
                RequiredColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive" },
                Domain = "Financial",
                Type = MetricType.Financial,
                Unit = "Currency"
            },
            ["NGR"] = new GamblingMetric
            {
                MetricId = "NGR",
                Name = "Net Gaming Revenue",
                Description = "Gross Gaming Revenue minus bonuses paid, free bet costs, and other player-favorable adjustments",
                CalculationFormula = "(SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive)) - SUM(BonusCosts + FreeBetCosts)",
                RequiredTables = new List<string> { "tbl_Daily_actions", "tbl_Bonuses" },
                RequiredColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive", "BonusCosts" },
                Domain = "Financial",
                Type = MetricType.Financial,
                Unit = "Currency"
            },
            ["RTP"] = new GamblingMetric
            {
                MetricId = "RTP",
                Name = "Return to Player",
                Description = "Theoretical percentage of total wagered money that a game will pay back to players over extended play period",
                CalculationFormula = "(SUM(TotalWins) / SUM(TotalBets)) * 100",
                RequiredTables = new List<string> { "tbl_Daily_actions_games", "Games" },
                RequiredColumns = new List<string> { "TotalBets", "TotalWins" },
                Domain = "Gaming",
                Type = MetricType.Performance,
                Unit = "Percentage"
            },
            ["LTV"] = new GamblingMetric
            {
                MetricId = "LTV",
                Name = "Player Lifetime Value",
                Description = "Predicted total revenue a player will generate throughout their relationship with the platform",
                CalculationFormula = "AVG(MonthlyRevenue) * AVG(RetentionMonths)",
                RequiredTables = new List<string> { "tbl_Daily_actions_players" },
                RequiredColumns = new List<string> { "PlayerID", "Deposits", "Date" },
                Domain = "Player Analytics",
                Type = MetricType.Engagement,
                Unit = "Currency"
            },
            ["ARPU"] = new GamblingMetric
            {
                MetricId = "ARPU",
                Name = "Average Revenue Per User",
                Description = "Total revenue divided by number of active players over specific time period",
                CalculationFormula = "SUM(TotalRevenue) / COUNT(DISTINCT PlayerID)",
                RequiredTables = new List<string> { "tbl_Daily_actions_players" },
                RequiredColumns = new List<string> { "PlayerID", "Deposits" },
                Domain = "Player Analytics",
                Type = MetricType.Financial,
                Unit = "Currency"
            }
        };

        // Entity to table mappings
        private readonly Dictionary<string, List<string>> _entityTableMappings = new()
        {
            ["Player"] = new List<string> { "tbl_Daily_actions_players", "tbl_Daily_actions" },
            ["Game"] = new List<string> { "Games", "tbl_Daily_actions_games", "GamesDescriptions" },
            ["Bet"] = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_games" },
            ["Deposit"] = new List<string> { "tbl_Daily_actions", "tbl_Daily_actionsGBP_transactions" },
            ["Withdrawal"] = new List<string> { "tbl_Withdrawal_requests", "tbl_Daily_actions" },
            ["Bonus"] = new List<string> { "tbl_Bonuses", "tbl_Bonus_balances" },
            ["Revenue"] = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_games" },
            ["Session"] = new List<string> { "GamesCasinoSessions", "tbl_Daily_actions_games" },
            ["Geography"] = new List<string> { "tbl_Countries" },
            ["Brand"] = new List<string> { "tbl_White_labels" }
        };

        // Business rules for gambling domain
        private readonly List<BusinessRule> _gamblingRules = new()
        {
            new BusinessRule
            {
                RuleId = "GGR_CALCULATION",
                Name = "GGR Calculation Rule",
                Description = "GGR must be calculated as total bets minus total wins",
                Expression = "GGR = SUM(Bets) - SUM(Wins)",
                Type = BusinessRuleType.Calculation,
                IsActive = true,
                Priority = 1.0f
            },
            new BusinessRule
            {
                RuleId = "RTP_COMPLIANCE",
                Name = "RTP Compliance Rule",
                Description = "RTP must be between 85% and 98% for regulatory compliance",
                Expression = "RTP >= 85 AND RTP <= 98",
                Type = BusinessRuleType.Compliance,
                IsActive = true,
                Priority = 0.9f
            },
            new BusinessRule
            {
                RuleId = "PLAYER_PRIVACY",
                Name = "Player Privacy Rule",
                Description = "Player personal data must be masked in reports",
                Expression = "MASK(PlayerPersonalData)",
                Type = BusinessRuleType.Security,
                IsActive = true,
                Priority = 1.0f
            }
        };

        public GamblingDomainService(
            ILogger<GamblingDomainService> logger,
            IEnhancedMetadataRepository metadataRepository,
            IMemoryCache cache)
        {
            _logger = logger;
            _metadataRepository = metadataRepository;
            _cache = cache;
        }

        public async Task<List<GamblingMetric>> GetAvailableMetricsAsync()
        {
            const string cacheKey = "gambling_metrics_all";
            
            if (_cache.TryGetValue(cacheKey, out List<GamblingMetric> cachedMetrics))
            {
                return cachedMetrics;
            }

            var metrics = _standardMetrics.Values.ToList();
            
            // Add custom metrics from database
            var customMetrics = await LoadCustomMetricsAsync();
            metrics.AddRange(customMetrics);

            _cache.Set(cacheKey, metrics, TimeSpan.FromHours(1));
            return metrics;
        }

        public async Task<GamblingMetric> GetMetricDefinitionAsync(string metricName)
        {
            if (_standardMetrics.TryGetValue(metricName.ToUpperInvariant(), out var metric))
            {
                return metric;
            }

            // Try to find in custom metrics
            var customMetrics = await LoadCustomMetricsAsync();
            return customMetrics.FirstOrDefault(m => 
                string.Equals(m.MetricId, metricName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(m.Name, metricName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<CalculationRule> GetMetricCalculationAsync(string metricName)
        {
            var metric = await GetMetricDefinitionAsync(metricName);
            if (metric == null) return null;

            return new CalculationRule
            {
                CalculationId = $"{metric.MetricId}_CALC",
                Name = $"{metric.Name} Calculation",
                Formula = metric.CalculationFormula,
                RequiredInputs = metric.RequiredColumns,
                OutputType = metric.Unit,
                Description = metric.Description,
                IsStandardMetric = _standardMetrics.ContainsKey(metric.MetricId)
            };
        }

        public async Task<List<BusinessRule>> GetApplicableRulesAsync(QueryContext context)
        {
            var applicableRules = new List<BusinessRule>(_gamblingRules);

            // Add domain-specific rules based on context
            if (context.PrimaryDomain?.DomainName == "Financial Operations")
            {
                applicableRules.AddRange(await GetFinancialRulesAsync());
            }
            else if (context.PrimaryDomain?.DomainName == "Gaming Activity")
            {
                applicableRules.AddRange(await GetGamingRulesAsync());
            }

            return applicableRules.Where(r => r.IsActive).OrderByDescending(r => r.Priority).ToList();
        }

        public async Task<ComplianceCheck> ValidateComplianceAsync(string sql)
        {
            var complianceCheck = new ComplianceCheck
            {
                IsCompliant = true,
                Issues = new List<string>(),
                Recommendations = new List<string>()
            };

            // Check for sensitive data access
            if (ContainsSensitiveDataAccess(sql))
            {
                complianceCheck.Issues.Add("Query accesses sensitive player data");
                complianceCheck.Recommendations.Add("Consider using data masking or aggregation");
                complianceCheck.IsCompliant = false;
            }

            // Check for regulatory compliance
            if (!await ValidateRegulatoryComplianceAsync(sql))
            {
                complianceCheck.Issues.Add("Query may not comply with gambling regulations");
                complianceCheck.Recommendations.Add("Review regulatory requirements for data access");
                complianceCheck.IsCompliant = false;
            }

            return complianceCheck;
        }

        public async Task<List<RecommendedQuery>> GetRecommendedQueriesAsync(QueryContext context)
        {
            var recommendations = new List<RecommendedQuery>();

            // Get domain-specific recommendations
            var domain = context.PrimaryDomain?.DomainName ?? "General";
            
            switch (domain)
            {
                case "Financial Operations":
                    recommendations.AddRange(await GetFinancialRecommendationsAsync());
                    break;
                case "Gaming Activity":
                    recommendations.AddRange(await GetGamingRecommendationsAsync());
                    break;
                case "Player Management":
                    recommendations.AddRange(await GetPlayerRecommendationsAsync());
                    break;
                default:
                    recommendations.AddRange(await GetGeneralRecommendationsAsync());
                    break;
            }

            return recommendations.Take(10).ToList();
        }

        public async Task<BusinessConcept> GetBusinessConceptAsync(string conceptId)
        {
            var glossary = await _metadataRepository.GetBusinessGlossaryAsync();
            var term = glossary.FirstOrDefault(g => 
                string.Equals(g.Term, conceptId, StringComparison.OrdinalIgnoreCase));

            if (term == null) return null;

            return new BusinessConcept
            {
                ConceptId = conceptId,
                Name = term.Term,
                Definition = term.Definition,
                Domain = term.Domain,
                Synonyms = ParseJsonArray(term.Synonyms),
                RelatedConcepts = ParseJsonArray(term.RelatedTerms),
                Type = DetermineConceptType(term.Category),
                Confidence = term.ConfidenceScore
            };
        }

        public async Task<List<string>> GetAmbiguousTermsAsync(string query)
        {
            var ambiguousTerms = new List<string>();
            var glossary = await _metadataRepository.GetBusinessGlossaryAsync();

            // Find terms with high ambiguity scores
            var ambiguousGlossaryTerms = glossary
                .Where(g => g.AmbiguityScore > 0.5f)
                .Select(g => g.Term.ToLowerInvariant())
                .ToList();

            var queryWords = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var word in queryWords)
            {
                if (ambiguousGlossaryTerms.Contains(word))
                {
                    ambiguousTerms.Add(word);
                }
            }

            return ambiguousTerms;
        }

        public async Task<List<string>> GetTablesForEntityAsync(string entityType)
        {
            if (_entityTableMappings.TryGetValue(entityType, out var tables))
            {
                return tables;
            }

            // Fallback to metadata search
            var allTables = await _metadataRepository.GetEnhancedTableInfoAsync();
            return allTables
                .Where(t => t.SemanticTags.Any(tag => 
                    string.Equals(tag.Name, entityType, StringComparison.OrdinalIgnoreCase)))
                .Select(t => t.TableName)
                .ToList();
        }

        public async Task<List<string>> GetTablesForConceptAsync(string conceptId)
        {
            var concept = await GetBusinessConceptAsync(conceptId);
            if (concept == null) return new List<string>();

            var glossary = await _metadataRepository.GetBusinessGlossaryAsync();
            var term = glossary.FirstOrDefault(g => 
                string.Equals(g.Term, conceptId, StringComparison.OrdinalIgnoreCase));

            if (term?.MappedTables != null)
            {
                return ParseJsonArray(term.MappedTables);
            }

            return new List<string>();
        }

        public async Task<List<JoinRequirement>> GetOptimalJoinPathsAsync(List<string> tables)
        {
            var joinRequirements = new List<JoinRequirement>();

            // Common join patterns in gambling domain
            var joinPatterns = new Dictionary<(string, string), JoinRequirement>
            {
                [("tbl_Daily_actions", "tbl_Daily_actions_players")] = new JoinRequirement
                {
                    LeftTable = "tbl_Daily_actions",
                    RightTable = "tbl_Daily_actions_players",
                    Type = JoinType.Inner,
                    Conditions = new List<JoinCondition>
                    {
                        new JoinCondition { LeftColumn = "PlayerID", RightColumn = "PlayerID" },
                        new JoinCondition { LeftColumn = "Date", RightColumn = "Date" }
                    },
                    BusinessReason = "Link daily actions to player-specific metrics",
                    Confidence = 0.95f,
                    IsRequired = true
                },
                [("tbl_Daily_actions", "tbl_Countries")] = new JoinRequirement
                {
                    LeftTable = "tbl_Daily_actions",
                    RightTable = "tbl_Countries",
                    Type = JoinType.Left,
                    Conditions = new List<JoinCondition>
                    {
                        new JoinCondition { LeftColumn = "CountryID", RightColumn = "Id" }
                    },
                    BusinessReason = "Add geographical context to player actions",
                    Confidence = 0.85f,
                    IsRequired = false
                },
                [("tbl_Daily_actions_games", "Games")] = new JoinRequirement
                {
                    LeftTable = "tbl_Daily_actions_games",
                    RightTable = "Games",
                    Type = JoinType.Inner,
                    Conditions = new List<JoinCondition>
                    {
                        new JoinCondition { LeftColumn = "GameID", RightColumn = "Id" }
                    },
                    BusinessReason = "Link game actions to game metadata",
                    Confidence = 0.9f,
                    IsRequired = true
                }
            };

            // Find applicable join patterns
            for (int i = 0; i < tables.Count; i++)
            {
                for (int j = i + 1; j < tables.Count; j++)
                {
                    var key1 = (tables[i], tables[j]);
                    var key2 = (tables[j], tables[i]);

                    if (joinPatterns.TryGetValue(key1, out var join1))
                    {
                        joinRequirements.Add(join1);
                    }
                    else if (joinPatterns.TryGetValue(key2, out var join2))
                    {
                        joinRequirements.Add(join2);
                    }
                }
            }

            return joinRequirements;
        }

        // Private helper methods
        private async Task<List<GamblingMetric>> LoadCustomMetricsAsync()
        {
            // Load custom metrics from database or configuration
            // This would typically query a custom metrics table
            return new List<GamblingMetric>();
        }

        private async Task<List<BusinessRule>> GetFinancialRulesAsync()
        {
            return new List<BusinessRule>
            {
                new BusinessRule
                {
                    RuleId = "FINANCIAL_ACCURACY",
                    Name = "Financial Data Accuracy",
                    Description = "All financial calculations must use precise decimal arithmetic",
                    Type = BusinessRuleType.Validation,
                    IsActive = true,
                    Priority = 1.0f
                }
            };
        }

        private async Task<List<BusinessRule>> GetGamingRulesAsync()
        {
            return new List<BusinessRule>
            {
                new BusinessRule
                {
                    RuleId = "GAME_RTP_VALIDATION",
                    Name = "Game RTP Validation",
                    Description = "Game RTP must be within regulatory limits",
                    Type = BusinessRuleType.Compliance,
                    IsActive = true,
                    Priority = 0.9f
                }
            };
        }

        private bool ContainsSensitiveDataAccess(string sql)
        {
            var sensitivePatterns = new[]
            {
                @"\bPlayerID\b",
                @"\bEmail\b",
                @"\bPersonalData\b",
                @"\bCreditCard\b"
            };

            return sensitivePatterns.Any(pattern => 
                System.Text.RegularExpressions.Regex.IsMatch(sql, pattern, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        private async Task<bool> ValidateRegulatoryComplianceAsync(string sql)
        {
            // Implement regulatory compliance checks
            // This would check against specific gambling regulations
            return true; // Simplified for now
        }

        private async Task<List<RecommendedQuery>> GetFinancialRecommendationsAsync()
        {
            return new List<RecommendedQuery>
            {
                new RecommendedQuery
                {
                    Title = "Daily GGR Report",
                    Description = "Calculate daily Gross Gaming Revenue",
                    NaturalLanguage = "Show me the daily GGR for this month",
                    Category = "Financial"
                }
            };
        }

        private async Task<List<RecommendedQuery>> GetGamingRecommendationsAsync()
        {
            return new List<RecommendedQuery>
            {
                new RecommendedQuery
                {
                    Title = "Top Performing Games",
                    Description = "Find the most popular games by revenue",
                    NaturalLanguage = "Show me the top 10 games by revenue this week",
                    Category = "Gaming"
                }
            };
        }

        private async Task<List<RecommendedQuery>> GetPlayerRecommendationsAsync()
        {
            return new List<RecommendedQuery>
            {
                new RecommendedQuery
                {
                    Title = "Player Lifetime Value",
                    Description = "Calculate average player lifetime value",
                    NaturalLanguage = "What is the average player lifetime value?",
                    Category = "Player Analytics"
                }
            };
        }

        private async Task<List<RecommendedQuery>> GetGeneralRecommendationsAsync()
        {
            return new List<RecommendedQuery>
            {
                new RecommendedQuery
                {
                    Title = "Daily Summary",
                    Description = "Get daily business summary",
                    NaturalLanguage = "Show me today's business summary",
                    Category = "General"
                }
            };
        }

        private List<string> ParseJsonArray(string jsonArray)
        {
            if (string.IsNullOrEmpty(jsonArray)) return new List<string>();
            
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonArray) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private ConceptType DetermineConceptType(string category)
        {
            return category?.ToLowerInvariant() switch
            {
                "financial" => ConceptType.Metric,
                "metric" => ConceptType.Metric,
                "dimension" => ConceptType.Dimension,
                "measure" => ConceptType.Measure,
                "entity" => ConceptType.Entity,
                _ => ConceptType.Attribute
            };
        }
    }

    // Supporting models
    public class ComplianceCheck
    {
        public bool IsCompliant { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public class RecommendedQuery
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NaturalLanguage { get; set; }
        public string Category { get; set; }
        public float Relevance { get; set; }
    }
}
