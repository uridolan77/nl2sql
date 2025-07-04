using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Advanced entity extraction implementation using pattern matching and domain knowledge
    /// </summary>
    public class AdvancedEntityExtractor : IAdvancedEntityExtractor
    {
        private readonly ILogger<AdvancedEntityExtractor> _logger;
        private readonly IMemoryCache _cache;
        private readonly IGamblingDomainKnowledge _domainKnowledge;
        
        // Pre-compiled regex patterns for performance
        private readonly Dictionary<GamblingEntityType, Regex> _gamblingPatterns;
        private readonly Dictionary<TemporalType, Regex> _temporalPatterns;
        private readonly Dictionary<FinancialEntityType, Regex> _financialPatterns;
        private readonly Dictionary<PlayerEntityType, Regex> _playerPatterns;
        private readonly Dictionary<GameEntityType, Regex> _gamePatterns;
        private readonly Dictionary<MetricEntityType, Regex> _metricPatterns;

        public AdvancedEntityExtractor(
            ILogger<AdvancedEntityExtractor> logger,
            IMemoryCache cache,
            IGamblingDomainKnowledge domainKnowledge)
        {
            _logger = logger;
            _cache = cache;
            _domainKnowledge = domainKnowledge;
            
            _gamblingPatterns = InitializeGamblingPatterns();
            _temporalPatterns = InitializeTemporalPatterns();
            _financialPatterns = InitializeFinancialPatterns();
            _playerPatterns = InitializePlayerPatterns();
            _gamePatterns = InitializeGamePatterns();
            _metricPatterns = InitializeMetricPatterns();
        }

        public async Task<EntityExtractionResult> ExtractEntitiesAsync(string query)
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Starting advanced entity extraction for query: {Query}", query);

            var result = new EntityExtractionResult
            {
                OriginalQuery = query
            };

            try
            {
                // Extract all entity types in parallel for better performance
                var gamblingTask = ExtractGamblingEntitiesAsync(query);
                var temporalTask = ExtractTemporalEntitiesAsync(query);
                var financialTask = ExtractFinancialEntitiesAsync(query);
                var playerTask = ExtractPlayerEntitiesAsync(query);
                var gameTask = ExtractGameEntitiesAsync(query);
                var metricTask = ExtractMetricEntitiesAsync(query);

                // Wait for all tasks to complete
                await Task.WhenAll(gamblingTask, temporalTask, financialTask, playerTask, gameTask, metricTask);

                result.GamblingEntities = await gamblingTask;
                result.TemporalEntities = await temporalTask;
                result.FinancialEntities = await financialTask;
                result.PlayerEntities = await playerTask;
                result.GameEntities = await gameTask;
                result.MetricEntities = await metricTask;

                // Calculate overall confidence
                var allEntities = new List<BaseEntity>();
                allEntities.AddRange(result.GamblingEntities);
                allEntities.AddRange(result.TemporalEntities);
                allEntities.AddRange(result.FinancialEntities);
                allEntities.AddRange(result.PlayerEntities);
                allEntities.AddRange(result.GameEntities);
                allEntities.AddRange(result.MetricEntities);

                result.OverallConfidence = allEntities.Any() 
                    ? allEntities.Average(e => e.Confidence) 
                    : 0f;

                result.ProcessingTime = DateTime.UtcNow - startTime;

                _logger.LogInformation("Entity extraction completed. Found {Count} entities with confidence {Confidence:F2}", 
                    allEntities.Count, result.OverallConfidence);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during entity extraction");
                result.ProcessingTime = DateTime.UtcNow - startTime;
                return result;
            }
        }

        public async Task<List<GamblingEntity>> ExtractGamblingEntitiesAsync(string query)
        {
            var entities = new List<GamblingEntity>();
            var queryLower = query.ToLowerInvariant();

            foreach (var pattern in _gamblingPatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new GamblingEntity
                    {
                        Text = match.Value,
                        EntityType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculateGamblingEntityConfidence(match.Value, pattern.Key),
                        Source = "GamblingPatterns"
                    };

                    // Enrich with domain knowledge
                    await EnrichGamblingEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<List<TemporalEntity>> ExtractTemporalEntitiesAsync(string query)
        {
            var entities = new List<TemporalEntity>();

            foreach (var pattern in _temporalPatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new TemporalEntity
                    {
                        Text = match.Value,
                        TemporalType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculateTemporalEntityConfidence(match.Value, pattern.Key),
                        Source = "TemporalPatterns"
                    };

                    // Resolve temporal expression to actual dates
                    await ResolveTemporalEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<List<FinancialEntity>> ExtractFinancialEntitiesAsync(string query)
        {
            var entities = new List<FinancialEntity>();

            foreach (var pattern in _financialPatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new FinancialEntity
                    {
                        Text = match.Value,
                        EntityType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculateFinancialEntityConfidence(match.Value, pattern.Key),
                        Source = "FinancialPatterns"
                    };

                    // Extract amount and currency if present
                    await EnrichFinancialEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<List<PlayerEntity>> ExtractPlayerEntitiesAsync(string query)
        {
            var entities = new List<PlayerEntity>();

            foreach (var pattern in _playerPatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new PlayerEntity
                    {
                        Text = match.Value,
                        EntityType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculatePlayerEntityConfidence(match.Value, pattern.Key),
                        Source = "PlayerPatterns"
                    };

                    await EnrichPlayerEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<List<GameEntity>> ExtractGameEntitiesAsync(string query)
        {
            var entities = new List<GameEntity>();

            foreach (var pattern in _gamePatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new GameEntity
                    {
                        Text = match.Value,
                        EntityType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculateGameEntityConfidence(match.Value, pattern.Key),
                        Source = "GamePatterns"
                    };

                    await EnrichGameEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<List<MetricEntity>> ExtractMetricEntitiesAsync(string query)
        {
            var entities = new List<MetricEntity>();

            foreach (var pattern in _metricPatterns)
            {
                var matches = pattern.Value.Matches(query);
                foreach (Match match in matches)
                {
                    var entity = new MetricEntity
                    {
                        Text = match.Value,
                        EntityType = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = CalculateMetricEntityConfidence(match.Value, pattern.Key),
                        Source = "MetricPatterns"
                    };

                    await EnrichMetricEntity(entity);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        private Dictionary<GamblingEntityType, Regex> InitializeGamblingPatterns()
        {
            return new Dictionary<GamblingEntityType, Regex>
            {
                { GamblingEntityType.GGR, new Regex(@"\b(ggr|gross\s+gaming\s+revenue)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.NGR, new Regex(@"\b(ngr|net\s+gaming\s+revenue)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.RTP, new Regex(@"\b(rtp|return\s+to\s+player)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.HoldPercentage, new Regex(@"\b(hold\s+percentage|hold\s+%|house\s+advantage)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.HouseEdge, new Regex(@"\b(house\s+edge|casino\s+advantage)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.Bonus, new Regex(@"\b(bonus|bonuses|promotional\s+funds)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.Cashback, new Regex(@"\b(cashback|cash\s+back|rebate)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GamblingEntityType.Commission, new Regex(@"\b(commission|rake|fee)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        private Dictionary<TemporalType, Regex> InitializeTemporalPatterns()
        {
            return new Dictionary<TemporalType, Regex>
            {
                { TemporalType.Relative, new Regex(@"\b(yesterday|today|tomorrow|last\s+(week|month|year|quarter)|this\s+(week|month|year|quarter)|next\s+(week|month|year|quarter))\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { TemporalType.Date, new Regex(@"\b\d{1,2}[/-]\d{1,2}[/-]\d{2,4}\b|\b\d{4}-\d{2}-\d{2}\b", RegexOptions.Compiled) },
                { TemporalType.DateRange, new Regex(@"\b(from|between)\s+.+?\s+(to|and)\s+.+?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { TemporalType.Period, new Regex(@"\b(daily|weekly|monthly|quarterly|yearly|annual)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        private Dictionary<FinancialEntityType, Regex> InitializeFinancialPatterns()
        {
            return new Dictionary<FinancialEntityType, Regex>
            {
                { FinancialEntityType.Amount, new Regex(@"\b\$?\d+(?:,\d{3})*(?:\.\d{2})?\b|\b\d+\s*(gbp|usd|eur|pounds?|dollars?|euros?)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Currency, new Regex(@"\b(gbp|usd|eur|pounds?|dollars?|euros?|currency)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Deposit, new Regex(@"\b(deposit|deposits|deposited|funding)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Withdrawal, new Regex(@"\b(withdrawal|withdrawals|withdraw|payout|cashout)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Bet, new Regex(@"\b(bet|bets|wager|wagers|stake|stakes)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Win, new Regex(@"\b(win|wins|winnings|payout|payouts)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Bonus, new Regex(@"\b(bonus|bonuses|promotion|promotional)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { FinancialEntityType.Fee, new Regex(@"\b(fee|fees|charge|charges|commission)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        private Dictionary<PlayerEntityType, Regex> InitializePlayerPatterns()
        {
            return new Dictionary<PlayerEntityType, Regex>
            {
                { PlayerEntityType.Individual, new Regex(@"\bplayer\s+id\s*:?\s*\d+\b|\bcustomer\s+id\s*:?\s*\d+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { PlayerEntityType.Segment, new Regex(@"\b(vip|high\s*roller|whale|premium|bronze|silver|gold|platinum)\s*(player|customer|member)s?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { PlayerEntityType.Type, new Regex(@"\b(new|existing|active|inactive|dormant|churned)\s*(player|customer|member)s?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { PlayerEntityType.Status, new Regex(@"\b(registered|verified|suspended|blocked|self\s*excluded)\s*(player|customer|member)s?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { PlayerEntityType.Attribute, new Regex(@"\b(player|customer|member)s?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        private Dictionary<GameEntityType, Regex> InitializeGamePatterns()
        {
            return new Dictionary<GameEntityType, Regex>
            {
                { GameEntityType.Category, new Regex(@"\b(casino|table|card|slot|sports?|live)\s*games?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GameEntityType.Type, new Regex(@"\b(slot|slots|blackjack|poker|roulette|baccarat|craps|keno|lottery)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GameEntityType.Specific, new Regex(@"\bgame\s+id\s*:?\s*\d+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GameEntityType.Provider, new Regex(@"\b(netent|microgaming|playtech|evolution|pragmatic)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { GameEntityType.Platform, new Regex(@"\b(mobile|desktop|tablet|web|app)\s*games?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        private Dictionary<MetricEntityType, Regex> InitializeMetricPatterns()
        {
            return new Dictionary<MetricEntityType, Regex>
            {
                { MetricEntityType.Revenue, new Regex(@"\b(revenue|income|earnings|profit|loss)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { MetricEntityType.Volume, new Regex(@"\b(volume|turnover|activity|transactions?)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { MetricEntityType.Count, new Regex(@"\b(count|number|total|sum)\s+of\b|\bhow\s+many\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { MetricEntityType.Average, new Regex(@"\b(average|avg|mean)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { MetricEntityType.Percentage, new Regex(@"\b(percentage|percent|%|rate)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) },
                { MetricEntityType.Ratio, new Regex(@"\b(ratio|proportion|per)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled) }
            };
        }

        // Confidence calculation methods
        private float CalculateGamblingEntityConfidence(string text, GamblingEntityType type)
        {
            var baseConfidence = 0.8f;

            // Exact matches get higher confidence
            if (text.ToLowerInvariant() == type.ToString().ToLowerInvariant())
                return 0.95f;

            // Acronyms get high confidence
            if (text.Length <= 4 && text.All(char.IsUpper))
                return 0.9f;

            return baseConfidence;
        }

        private float CalculateTemporalEntityConfidence(string text, TemporalType type)
        {
            return type switch
            {
                TemporalType.Date => 0.95f,
                TemporalType.Relative => 0.85f,
                TemporalType.DateRange => 0.9f,
                TemporalType.Period => 0.8f,
                _ => 0.7f
            };
        }

        private float CalculateFinancialEntityConfidence(string text, FinancialEntityType type)
        {
            if (Regex.IsMatch(text, @"\$?\d+(?:,\d{3})*(?:\.\d{2})?"))
                return 0.95f;

            return 0.8f;
        }

        private float CalculatePlayerEntityConfidence(string text, PlayerEntityType type)
        {
            if (text.Contains("id", StringComparison.OrdinalIgnoreCase))
                return 0.95f;

            return 0.8f;
        }

        private float CalculateGameEntityConfidence(string text, GameEntityType type)
        {
            if (text.Contains("id", StringComparison.OrdinalIgnoreCase))
                return 0.95f;

            return 0.8f;
        }

        private float CalculateMetricEntityConfidence(string text, MetricEntityType type)
        {
            return 0.8f;
        }

        // Entity enrichment methods
        private async Task EnrichGamblingEntity(GamblingEntity entity)
        {
            var terms = await _domainKnowledge.GetGamblingTermsAsync();
            if (terms.TryGetValue(entity.Text.ToLowerInvariant(), out var definition))
            {
                entity.StandardTerm = definition.Term;
                entity.Definition = definition.Definition;
                entity.RelatedTables = definition.RelatedTables;
                entity.RelatedColumns = definition.RelatedColumns;
                entity.CalculationFormula = definition.CalculationFormula;
            }
        }

        private async Task ResolveTemporalEntity(TemporalEntity entity)
        {
            var text = entity.Text.ToLowerInvariant();
            var now = DateTime.UtcNow;

            entity.IsRelative = entity.TemporalType == TemporalType.Relative;

            switch (text)
            {
                case "yesterday":
                    entity.StartDate = now.AddDays(-1).Date;
                    entity.EndDate = entity.StartDate;
                    entity.SqlExpression = "DATEADD(day, -1, CAST(GETDATE() AS DATE))";
                    entity.Granularity = "day";
                    break;
                case "today":
                    entity.StartDate = now.Date;
                    entity.EndDate = entity.StartDate;
                    entity.SqlExpression = "CAST(GETDATE() AS DATE)";
                    entity.Granularity = "day";
                    break;
                case var t when t.Contains("last week"):
                    entity.StartDate = now.AddDays(-7).Date;
                    entity.EndDate = now.Date;
                    entity.SqlExpression = "DATEADD(week, -1, GETDATE())";
                    entity.Granularity = "week";
                    break;
                case var t when t.Contains("last month"):
                    entity.StartDate = now.AddMonths(-1).Date;
                    entity.EndDate = now.Date;
                    entity.SqlExpression = "DATEADD(month, -1, GETDATE())";
                    entity.Granularity = "month";
                    break;
                case var t when t.Contains("last year"):
                    entity.StartDate = now.AddYears(-1).Date;
                    entity.EndDate = now.Date;
                    entity.SqlExpression = "DATEADD(year, -1, GETDATE())";
                    entity.Granularity = "year";
                    break;
            }

            await Task.CompletedTask;
        }

        private async Task EnrichFinancialEntity(FinancialEntity entity)
        {
            // Extract amount if present
            var amountMatch = Regex.Match(entity.Text, @"\$?(\d+(?:,\d{3})*(?:\.\d{2})?)");
            if (amountMatch.Success && decimal.TryParse(amountMatch.Groups[1].Value.Replace(",", ""), out var amount))
            {
                entity.Amount = amount;
            }

            // Extract currency
            var currencyMatch = Regex.Match(entity.Text, @"\b(gbp|usd|eur|pounds?|dollars?|euros?)\b", RegexOptions.IgnoreCase);
            if (currencyMatch.Success)
            {
                entity.Currency = currencyMatch.Value.ToUpperInvariant();
            }

            await Task.CompletedTask;
        }

        private async Task EnrichPlayerEntity(PlayerEntity entity)
        {
            var text = entity.Text.ToLowerInvariant();

            if (text.Contains("vip") || text.Contains("high roller") || text.Contains("whale"))
                entity.PlayerSegment = "VIP";
            else if (text.Contains("new"))
                entity.PlayerType = "New";
            else if (text.Contains("active"))
                entity.PlayerType = "Active";
            else if (text.Contains("inactive"))
                entity.PlayerType = "Inactive";

            // Extract player ID if present
            var idMatch = Regex.Match(entity.Text, @"id\s*:?\s*(\d+)", RegexOptions.IgnoreCase);
            if (idMatch.Success && int.TryParse(idMatch.Groups[1].Value, out var playerId))
            {
                entity.SpecificPlayerId = playerId;
            }

            await Task.CompletedTask;
        }

        private async Task EnrichGameEntity(GameEntity entity)
        {
            var text = entity.Text.ToLowerInvariant();

            if (text.Contains("slot"))
                entity.GameCategory = "Slots";
            else if (text.Contains("table") || text.Contains("blackjack") || text.Contains("poker"))
                entity.GameCategory = "Table Games";
            else if (text.Contains("sports"))
                entity.GameCategory = "Sports Betting";
            else if (text.Contains("live"))
                entity.GameCategory = "Live Casino";

            // Extract game ID if present
            var idMatch = Regex.Match(entity.Text, @"id\s*:?\s*(\d+)", RegexOptions.IgnoreCase);
            if (idMatch.Success && int.TryParse(idMatch.Groups[1].Value, out var gameId))
            {
                entity.SpecificGameId = gameId;
            }

            await Task.CompletedTask;
        }

        private async Task EnrichMetricEntity(MetricEntity entity)
        {
            var text = entity.Text.ToLowerInvariant();

            if (text.Contains("sum") || text.Contains("total"))
                entity.AggregationType = "SUM";
            else if (text.Contains("count") || text.Contains("number"))
                entity.AggregationType = "COUNT";
            else if (text.Contains("average") || text.Contains("avg"))
                entity.AggregationType = "AVG";
            else if (text.Contains("max") || text.Contains("highest"))
                entity.AggregationType = "MAX";
            else if (text.Contains("min") || text.Contains("lowest"))
                entity.AggregationType = "MIN";

            await Task.CompletedTask;
        }
    }
}
