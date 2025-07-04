using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;
using NL2SQL.Core.Models;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Advanced NLP pipeline that combines entity extraction, semantic search, and domain knowledge
    /// </summary>
    public class AdvancedNLPPipeline : IAdvancedNLPPipeline
    {
        private readonly ILogger<AdvancedNLPPipeline> _logger;
        private readonly IAdvancedEntityExtractor _entityExtractor;
        private readonly ISemanticSearchService _semanticSearch;
        private readonly IGamblingDomainKnowledge _domainKnowledge;

        public AdvancedNLPPipeline(
            ILogger<AdvancedNLPPipeline> logger,
            IAdvancedEntityExtractor entityExtractor,
            ISemanticSearchService semanticSearch,
            IGamblingDomainKnowledge domainKnowledge)
        {
            _logger = logger;
            _entityExtractor = entityExtractor;
            _semanticSearch = semanticSearch;
            _domainKnowledge = domainKnowledge;
        }

        public async Task<NLPProcessingResult> ProcessQueryAsync(string query)
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Starting advanced NLP processing for query: {Query}", query);

            var result = new NLPProcessingResult
            {
                OriginalQuery = query,
                ProcessingSteps = new List<string>()
            };

            try
            {
                // Step 1: Extract entities
                result.ProcessingSteps.Add("Entity Extraction");
                result.EntityExtraction = await _entityExtractor.ExtractEntitiesAsync(query);
                _logger.LogDebug("Extracted {Count} entities", GetTotalEntityCount(result.EntityExtraction));

                // Step 2: Analyze intent
                result.ProcessingSteps.Add("Intent Analysis");
                result.IntentAnalysis = await AnalyzeIntentAsync(query);
                _logger.LogDebug("Detected intent: {Intent} with confidence {Confidence:F2}", 
                    result.IntentAnalysis.PrimaryIntent, result.IntentAnalysis.Confidence);

                // Step 3: Find similar tables
                result.ProcessingSteps.Add("Table Semantic Search");
                result.TableRecommendations = await _semanticSearch.FindSimilarTablesAsync(query, 5);
                _logger.LogDebug("Found {Count} table recommendations", result.TableRecommendations.Count);

                // Step 4: Find similar columns
                result.ProcessingSteps.Add("Column Semantic Search");
                result.ColumnRecommendations = await _semanticSearch.FindSimilarColumnsAsync(query, 10);
                _logger.LogDebug("Found {Count} column recommendations", result.ColumnRecommendations.Count);

                // Step 5: Calculate overall confidence
                result.OverallConfidence = CalculateOverallConfidence(result);

                result.ProcessingTime = DateTime.UtcNow - startTime;
                _logger.LogInformation("NLP processing completed in {Time}ms with confidence {Confidence:F2}", 
                    result.ProcessingTime.TotalMilliseconds, result.OverallConfidence);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during NLP processing");
                result.ProcessingTime = DateTime.UtcNow - startTime;
                result.OverallConfidence = 0f;
                return result;
            }
        }

        public async Task<QueryIntentAnalysis> AnalyzeIntentAsync(string query)
        {
            var queryLower = query.ToLowerInvariant();
            var analysis = new QueryIntentAnalysis
            {
                SecondaryIntents = new List<QueryIntent>(),
                RequiredActions = new List<string>()
            };

            // Primary intent detection with confidence scoring
            var intentScores = new Dictionary<QueryIntent, float>();

            // Aggregate intent patterns
            if (ContainsAggregatePatterns(queryLower))
            {
                intentScores[QueryIntent.Aggregate] = CalculateAggregateConfidence(queryLower);
            }

            // Select intent patterns
            if (ContainsSelectPatterns(queryLower))
            {
                intentScores[QueryIntent.Select] = CalculateSelectConfidence(queryLower);
            }

            // TopN intent patterns
            if (ContainsTopNPatterns(queryLower))
            {
                intentScores[QueryIntent.TopN] = CalculateTopNConfidence(queryLower);
            }

            // Trend intent patterns
            if (ContainsTrendPatterns(queryLower))
            {
                intentScores[QueryIntent.Trend] = CalculateTrendConfidence(queryLower);
            }

            // Comparison intent patterns
            if (ContainsComparisonPatterns(queryLower))
            {
                intentScores[QueryIntent.Comparison] = CalculateComparisonConfidence(queryLower);
            }

            // Determine primary and secondary intents
            var sortedIntents = intentScores.OrderByDescending(kvp => kvp.Value).ToList();
            
            if (sortedIntents.Any())
            {
                analysis.PrimaryIntent = sortedIntents.First().Key;
                analysis.Confidence = sortedIntents.First().Value;
                analysis.SecondaryIntents = sortedIntents.Skip(1).Where(kvp => kvp.Value > 0.3f).Select(kvp => kvp.Key).ToList();
            }
            else
            {
                analysis.PrimaryIntent = QueryIntent.Select;
                analysis.Confidence = 0.5f;
            }

            // Generate intent description and required actions
            analysis.IntentDescription = GenerateIntentDescription(analysis.PrimaryIntent, queryLower);
            analysis.RequiredActions = GenerateRequiredActions(analysis.PrimaryIntent, queryLower);

            await Task.CompletedTask;
            return analysis;
        }

        public async Task<NamedEntityResult> ExtractNamedEntitiesAsync(string query)
        {
            // This would integrate with spaCy or Stanford NLP in production
            // For now, we'll use our advanced entity extractor
            var entityResult = await _entityExtractor.ExtractEntitiesAsync(query);
            
            var namedEntities = new List<NamedEntity>();

            // Convert our entities to named entity format
            foreach (var entity in entityResult.GamblingEntities)
            {
                namedEntities.Add(new NamedEntity
                {
                    Text = entity.Text,
                    Label = $"GAMBLING_{entity.EntityType}",
                    StartChar = entity.StartPosition,
                    EndChar = entity.EndPosition,
                    Confidence = entity.Confidence
                });
            }

            foreach (var entity in entityResult.PlayerEntities)
            {
                namedEntities.Add(new NamedEntity
                {
                    Text = entity.Text,
                    Label = $"PLAYER_{entity.EntityType}",
                    StartChar = entity.StartPosition,
                    EndChar = entity.EndPosition,
                    Confidence = entity.Confidence
                });
            }

            foreach (var entity in entityResult.FinancialEntities)
            {
                namedEntities.Add(new NamedEntity
                {
                    Text = entity.Text,
                    Label = $"FINANCIAL_{entity.EntityType}",
                    StartChar = entity.StartPosition,
                    EndChar = entity.EndPosition,
                    Confidence = entity.Confidence
                });
            }

            return new NamedEntityResult
            {
                Entities = namedEntities,
                OverallConfidence = namedEntities.Any() ? namedEntities.Average(e => e.Confidence) : 0f,
                ModelUsed = "AdvancedEntityExtractor"
            };
        }

        public async Task<DependencyParseResult> ParseDependenciesAsync(string query)
        {
            // Simplified dependency parsing - in production would use spaCy or Stanford NLP
            var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select((word, index) => new Token
                {
                    Text = word,
                    Lemma = word.ToLowerInvariant(),
                    PartOfSpeech = DeterminePartOfSpeech(word),
                    Index = index
                }).ToList();

            var relations = new List<DependencyRelation>();

            // Simple heuristic-based dependency relations
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                var currentToken = tokens[i];
                var nextToken = tokens[i + 1];

                if (IsVerb(currentToken.PartOfSpeech) && IsNoun(nextToken.PartOfSpeech))
                {
                    relations.Add(new DependencyRelation
                    {
                        HeadIndex = i,
                        DependentIndex = i + 1,
                        RelationType = "dobj"
                    });
                }
                else if (IsAdjective(currentToken.PartOfSpeech) && IsNoun(nextToken.PartOfSpeech))
                {
                    relations.Add(new DependencyRelation
                    {
                        HeadIndex = i + 1,
                        DependentIndex = i,
                        RelationType = "amod"
                    });
                }
            }

            await Task.CompletedTask;
            return new DependencyParseResult
            {
                Tokens = tokens,
                Relations = relations
            };
        }

        public async Task<CoreferenceResult> ResolveCoreferencesAsync(string query)
        {
            // Simplified coreference resolution
            var chains = new List<CoreferenceChain>();
            var resolvedQuery = query;

            // Look for pronouns and resolve them to likely entities
            var pronouns = new[] { "it", "they", "them", "this", "that", "these", "those" };
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i].ToLowerInvariant().Trim('.', ',', '?', '!');
                if (pronouns.Contains(word))
                {
                    // Simple heuristic: resolve to the nearest noun before the pronoun
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var prevWord = words[j].ToLowerInvariant().Trim('.', ',', '?', '!');
                        if (IsLikelyEntity(prevWord))
                        {
                            var chain = new CoreferenceChain
                            {
                                ResolvedEntity = prevWord,
                                Mentions = new List<Mention>
                                {
                                    new Mention { Text = prevWord, StartChar = GetWordPosition(query, j), EndChar = GetWordPosition(query, j) + prevWord.Length },
                                    new Mention { Text = word, StartChar = GetWordPosition(query, i), EndChar = GetWordPosition(query, i) + word.Length }
                                }
                            };
                            chains.Add(chain);

                            // Replace pronoun in resolved query
                            resolvedQuery = resolvedQuery.Replace(words[i], prevWord, StringComparison.OrdinalIgnoreCase);
                            break;
                        }
                    }
                }
            }

            await Task.CompletedTask;
            return new CoreferenceResult
            {
                Chains = chains,
                ResolvedQuery = resolvedQuery
            };
        }

        // Helper methods
        private int GetTotalEntityCount(EntityExtractionResult extraction)
        {
            return extraction.GamblingEntities.Count +
                   extraction.TemporalEntities.Count +
                   extraction.FinancialEntities.Count +
                   extraction.PlayerEntities.Count +
                   extraction.GameEntities.Count +
                   extraction.MetricEntities.Count;
        }

        private float CalculateOverallConfidence(NLPProcessingResult result)
        {
            var confidences = new List<float>
            {
                result.EntityExtraction.OverallConfidence,
                result.IntentAnalysis.Confidence
            };

            if (result.TableRecommendations.Any())
                confidences.Add(result.TableRecommendations.Average(t => t.SimilarityScore));

            if (result.ColumnRecommendations.Any())
                confidences.Add(result.ColumnRecommendations.Average(c => c.SimilarityScore));

            return confidences.Any() ? confidences.Average() : 0f;
        }

        private bool ContainsAggregatePatterns(string query)
        {
            var patterns = new[] { "sum", "total", "count", "average", "avg", "max", "min", "calculate" };
            return patterns.Any(p => query.Contains(p));
        }

        private bool ContainsSelectPatterns(string query)
        {
            var patterns = new[] { "show", "display", "list", "get", "find", "what", "which" };
            return patterns.Any(p => query.Contains(p));
        }

        private bool ContainsTopNPatterns(string query)
        {
            var patterns = new[] { "top", "best", "highest", "most", "largest", "biggest" };
            return patterns.Any(p => query.Contains(p));
        }

        private bool ContainsTrendPatterns(string query)
        {
            var patterns = new[] { "trend", "over time", "daily", "weekly", "monthly", "yearly", "growth", "change" };
            return patterns.Any(p => query.Contains(p));
        }

        private bool ContainsComparisonPatterns(string query)
        {
            var patterns = new[] { "compare", "vs", "versus", "against", "difference", "between" };
            return patterns.Any(p => query.Contains(p));
        }

        private float CalculateAggregateConfidence(string query)
        {
            var score = 0f;
            if (query.Contains("sum") || query.Contains("total")) score += 0.3f;
            if (query.Contains("count")) score += 0.3f;
            if (query.Contains("average") || query.Contains("avg")) score += 0.3f;
            if (query.Contains("calculate")) score += 0.2f;
            return Math.Min(score, 1.0f);
        }

        private float CalculateSelectConfidence(string query)
        {
            var score = 0f;
            if (query.Contains("show") || query.Contains("display")) score += 0.3f;
            if (query.Contains("list") || query.Contains("get")) score += 0.2f;
            if (query.Contains("what") || query.Contains("which")) score += 0.3f;
            return Math.Min(score, 1.0f);
        }

        private float CalculateTopNConfidence(string query)
        {
            var score = 0f;
            if (query.Contains("top")) score += 0.4f;
            if (query.Contains("best") || query.Contains("highest")) score += 0.3f;
            if (query.Contains("most")) score += 0.2f;
            return Math.Min(score, 1.0f);
        }

        private float CalculateTrendConfidence(string query)
        {
            var score = 0f;
            if (query.Contains("trend")) score += 0.4f;
            if (query.Contains("over time")) score += 0.4f;
            if (query.Contains("daily") || query.Contains("monthly")) score += 0.3f;
            return Math.Min(score, 1.0f);
        }

        private float CalculateComparisonConfidence(string query)
        {
            var score = 0f;
            if (query.Contains("compare")) score += 0.4f;
            if (query.Contains("vs") || query.Contains("versus")) score += 0.4f;
            if (query.Contains("between")) score += 0.3f;
            return Math.Min(score, 1.0f);
        }

        private string GenerateIntentDescription(QueryIntent intent, string query)
        {
            return intent switch
            {
                QueryIntent.Aggregate => "Query requests aggregated calculations or summaries",
                QueryIntent.Select => "Query requests data retrieval or display",
                QueryIntent.TopN => "Query requests top-ranked results",
                QueryIntent.Trend => "Query requests trend analysis over time",
                QueryIntent.Comparison => "Query requests comparison between entities",
                _ => "General data query"
            };
        }

        private List<string> GenerateRequiredActions(QueryIntent intent, string query)
        {
            var actions = new List<string>();

            switch (intent)
            {
                case QueryIntent.Aggregate:
                    actions.Add("Identify aggregation function (SUM, COUNT, AVG, etc.)");
                    actions.Add("Determine grouping criteria");
                    break;
                case QueryIntent.TopN:
                    actions.Add("Extract ranking criteria");
                    actions.Add("Determine number of results (N)");
                    actions.Add("Add ORDER BY clause");
                    break;
                case QueryIntent.Trend:
                    actions.Add("Identify time dimension");
                    actions.Add("Determine time granularity");
                    actions.Add("Add temporal filtering");
                    break;
                case QueryIntent.Comparison:
                    actions.Add("Identify entities to compare");
                    actions.Add("Determine comparison criteria");
                    break;
            }

            return actions;
        }

        private string DeterminePartOfSpeech(string word)
        {
            // Simplified POS tagging
            var wordLower = word.ToLowerInvariant();
            
            if (new[] { "show", "get", "find", "calculate", "compare" }.Contains(wordLower))
                return "VERB";
            if (new[] { "player", "game", "revenue", "deposit", "withdrawal" }.Contains(wordLower))
                return "NOUN";
            if (new[] { "top", "best", "highest", "total", "average" }.Contains(wordLower))
                return "ADJ";
            if (new[] { "the", "a", "an" }.Contains(wordLower))
                return "DET";
            
            return "NOUN"; // Default
        }

        private bool IsVerb(string pos) => pos == "VERB";
        private bool IsNoun(string pos) => pos == "NOUN";
        private bool IsAdjective(string pos) => pos == "ADJ";

        private bool IsLikelyEntity(string word)
        {
            var entities = new[] { "player", "customer", "game", "revenue", "deposit", "withdrawal", "bet", "win" };
            return entities.Contains(word);
        }

        private int GetWordPosition(string text, int wordIndex)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var position = 0;
            for (int i = 0; i < wordIndex && i < words.Length; i++)
            {
                position += words[i].Length + 1; // +1 for space
            }
            return position;
        }
    }
}
