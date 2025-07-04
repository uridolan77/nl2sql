using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MathNet.Numerics.LinearAlgebra;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Core.Models.Advanced;

namespace NL2SQL.Infrastructure.Services
{
    /// <summary>
    /// Vector embedding service using sentence transformers for semantic similarity
    /// </summary>
    public class VectorEmbeddingService : IVectorEmbeddingService
    {
        private readonly ILogger<VectorEmbeddingService> _logger;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly VectorEmbeddingOptions _options;

        // Pre-trained sentence transformer model (simulated)
        private readonly Dictionary<string, float[]> _precomputedEmbeddings;

        public VectorEmbeddingService(
            ILogger<VectorEmbeddingService> logger,
            IMemoryCache cache,
            HttpClient httpClient,
            IOptions<VectorEmbeddingOptions> options)
        {
            _logger = logger;
            _cache = cache;
            _httpClient = httpClient;
            _options = options.Value;
            _precomputedEmbeddings = InitializePrecomputedEmbeddings();
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new float[384]; // Default embedding size

            var cacheKey = $"embedding_{text.GetHashCode()}";
            if (_cache.TryGetValue(cacheKey, out float[]? cachedEmbedding) && cachedEmbedding != null)
            {
                _logger.LogDebug("Retrieved embedding from cache for text: {Text}", text);
                return cachedEmbedding;
            }

            try
            {
                float[] embedding;

                // Try to use external API first (e.g., OpenAI, Hugging Face)
                if (!string.IsNullOrEmpty(_options.ApiEndpoint))
                {
                    embedding = await GenerateEmbeddingFromApiAsync(text);
                }
                else
                {
                    // Fallback to local computation
                    embedding = GenerateEmbeddingLocally(text);
                }

                // Cache the result
                _cache.Set(cacheKey, embedding, TimeSpan.FromHours(24));

                _logger.LogDebug("Generated embedding for text: {Text}", text);
                return embedding;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding for text: {Text}", text);
                return GenerateEmbeddingLocally(text); // Fallback
            }
        }

        public async Task<List<float[]>> GenerateEmbeddingsBatchAsync(List<string> texts)
        {
            var tasks = texts.Select(GenerateEmbeddingAsync);
            var embeddings = await Task.WhenAll(tasks);
            return embeddings.ToList();
        }

        public float CalculateCosineSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1.Length != embedding2.Length)
                throw new ArgumentException("Embeddings must have the same dimension");

            var vector1 = Vector<float>.Build.DenseOfArray(embedding1);
            var vector2 = Vector<float>.Build.DenseOfArray(embedding2);

            var dotProduct = vector1.DotProduct(vector2);
            var magnitude1 = vector1.L2Norm();
            var magnitude2 = vector2.L2Norm();

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0f;

