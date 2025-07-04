using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NL2SQL.Core.Models;
using NL2SQL.Core.Models.Enhanced;
using Microsoft.Extensions.Logging;

namespace NL2SQL.Core.Services.Enhanced
{
    /// <summary>
    /// Advanced query analyzer with gambling domain expertise
    /// </summary>
    public class AdvancedQueryAnalyzer : IAdvancedQueryAnalyzer
    {
        private readonly ILogger<AdvancedQueryAnalyzer> _logger;
        private readonly IGamblingDomainService _gamblingDomainService;
        private readonly ISemanticAnalysisService _semanticAnalysisService;
        private readonly ITemporalAnalysisService _temporalAnalysisService;

        // Gambling-specific patterns
        private readonly Dictionary<string, Models.QueryIntent> _intentPatterns = new()
        {
            { @"\b(show|display|list|get)\b.*\b(top|best|highest|most)\b", Models.QueryIntent.TopN },
            { @"\b(total|sum|aggregate|calculate)\b", Models.QueryIntent.Aggregate },
            { @"\b(trend|over time|daily|monthly|yearly)\b", Models.QueryIntent.Trend },
            { @"\b(compare|vs|versus|against)\b", Models.QueryIntent.Comparison },
            { @"\b(distribution|breakdown|split|segment)\b", Models.QueryIntent.Distribution },
            { @"\b(correlation|relationship|related)\b", Models.QueryIntent.Correlation },
            { @"\b(predict|forecast|estimate)\b", Models.QueryIntent.Forecast },
            { @"\b(anomaly|unusual|outlier|strange)\b", Models.QueryIntent.Anomaly },
            { @"\b(drill|detail|breakdown|deep)\b", Models.QueryIntent.Drill }
        };

        // Gambling domain entities
        private readonly Dictionary<string, string> _gamblingEntities = new()
        {
            { @"\b(player|customer|user|gambler)s?\b", "Player" },
            { @"\b(game|slot|casino|poker|blackjack|roulette)s?\b", "Game" },
            { @"\b(bet|wager|stake)s?\b", "Bet" },
            { @"\b(deposit|funding|payment)s?\b", "Deposit" },
            { @"\b(withdrawal|cashout|payout)s?\b", "Withdrawal" },
            { @"\b(bonus|promotion|offer)s?\b", "Bonus" },
            { @"\b(revenue|income|profit|loss)s?\b", "Revenue" },
            { @"\b(session|round|spin)s?\b", "Session" },
            { @"\b(country|region|jurisdiction)s?\b", "Geography" },
            { @"\b(brand|label|operator)s?\b", "Brand" }
        };

        // Gambling metrics patterns
        private readonly Dictionary<string, string> _gamblingMetrics = new()
        {
            { @"\bggr\b|\bgross gaming revenue\b", "GGR" },
            { @"\bngr\b|\bnet gaming revenue\b", "NGR" },
            { @"\brtp\b|\breturn to player\b", "RTP" },
            { @"\bhouse edge\b", "HouseEdge" },
            { @"\bltv\b|\blifetime value\b", "LTV" },
            { @"\barpu\b|\baverage revenue per user\b", "ARPU" },
            { @"\bcac\b|\bcustomer acquisition cost\b", "CAC" },
            { @"\bdau\b|\bdaily active users\b", "DAU" },
            { @"\bmau\b|\bmonthly active users\b", "MAU" },
            { @"\bchurn rate\b|\bchurn\b", "ChurnRate" }
        };

        // Temporal expressions
        private readonly Dictionary<string, TemporalExpression> _temporalExpressions = new()
        {
            { @"\btoday\b", new TemporalExpression { Type = TemporalType.Absolute, Value = "CURRENT_DATE" } },
            { @"\byesterday\b", new TemporalExpression { Type = TemporalType.Relative, Value = "DATEADD(day, -1, CURRENT_DATE)" } },
            { @"\blast month\b", new TemporalExpression { Type = TemporalType.Relative, Value = "DATEADD(month, -1, CURRENT_DATE)" } },
            { @"\bthis month\b", new TemporalExpression { Type = TemporalType.Relative, Value = "MONTH(CURRENT_DATE) = MONTH(GETDATE())" } },
            { @"\blast year\b", new TemporalExpression { Type = TemporalType.Relative, Value = "YEAR(DATEADD(year, -1, CURRENT_DATE))" } },
            { @"\bthis year\b", new TemporalExpression { Type = TemporalType.Relative, Value = "YEAR(CURRENT_DATE) = YEAR(GETDATE())" } },
            { @"\blast week\b", new TemporalExpression { Type = TemporalType.Relative, Value = "DATEADD(week, -1, CURRENT_DATE)" } },
            { @"\bthis week\b", new TemporalExpression { Type = TemporalType.Relative, Value = "DATEPART(week, CURRENT_DATE) = DATEPART(week, GETDATE())" } }
        };

