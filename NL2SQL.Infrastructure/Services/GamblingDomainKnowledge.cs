using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Gambling domain knowledge service providing industry-specific term definitions and mappings
    /// </summary>
    public class GamblingDomainKnowledge : IGamblingDomainKnowledge
    {
        private readonly ILogger<GamblingDomainKnowledge> _logger;
        private readonly IMemoryCache _cache;
        
        private readonly Dictionary<string, GamblingTermDefinition> _gamblingTerms;
        private readonly Dictionary<string, MetricCalculation> _metricCalculations;
        private readonly List<QueryPattern> _queryPatterns;

        public GamblingDomainKnowledge(ILogger<GamblingDomainKnowledge> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
            
            _gamblingTerms = InitializeGamblingTerms();
            _metricCalculations = InitializeMetricCalculations();
            _queryPatterns = InitializeQueryPatterns();
        }

        public async Task<Dictionary<string, GamblingTermDefinition>> GetGamblingTermsAsync()
        {
            const string cacheKey = "gambling_terms";
            
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, GamblingTermDefinition>? cached) && cached != null)
            {
                return cached;
            }

            // In production, this might load from a database or external service
            var terms = _gamblingTerms;
            _cache.Set(cacheKey, terms, TimeSpan.FromHours(24));
            
            await Task.CompletedTask;
            return terms;
        }

        public async Task<List<TermColumnMapping>> MapTermsToColumnsAsync(List<string> terms)
        {
            var mappings = new List<TermColumnMapping>();

            foreach (var term in terms)
            {
                var termLower = term.ToLowerInvariant();
                
                if (_gamblingTerms.TryGetValue(termLower, out var definition))
                {
                    foreach (var table in definition.RelatedTables)
                    {
                        foreach (var column in definition.RelatedColumns)
                        {
                            mappings.Add(new TermColumnMapping
                            {
                                Term = term,
                                TableName = table,
                                ColumnName = column,
                                MatchScore = 0.95f,
                                MatchType = "exact"
                            });
                        }
                    }
                }
                else
                {
                    // Try fuzzy matching
                    var fuzzyMatches = FindFuzzyMatches(termLower);
                    mappings.AddRange(fuzzyMatches);
                }
            }

            await Task.CompletedTask;
            return mappings;
        }

        public async Task<Dictionary<string, MetricCalculation>> GetMetricCalculationsAsync()
        {
            const string cacheKey = "metric_calculations";
            
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, MetricCalculation>? cached) && cached != null)
            {
                return cached;
            }

            var calculations = _metricCalculations;
            _cache.Set(cacheKey, calculations, TimeSpan.FromHours(24));
            
            await Task.CompletedTask;
            return calculations;
        }

        public async Task<BusinessRuleValidation> ValidateBusinessRulesAsync(string query, EntityExtractionResult entities)
        {
            var validation = new BusinessRuleValidation { IsValid = true };

            // Rule 1: GGR calculations must include both bets and wins
            if (entities.GamblingEntities.Any(e => e.EntityType == GamblingEntityType.GGR))
            {
                var hasBets = entities.FinancialEntities.Any(e => e.EntityType == FinancialEntityType.Bet) ||
                             query.ToLowerInvariant().Contains("bet");
                var hasWins = entities.FinancialEntities.Any(e => e.EntityType == FinancialEntityType.Win) ||
                             query.ToLowerInvariant().Contains("win");

                if (!hasBets || !hasWins)
                {
                    validation.Warnings.Add("GGR calculation typically requires both bet and win amounts");
                    validation.Suggestions.Add("Consider including both bet amounts and win amounts in your query");
                }
            }

            // Rule 2: Player queries should specify time period for meaningful results
            if (entities.PlayerEntities.Any() && !entities.TemporalEntities.Any())
            {
                validation.Warnings.Add("Player analysis queries often benefit from specifying a time period");
                validation.Suggestions.Add("Consider adding a time period like 'last month' or 'this year'");
            }

            // Rule 3: Revenue queries should consider currency consistency
            if (entities.GamblingEntities.Any(e => e.EntityType == GamblingEntityType.GGR || e.EntityType == GamblingEntityType.NGR))
            {
                var currencies = entities.FinancialEntities
                    .Where(e => !string.IsNullOrEmpty(e.Currency))
                    .Select(e => e.Currency)
                    .Distinct()
                    .ToList();

                if (currencies.Count > 1)
                {
                    validation.Warnings.Add($"Multiple currencies detected: {string.Join(", ", currencies)}");
                    validation.Suggestions.Add("Consider specifying a single currency or using currency conversion");
                }
            }

            // Rule 4: Compliance check for sensitive data
            var sensitiveTerms = new[] { "personal", "address", "phone", "email", "ssn", "credit card" };
            if (sensitiveTerms.Any(term => query.ToLowerInvariant().Contains(term)))
            {
                validation.IsValid = false;
                validation.Violations.Add("Query may request sensitive personal information");
                validation.Suggestions.Add("Ensure compliance with data protection regulations (GDPR, CCPA)");
            }

            await Task.CompletedTask;
            return validation;
        }

        public async Task<List<QueryPattern>> GetQueryPatternsAsync()
        {
            await Task.CompletedTask;
            return _queryPatterns;
        }

        private Dictionary<string, GamblingTermDefinition> InitializeGamblingTerms()
        {
            return new Dictionary<string, GamblingTermDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "ggr",
                    new GamblingTermDefinition
                    {
                        Term = "Gross Gaming Revenue",
                        Definition = "Total amount wagered by players minus total winnings paid out, before deducting bonuses, taxes, or other costs",
                        Synonyms = new List<string> { "ggr", "gross gaming revenue", "gross revenue" },
                        RelatedTables = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_players" },
                        RelatedColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive" },
                        CalculationFormula = "(BetsCasino + BetsSport + BetsLive) - (WinsCasino + WinsSport + WinsLive)",
                        Domain = "Financial Operations"
                    }
                },
                {
                    "ngr",
                    new GamblingTermDefinition
                    {
                        Term = "Net Gaming Revenue",
                        Definition = "Gross Gaming Revenue minus bonuses paid, free bet costs, and other player-favorable adjustments",
                        Synonyms = new List<string> { "ngr", "net gaming revenue", "net revenue" },
                        RelatedTables = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_players" },
                        RelatedColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive", "Bonuses" },
                        CalculationFormula = "GGR - Bonuses - FreeBets - Adjustments",
                        Domain = "Financial Operations"
                    }
                },
                {
                    "rtp",
                    new GamblingTermDefinition
                    {
                        Term = "Return to Player",
                        Definition = "Theoretical percentage of total wagered money that a game will pay back to players over extended play",
                        Synonyms = new List<string> { "rtp", "return to player", "payout percentage" },
                        RelatedTables = new List<string> { "Games", "tbl_Daily_actions_games" },
                        RelatedColumns = new List<string> { "RTP", "PayoutPercentage", "TheoreticalRTP" },
                        CalculationFormula = "(Total Wins / Total Bets) * 100",
                        Domain = "Gaming Activity"
                    }
                },
                {
                    "player",
                    new GamblingTermDefinition
                    {
                        Term = "Player",
                        Definition = "Individual customer who participates in gambling activities on the platform",
                        Synonyms = new List<string> { "player", "customer", "user", "member", "gambler" },
                        RelatedTables = new List<string> { "tbl_Daily_actions_players", "tbl_Daily_actions" },
                        RelatedColumns = new List<string> { "PlayerID", "CustomerID", "UserID" },
                        CalculationFormula = "",
                        Domain = "Player Management"
                    }
                },
                {
                    "deposit",
                    new GamblingTermDefinition
                    {
                        Term = "Deposit",
                        Definition = "Money transferred by a player into their gambling account to fund their play",
                        Synonyms = new List<string> { "deposit", "funding", "top-up", "payment in" },
                        RelatedTables = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_players", "tbl_Daily_actionsGBP_transactions" },
                        RelatedColumns = new List<string> { "Deposits", "DepositAmount", "FundingAmount" },
                        CalculationFormula = "SUM(Deposits)",
                        Domain = "Financial Operations"
                    }
                },
                {
                    "withdrawal",
                    new GamblingTermDefinition
                    {
                        Term = "Withdrawal",
                        Definition = "Money transferred from a player's gambling account back to their bank account or payment method",
                        Synonyms = new List<string> { "withdrawal", "cashout", "payout", "payment out" },
                        RelatedTables = new List<string> { "tbl_Daily_actions", "tbl_Daily_actions_players", "tbl_Daily_actionsGBP_transactions" },
                        RelatedColumns = new List<string> { "Withdrawals", "WithdrawalAmount", "CashoutAmount" },
                        CalculationFormula = "SUM(Withdrawals)",
                        Domain = "Financial Operations"
                    }
                }
            };
        }

        private Dictionary<string, MetricCalculation> InitializeMetricCalculations()
        {
            return new Dictionary<string, MetricCalculation>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "GGR",
                    new MetricCalculation
                    {
                        MetricName = "Gross Gaming Revenue",
                        Formula = "(BetsCasino + BetsSport + BetsLive) - (WinsCasino + WinsSport + WinsLive)",
                        RequiredTables = new List<string> { "tbl_Daily_actions" },
                        RequiredColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive" },
                        SqlTemplate = "SELECT SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive) as GGR FROM tbl_Daily_actions WHERE {date_filter}",
                        Parameters = new Dictionary<string, string> { { "date_filter", "Date >= DATEADD(month, -1, GETDATE())" } }
                    }
                },
                {
                    "NGR",
                    new MetricCalculation
                    {
                        MetricName = "Net Gaming Revenue",
                        Formula = "GGR - Bonuses - FreeBets",
                        RequiredTables = new List<string> { "tbl_Daily_actions" },
                        RequiredColumns = new List<string> { "BetsCasino", "BetsSport", "BetsLive", "WinsCasino", "WinsSport", "WinsLive", "Bonuses" },
                        SqlTemplate = "SELECT (SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive)) - SUM(ISNULL(Bonuses, 0)) as NGR FROM tbl_Daily_actions WHERE {date_filter}",
                        Parameters = new Dictionary<string, string> { { "date_filter", "Date >= DATEADD(month, -1, GETDATE())" } }
                    }
                },
                {
                    "PlayerCount",
                    new MetricCalculation
                    {
                        MetricName = "Active Player Count",
                        Formula = "COUNT(DISTINCT PlayerID)",
                        RequiredTables = new List<string> { "tbl_Daily_actions" },
                        RequiredColumns = new List<string> { "PlayerID" },
                        SqlTemplate = "SELECT COUNT(DISTINCT PlayerID) as ActivePlayers FROM tbl_Daily_actions WHERE {date_filter}",
                        Parameters = new Dictionary<string, string> { { "date_filter", "Date >= DATEADD(month, -1, GETDATE())" } }
                    }
                }
            };
        }

        private List<QueryPattern> InitializeQueryPatterns()
        {
            return new List<QueryPattern>
            {
                new QueryPattern
                {
                    PatternName = "GGR Calculation",
                    Pattern = @"\b(ggr|gross\s+gaming\s+revenue)\b.*\b(last|this|past)\s+(month|week|year|quarter)\b",
                    SqlTemplate = "SELECT SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive) as GGR FROM tbl_Daily_actions WHERE Date >= {date_filter}",
                    RequiredEntities = new List<string> { "GGR", "Temporal" },
                    Confidence = 0.9f,
                    Domain = "Financial Operations"
                },
                new QueryPattern
                {
                    PatternName = "Top Players by Deposits",
                    Pattern = @"\b(top|best|highest)\s+\d*\s*(player|customer)s?\s+by\s+(deposit|funding)\b",
                    SqlTemplate = "SELECT TOP {limit} PlayerID, SUM(Deposits) as TotalDeposits FROM tbl_Daily_actions_players WHERE Date >= {date_filter} GROUP BY PlayerID ORDER BY TotalDeposits DESC",
                    RequiredEntities = new List<string> { "Player", "Deposit", "TopN" },
                    Confidence = 0.85f,
                    Domain = "Player Management"
                },
                new QueryPattern
                {
                    PatternName = "Player Count",
                    Pattern = @"\b(how\s+many|count|number\s+of)\s+(player|customer|user)s?\b",
                    SqlTemplate = "SELECT COUNT(DISTINCT PlayerID) as PlayerCount FROM tbl_Daily_actions WHERE Date >= {date_filter}",
                    RequiredEntities = new List<string> { "Player", "Count" },
                    Confidence = 0.8f,
                    Domain = "Player Management"
                }
            };
        }

        private List<TermColumnMapping> FindFuzzyMatches(string term)
        {
            var mappings = new List<TermColumnMapping>();

            // Simple fuzzy matching based on common gambling terms
            var fuzzyMappings = new Dictionary<string, (string table, string column, float score)>
            {
                { "revenue", ("tbl_Daily_actions", "BetsCasino", 0.7f) },
                { "profit", ("tbl_Daily_actions", "BetsCasino", 0.6f) },
                { "income", ("tbl_Daily_actions", "BetsCasino", 0.6f) },
                { "bet", ("tbl_Daily_actions", "BetsCasino", 0.8f) },
                { "wager", ("tbl_Daily_actions", "BetsCasino", 0.8f) },
                { "win", ("tbl_Daily_actions", "WinsCasino", 0.8f) },
                { "payout", ("tbl_Daily_actions", "WinsCasino", 0.7f) },
                { "customer", ("tbl_Daily_actions_players", "PlayerID", 0.8f) },
                { "user", ("tbl_Daily_actions_players", "PlayerID", 0.7f) },
                { "game", ("Games", "GameName", 0.8f) }
            };

            foreach (var mapping in fuzzyMappings)
            {
                if (term.Contains(mapping.Key) || mapping.Key.Contains(term))
                {
                    mappings.Add(new TermColumnMapping
                    {
                        Term = term,
                        TableName = mapping.Value.table,
                        ColumnName = mapping.Value.column,
                        MatchScore = mapping.Value.score,
                        MatchType = "fuzzy"
                    });
                }
            }

            return mappings;
        }
    }
}
