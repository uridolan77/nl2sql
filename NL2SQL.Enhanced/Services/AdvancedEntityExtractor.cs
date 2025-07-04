using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NL2SQL.Enhanced.Models;

namespace NL2SQL.Enhanced.Services
{
    /// <summary>
    /// Advanced entity extraction service using NLP techniques and gambling domain knowledge
    /// </summary>
    public class AdvancedEntityExtractor : IEntityExtractor
    {
        private readonly ILogger<AdvancedEntityExtractor> _logger;
        private readonly Dictionary<string, EntityType> _gamblingTerms;
        private readonly Dictionary<string, TemporalPattern> _temporalPatterns;
        private readonly Dictionary<string, MetricPattern> _metricPatterns;

        public AdvancedEntityExtractor(ILogger<AdvancedEntityExtractor> logger)
        {
            _logger = logger;
            _gamblingTerms = InitializeGamblingTerms();
            _temporalPatterns = InitializeTemporalPatterns();
            _metricPatterns = InitializeMetricPatterns();
        }

        public async Task<EntityExtractionResult> ExtractEntitiesAsync(string query)
        {
            _logger.LogInformation("Extracting entities from query: {Query}", query);

            var result = new EntityExtractionResult
            {
                OriginalQuery = query,
                Entities = new List<ExtractedEntity>(),
                TemporalExpressions = new List<TemporalExpression>(),
                MetricReferences = new List<MetricReference>(),
                PlayerReferences = new List<PlayerReference>(),
                GameReferences = new List<GameReference>()
            };

            // Extract different types of entities
            await ExtractGamblingEntitiesAsync(query, result);
            await ExtractTemporalEntitiesAsync(query, result);
            await ExtractMetricEntitiesAsync(query, result);
            await ExtractPlayerEntitiesAsync(query, result);
            await ExtractGameEntitiesAsync(query, result);
            await ExtractFinancialEntitiesAsync(query, result);

            _logger.LogInformation("Extracted {Count} entities from query", result.Entities.Count);
            return result;
        }

        private async Task ExtractGamblingEntitiesAsync(string query, EntityExtractionResult result)
        {
            var queryLower = query.ToLowerInvariant();

            foreach (var term in _gamblingTerms)
            {
                if (queryLower.Contains(term.Key))
                {
                    var entity = new ExtractedEntity
                    {
                        Text = term.Key,
                        EntityType = term.Value,
                        StartPosition = queryLower.IndexOf(term.Key),
                        EndPosition = queryLower.IndexOf(term.Key) + term.Key.Length,
                        Confidence = 0.9f,
                        Source = "GamblingTermsDictionary"
                    };
                    result.Entities.Add(entity);
                }
            }

            await Task.CompletedTask;
        }

        private async Task ExtractTemporalEntitiesAsync(string query, EntityExtractionResult result)
        {
            foreach (var pattern in _temporalPatterns)
            {
                var matches = Regex.Matches(query, pattern.Value.Pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var temporal = new TemporalExpression
                    {
                        Text = match.Value,
                        TemporalType = pattern.Value.Type,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = pattern.Value.Confidence,
                        ResolvedValue = await ResolveTemporalExpression(match.Value, pattern.Value.Type)
                    };
                    result.TemporalExpressions.Add(temporal);

                    // Also add as general entity
                    result.Entities.Add(new ExtractedEntity
                    {
                        Text = match.Value,
                        EntityType = EntityType.Temporal,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = pattern.Value.Confidence,
                        Source = "TemporalPatterns"
                    });
                }
            }
        }

        private async Task ExtractMetricEntitiesAsync(string query, EntityExtractionResult result)
        {
            foreach (var pattern in _metricPatterns)
            {
                var matches = Regex.Matches(query, pattern.Value.Pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var metric = new MetricReference
                    {
                        Text = match.Value,
                        MetricType = pattern.Value.Type,
                        MetricName = pattern.Key,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = pattern.Value.Confidence,
                        RequiredTables = pattern.Value.RequiredTables,
                        CalculationHint = pattern.Value.CalculationHint
                    };
                    result.MetricReferences.Add(metric);

                    // Also add as general entity
                    result.Entities.Add(new ExtractedEntity
                    {
                        Text = match.Value,
                        EntityType = EntityType.Metric,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = pattern.Value.Confidence,
                        Source = "MetricPatterns"
                    });
                }
            }

            await Task.CompletedTask;
        }