        public AdvancedQueryAnalyzer(
            ILogger<AdvancedQueryAnalyzer> logger,
            IGamblingDomainService gamblingDomainService,
            ISemanticAnalysisService semanticAnalysisService,
            ITemporalAnalysisService temporalAnalysisService)
        {
            _logger = logger;
            _gamblingDomainService = gamblingDomainService;
            _semanticAnalysisService = semanticAnalysisService;
            _temporalAnalysisService = temporalAnalysisService;
        }

        public async Task<EnhancedQueryAnalysis> AnalyzeAsync(string query, QueryContext context)
        {
            _logger.LogInformation("Starting advanced analysis for query: {Query}", query);

            var analysis = new EnhancedQueryAnalysis
            {
                OriginalQuery = query,
                NormalizedQuery = NormalizeQuery(query),
                QueryContext = context,
                AnalysisTimestamp = DateTime.UtcNow
            };

            // Parallel analysis tasks
            var tasks = new List<Task>
            {
                Task.Run(async () => analysis.Intent = await DetectIntentAsync(query)),
                Task.Run(async () => analysis.Entities = await ExtractEntitiesAsync(query)),
                Task.Run(async () => analysis.TemporalContext = await ExtractTemporalContextAsync(query)),
                Task.Run(async () => analysis.BusinessConcepts = await MapToBusinessConceptsAsync(query)),
                Task.Run(async () => analysis.GamblingMetrics = await ExtractGamblingMetricsAsync(query)),
                Task.Run(async () => analysis.AggregationRequirements = await DetectAggregationRequirementsAsync(query)),
                Task.Run(async () => analysis.FilterRequirements = await ExtractFilterRequirementsAsync(query)),
                Task.Run(async () => analysis.SortingRequirements = await ExtractSortingRequirementsAsync(query))
            };

            await Task.WhenAll(tasks);

            // Post-processing
            analysis.Complexity = CalculateComplexity(analysis);
            analysis.Confidence = CalculateConfidence(analysis);
            analysis.Ambiguities = await DetectAmbiguitiesAsync(analysis);
            analysis.RecommendedTables = await RecommendTablesAsync(analysis);
            analysis.RequiredJoins = await IdentifyRequiredJoinsAsync(analysis);

            _logger.LogInformation("Analysis completed with confidence: {Confidence}", analysis.Confidence);
            return analysis;
        }

        public async Task<List<EntityMention>> ExtractEntitiesAsync(string query)
        {
            var entities = new List<EntityMention>();
            var normalizedQuery = query.ToLowerInvariant();

            foreach (var pattern in _gamblingEntities)
            {
                var matches = Regex.Matches(normalizedQuery, pattern.Key, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    entities.Add(new EntityMention
                    {
                        Text = match.Value,
                        EntityType = pattern.Value,
                        StartPosition = match.Index,
                        EndPosition = match.Index + match.Length,
                        Confidence = 0.9f,
                        Source = "Pattern Matching"
                    });
                }
            }

            // Use semantic analysis for additional entity extraction
            var semanticEntities = await _semanticAnalysisService.ExtractEntitiesAsync(query);
            entities.AddRange(semanticEntities);

            return entities.DistinctBy(e => new { e.Text, e.EntityType }).ToList();
        }

        public async Task<Models.QueryIntent> DetectIntentAsync(string query)
        {
            var normalizedQuery = query.ToLowerInvariant();

            foreach (var pattern in _intentPatterns)
            {
                if (Regex.IsMatch(normalizedQuery, pattern.Key, RegexOptions.IgnoreCase))
                {
                    return pattern.Value;
                }
            }

            // Use semantic analysis for intent detection
            return await _semanticAnalysisService.DetectIntentAsync(query);
        }