            return (float)(dotProduct / (magnitude1 * magnitude2));
        }

        public async Task<List<SimilarityResult>> FindMostSimilarAsync(
            float[] queryEmbedding, 
            List<EmbeddingItem> candidates, 
            int topK = 5)
        {
            var similarities = new List<SimilarityResult>();

            foreach (var candidate in candidates)
            {
                var similarity = CalculateCosineSimilarity(queryEmbedding, candidate.Embedding);
                similarities.Add(new SimilarityResult
                {
                    Item = candidate,
                    SimilarityScore = similarity
                });
            }

            return similarities
                .OrderByDescending(s => s.SimilarityScore)
                .Take(topK)
                .Select((s, index) => { s.Rank = index + 1; return s; })
                .ToList();
        }

        private async Task<float[]> GenerateEmbeddingFromApiAsync(string text)
        {
            var request = new
            {
                input = text,
                model = _options.ModelName ?? "sentence-transformers/all-MiniLM-L6-v2"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
            }

            var response = await _httpClient.PostAsync(_options.ApiEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<EmbeddingApiResponse>(responseJson);

            return result?.Data?.FirstOrDefault()?.Embedding ?? new float[384];
        }

        private float[] GenerateEmbeddingLocally(string text)
        {
            // Simplified local embedding generation using TF-IDF-like approach
            // In production, you would use a proper sentence transformer model
            
            var words = text.ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .ToList();

            var embedding = new float[384]; // Standard sentence transformer dimension

            // Check for pre-computed embeddings for gambling terms
            foreach (var word in words)
            {
                if (_precomputedEmbeddings.TryGetValue(word, out var precomputed))
                {
                    for (int i = 0; i < Math.Min(embedding.Length, precomputed.Length); i++)
                    {
                        embedding[i] += precomputed[i];
                    }
                }
                else
                {
                    // Generate simple hash-based embedding
                    var hash = word.GetHashCode();
                    var random = new Random(hash);
                    for (int i = 0; i < embedding.Length; i++)
                    {
                        embedding[i] += (float)(random.NextDouble() - 0.5) * 0.1f;
                    }
                }
            }

            // Normalize the embedding
            var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
            if (magnitude > 0)
            {
                for (int i = 0; i < embedding.Length; i++)
                {
                    embedding[i] /= (float)magnitude;
                }
            }

            return embedding;
        }

        private Dictionary<string, float[]> InitializePrecomputedEmbeddings()
        {
            // Pre-computed embeddings for key gambling terms
            // In production, these would be generated by a proper sentence transformer
            var embeddings = new Dictionary<string, float[]>();

            var gamblingTerms = new[]
            {
                "ggr", "gross gaming revenue", "ngr", "net gaming revenue", "rtp", "return to player",
                "player", "customer", "deposit", "withdrawal", "bet", "win", "bonus", "game",
                "slot", "blackjack", "poker", "roulette", "casino", "sports betting",
                "revenue", "profit", "loss", "amount", "total", "sum", "count", "average"
            };

            foreach (var term in gamblingTerms)
            {
                // Generate deterministic embeddings based on term characteristics
                var embedding = GenerateTermEmbedding(term);
                embeddings[term] = embedding;
            }

            return embeddings;
        }

        private float[] GenerateTermEmbedding(string term)
        {
            var embedding = new float[384];
            var hash = term.GetHashCode();
            var random = new Random(hash);

            // Generate embedding with some semantic structure
            for (int i = 0; i < embedding.Length; i++)
            {
                embedding[i] = (float)(random.NextDouble() - 0.5);
            }

            // Add domain-specific patterns
            if (term.Contains("revenue") || term.Contains("ggr") || term.Contains("ngr"))
            {
                // Revenue terms get similar patterns in certain dimensions
                for (int i = 0; i < 50; i++)
                {
                    embedding[i] += 0.3f;
                }
            }

            if (term.Contains("player") || term.Contains("customer"))
            {
                // Player terms get similar patterns
                for (int i = 50; i < 100; i++)
                {
                    embedding[i] += 0.3f;
                }
            }

            if (term.Contains("game") || term.Contains("slot") || term.Contains("poker"))
            {
                // Game terms get similar patterns
                for (int i = 100; i < 150; i++)
                {
                    embedding[i] += 0.3f;
                }
            }

            if (term.Contains("deposit") || term.Contains("withdrawal") || term.Contains("bet"))
            {
                // Financial action terms
                for (int i = 150; i < 200; i++)
                {
                    embedding[i] += 0.3f;
                }
            }

            // Normalize
            var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
            if (magnitude > 0)
            {
                for (int i = 0; i < embedding.Length; i++)
                {
                    embedding[i] /= (float)magnitude;
                }
            }

            return embedding;
        }
    }

    /// <summary>
    /// Configuration options for vector embedding service
    /// </summary>
    public class VectorEmbeddingOptions
    {
        public string? ApiEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? ModelName { get; set; }
        public int EmbeddingDimension { get; set; } = 384;
        public int CacheExpirationHours { get; set; } = 24;
    }

    /// <summary>
    /// Response from embedding API
    /// </summary>
    public class EmbeddingApiResponse
    {
        public List<EmbeddingData>? Data { get; set; }
    }

    /// <summary>
    /// Embedding data from API
    /// </summary>
    public class EmbeddingData
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