        private async Task ExtractPlayerEntitiesAsync(string query, EntityExtractionResult result)
        {
            var playerPatterns = new[]
            {
                @"\b(player|customer|user|member)s?\b",
                @"\b(vip|high.?roller|whale)s?\b",
                @"\b(new|existing|active|inactive)\s+(player|customer|user|member)s?\b",
                @"\bplayer\s+id\s*:?\s*(\d+)\b",
                @"\bcustomer\s+id\s*:?\s*(\d+)\b"
            };

            foreach (var pattern in playerPatterns)
            {
                var matches = Regex.Matches(query, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var playerRef = new PlayerReference
                    {
                        Text = match.Value,
                        PlayerType = DeterminePlayerType(match.Value),
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.8f,
                        SpecificId = ExtractPlayerIdFromMatch(match)
                    };
                    result.PlayerReferences.Add(playerRef);

                    result.Entities.Add(new ExtractedEntity
                    {
                        Text = match.Value,
                        EntityType = EntityType.Player,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.8f,
                        Source = "PlayerPatterns"
                    });
                }
            }

            await Task.CompletedTask;
        }

        private async Task ExtractGameEntitiesAsync(string query, EntityExtractionResult result)
        {
            var gamePatterns = new[]
            {
                @"\b(slot|slots|slot\s+machine)s?\b",
                @"\b(blackjack|poker|roulette|baccarat)\b",
                @"\b(casino|table)\s+games?\b",
                @"\b(live|virtual)\s+games?\b",
                @"\b(sports?\s+betting|sportsbook)\b",
                @"\bgame\s+id\s*:?\s*(\d+)\b"
            };

            foreach (var pattern in gamePatterns)
            {
                var matches = Regex.Matches(query, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var gameRef = new GameReference
                    {
                        Text = match.Value,
                        GameType = DetermineGameType(match.Value),
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.8f,
                        SpecificId = ExtractGameIdFromMatch(match)
                    };
                    result.GameReferences.Add(gameRef);

                    result.Entities.Add(new ExtractedEntity
                    {
                        Text = match.Value,
                        EntityType = EntityType.Game,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.8f,
                        Source = "GamePatterns"
                    });
                }
            }

            await Task.CompletedTask;
        }

        private async Task ExtractFinancialEntitiesAsync(string query, EntityExtractionResult result)
        {
            var financialPatterns = new[]
            {
                @"\b(deposit|withdrawal|transaction)s?\b",
                @"\b(amount|sum|total|value)\b",
                @"\b(currency|gbp|usd|eur)\b",
                @"\b\$?\d+(?:,\d{3})*(?:\.\d{2})?\b",
                @"\b(bonus|promotion|free\s+bet)s?\b"
            };

            foreach (var pattern in financialPatterns)
            {
                var matches = Regex.Matches(query, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    result.Entities.Add(new ExtractedEntity
                    {
                        Text = match.Value,
                        EntityType = EntityType.Financial,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.7f,
                        Source = "FinancialPatterns"
                    });
                }
            }

            await Task.CompletedTask;
        }

        private Dictionary<string, EntityType> InitializeGamblingTerms()
        {
            return new Dictionary<string, EntityType>(StringComparer.OrdinalIgnoreCase)
            {
                // Revenue Metrics
                { "ggr", EntityType.Metric },
                { "gross gaming revenue", EntityType.Metric },
                { "ngr", EntityType.Metric },
                { "net gaming revenue", EntityType.Metric },
                { "rtp", EntityType.Metric },
                { "return to player", EntityType.Metric },
                { "hold percentage", EntityType.Metric },
                { "house edge", EntityType.Metric },

                // Player Types
                { "vip", EntityType.Player },
                { "high roller", EntityType.Player },
                { "whale", EntityType.Player },
                { "new player", EntityType.Player },
                { "active player", EntityType.Player },

                // Game Types
                { "slot", EntityType.Game },
                { "blackjack", EntityType.Game },
                { "poker", EntityType.Game },
                { "roulette", EntityType.Game },
                { "baccarat", EntityType.Game },
                { "sports betting", EntityType.Game },

                // Financial Terms
                { "deposit", EntityType.Financial },
                { "withdrawal", EntityType.Financial },
                { "bonus", EntityType.Financial },
                { "cashback", EntityType.Financial },
                { "commission", EntityType.Financial }
            };
        }