        public async Task<TemporalContext> ExtractTemporalContextAsync(string query)
        {
            return await _temporalAnalysisService.ExtractTemporalContextAsync(query, _temporalExpressions);
        }

        public async Task<List<BusinessConcept>> MapToBusinessConceptsAsync(string query)
        {
            var concepts = new List<BusinessConcept>();

            // Extract gambling-specific concepts
            foreach (var metric in _gamblingMetrics)
            {
                if (Regex.IsMatch(query, metric.Key, RegexOptions.IgnoreCase))
                {
                    var concept = await _gamblingDomainService.GetBusinessConceptAsync(metric.Value);
                    if (concept != null)
                    {
                        concepts.Add(concept);
                    }
                }
            }

            // Use semantic analysis for additional concept mapping
            var semanticConcepts = await _semanticAnalysisService.MapToBusinessConceptsAsync(query);
            concepts.AddRange(semanticConcepts);

            return concepts.DistinctBy(c => c.ConceptId).ToList();
        }

        private async Task<List<GamblingMetric>> ExtractGamblingMetricsAsync(string query)
        {
            var metrics = new List<GamblingMetric>();
            var normalizedQuery = query.ToLowerInvariant();

            foreach (var metricPattern in _gamblingMetrics)
            {
                if (Regex.IsMatch(normalizedQuery, metricPattern.Key, RegexOptions.IgnoreCase))
                {
                    var metric = await _gamblingDomainService.GetMetricDefinitionAsync(metricPattern.Value);
                    if (metric != null)
                    {
                        metrics.Add(metric);
                    }
                }
            }

            return metrics;
        }

        private async Task<AggregationRequirements> DetectAggregationRequirementsAsync(string query)
        {
            var requirements = new AggregationRequirements();
            var normalizedQuery = query.ToLowerInvariant();

            // Detect aggregation functions
            if (Regex.IsMatch(normalizedQuery, @"\b(total|sum)\b", RegexOptions.IgnoreCase))
                requirements.Functions.Add("SUM");
            if (Regex.IsMatch(normalizedQuery, @"\b(average|avg|mean)\b", RegexOptions.IgnoreCase))
                requirements.Functions.Add("AVG");
            if (Regex.IsMatch(normalizedQuery, @"\b(count|number of)\b", RegexOptions.IgnoreCase))
                requirements.Functions.Add("COUNT");
            if (Regex.IsMatch(normalizedQuery, @"\b(max|maximum|highest)\b", RegexOptions.IgnoreCase))
                requirements.Functions.Add("MAX");
            if (Regex.IsMatch(normalizedQuery, @"\b(min|minimum|lowest)\b", RegexOptions.IgnoreCase))
                requirements.Functions.Add("MIN");

            // Detect grouping requirements
            if (Regex.IsMatch(normalizedQuery, @"\bby\s+(\w+)", RegexOptions.IgnoreCase))
            {
                var matches = Regex.Matches(normalizedQuery, @"\bby\s+(\w+)", RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    requirements.GroupByColumns.Add(match.Groups[1].Value);
                }
            }

            return requirements;
        }

        private async Task<FilterRequirements> ExtractFilterRequirementsAsync(string query)
        {
            var requirements = new FilterRequirements();
            var normalizedQuery = query.ToLowerInvariant();

            // Extract WHERE conditions
            var wherePatterns = new[]
            {
                @"\bwhere\s+(.+?)(?:\s+group\s+by|\s+order\s+by|\s+having|$)",
                @"\bfor\s+(.+?)(?:\s+group\s+by|\s+order\s+by|\s+having|$)",
                @"\bin\s+(.+?)(?:\s+group\s+by|\s+order\s+by|\s+having|$)"
            };

            foreach (var pattern in wherePatterns)
            {
                var matches = Regex.Matches(normalizedQuery, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    requirements.Conditions.Add(match.Groups[1].Value.Trim());
                }
            }

            return requirements;
        }

        private async Task<SortingRequirements> ExtractSortingRequirementsAsync(string query)
        {
            var requirements = new SortingRequirements();
            var normalizedQuery = query.ToLowerInvariant();

            // Detect ORDER BY requirements
            if (Regex.IsMatch(normalizedQuery, @"\b(top|highest|best|most)\b", RegexOptions.IgnoreCase))
            {
                requirements.Direction = "DESC";
            }
            else if (Regex.IsMatch(normalizedQuery, @"\b(bottom|lowest|least|worst)\b", RegexOptions.IgnoreCase))
            {
                requirements.Direction = "ASC";
            }

            // Extract TOP N requirements
            var topMatch = Regex.Match(normalizedQuery, @"\btop\s+(\d+)", RegexOptions.IgnoreCase);
            if (topMatch.Success)
            {
                requirements.Limit = int.Parse(topMatch.Groups[1].Value);
            }

            return requirements;
        }

        private Models.QueryComplexity CalculateComplexity(EnhancedQueryAnalysis analysis)
        {
            var score = 0;

            // Base complexity factors
            score += analysis.Entities.Count;
            score += analysis.BusinessConcepts.Count * 2;
            score += analysis.GamblingMetrics.Count * 3;
            score += analysis.AggregationRequirements.Functions.Count * 2;
            score += analysis.FilterRequirements.Conditions.Count;
            score += analysis.RequiredJoins.Count * 3;

            // Temporal complexity
            if (analysis.TemporalContext.HasTemporalElements)
                score += 2;

            return score switch
            {
                <= 5 => Models.QueryComplexity.Simple,
                <= 10 => Models.QueryComplexity.Medium,
                <= 20 => Models.QueryComplexity.Complex,
                _ => Models.QueryComplexity.VeryComplex
            };
        }

        private float CalculateConfidence(EnhancedQueryAnalysis analysis)
        {
            var factors = new List<float>();

            // Intent confidence
            factors.Add(analysis.Intent != QueryIntent.Select ? 0.8f : 0.6f);

            // Entity extraction confidence
            var entityConfidence = analysis.Entities.Any() ? analysis.Entities.Average(e => e.Confidence) : 0.5f;
            factors.Add(entityConfidence);

            // Business concept confidence
            var conceptConfidence = analysis.BusinessConcepts.Any() ? analysis.BusinessConcepts.Average(c => c.Confidence) : 0.5f;
            factors.Add(conceptConfidence);

            // Temporal confidence
            factors.Add(analysis.TemporalContext.HasTemporalElements ? 0.9f : 0.7f);

            return factors.Average();
        }

        private async Task<List<QueryAmbiguity>> DetectAmbiguitiesAsync(EnhancedQueryAnalysis analysis)
        {
            var ambiguities = new List<QueryAmbiguity>();

            // Check for ambiguous terms
            var ambiguousTerms = await _gamblingDomainService.GetAmbiguousTermsAsync(analysis.OriginalQuery);
            ambiguities.AddRange(ambiguousTerms.Select(term => new QueryAmbiguity
            {
                Type = AmbiguityType.TermAmbiguity,
                Description = $"Term '{term}' has multiple meanings",
                Suggestions = new List<string> { $"Clarify the meaning of '{term}'" }
            }));

            return ambiguities;
        }

        private async Task<List<string>> RecommendTablesAsync(EnhancedQueryAnalysis analysis)
        {
            var recommendedTables = new List<string>();

            // Based on entities and concepts
            foreach (var entity in analysis.Entities)
            {
                var tables = await _gamblingDomainService.GetTablesForEntityAsync(entity.EntityType);
                recommendedTables.AddRange(tables);
            }

            foreach (var concept in analysis.BusinessConcepts)
            {
                var tables = await _gamblingDomainService.GetTablesForConceptAsync(concept.ConceptId);
                recommendedTables.AddRange(tables);
            }

            return recommendedTables.Distinct().ToList();
        }

        private async Task<List<JoinRequirement>> IdentifyRequiredJoinsAsync(EnhancedQueryAnalysis analysis)
        {
            var joins = new List<JoinRequirement>();

            if (analysis.RecommendedTables.Count > 1)
            {
                var joinPaths = await _gamblingDomainService.GetOptimalJoinPathsAsync(analysis.RecommendedTables);
                joins.AddRange(joinPaths);
            }

            return joins;
        }

        private string NormalizeQuery(string query)
        {
            // Basic normalization
            return query.Trim()
                       .Replace("  ", " ")
                       .Replace("\n", " ")
                       .Replace("\r", " ");
        }
    }
}
