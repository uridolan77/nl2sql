using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Services;
using NL2SQL.Core.Repositories;
using System;

namespace NL2SQL.Core.Configuration
{
    /// <summary>
    /// Service collection extensions for NL2SQL dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add NL2SQL services to the service collection
        /// </summary>
        public static IServiceCollection AddNL2SQL(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            var nl2SqlConfig = new NL2SQLConfiguration();
            configuration.GetSection("NL2SQL").Bind(nl2SqlConfig);
            services.AddSingleton(nl2SqlConfig);

            // Connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

            // Core repositories
            services.AddScoped<IMetadataRepository>(provider => 
                new SqlServerMetadataRepository(connectionString, provider.GetRequiredService<ILogger<SqlServerMetadataRepository>>()));
            
            // Core services
            services.AddScoped<INaturalLanguageAnalyzer, SimpleNaturalLanguageAnalyzer>();
            services.AddScoped<ISchemaContextBuilder, SchemaContextBuilder>();
            services.AddScoped<ISqlPromptBuilder, SqlPromptBuilder>();

            // LLM services
            services.AddScoped<ILLMService, OpenAILLMService>();
            // Caching
            services.AddMemoryCache();

            // Main NL2SQL service
            services.AddScoped<INL2SqlService, NL2SqlService>();



            return services;
        }

        /// <summary>
        /// Add NL2SQL with custom configuration
        /// </summary>
        public static IServiceCollection AddNL2SQL(this IServiceCollection services, Action<NL2SQLConfiguration> configureOptions)
        {
            var config = new NL2SQLConfiguration();
            configureOptions(config);
            services.AddSingleton(config);

            return AddNL2SQLCore(services, config);
        }

        private static IServiceCollection AddNL2SQLCore(IServiceCollection services, NL2SQLConfiguration config)
        {
            // Core repositories
            services.AddScoped<IMetadataRepository>(provider => 
                new SqlServerMetadataRepository(config.ConnectionString, provider.GetRequiredService<ILogger<SqlServerMetadataRepository>>()));
            
            // Register all other services...
            RegisterCoreServices(services);
            RegisterLLMServices(services, config);

            return services;
        }

        private static void RegisterCoreServices(IServiceCollection services)
        {
            services.AddScoped<INaturalLanguageAnalyzer, SimpleNaturalLanguageAnalyzer>();
            services.AddScoped<ISchemaContextBuilder, SchemaContextBuilder>();
            services.AddScoped<ISqlPromptBuilder, SqlPromptBuilder>();
            services.AddScoped<INL2SqlService, NL2SqlService>();
        }



        private static void RegisterLLMServices(IServiceCollection services, NL2SQLConfiguration config)
        {
            // Register OpenAI LLM service as default
            services.AddScoped<ILLMService>(provider =>
                new OpenAILLMService("mock-key", provider.GetRequiredService<ILogger<OpenAILLMService>>()));
        }




    }

    /// <summary>
    /// NL2SQL configuration class
    /// </summary>
    public class NL2SQLConfiguration
    {
        public string ConnectionString { get; set; } = "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
        public LLMProviderSettings LLMProviders { get; set; } = new LLMProviderSettings();
        public CachingSettings Caching { get; set; } = new CachingSettings();
        public SecuritySettings Security { get; set; } = new SecuritySettings();
        public PerformanceSettings Performance { get; set; } = new PerformanceSettings();
        public GamblingDomainSettings Domain { get; set; } = new GamblingDomainSettings();
        public MonitoringSettings Monitoring { get; set; } = new MonitoringSettings();
    }

    public class LLMProviderSettings
    {
        public List<LLMProviderConfig> Providers { get; set; } = new List<LLMProviderConfig>
        {
            new LLMProviderConfig
            {
                ProviderId = "openai",
                Name = "OpenAI",
                Model = "gpt-4",
                IsAvailable = true,
                Priority = 1.0f,
                Configuration = new Dictionary<string, object>()
            }
        };
        public FallbackStrategy FallbackStrategy { get; set; } = FallbackStrategy.Sequential;
        public LoadBalancingStrategy LoadBalancing { get; set; } = LoadBalancingStrategy.RoundRobin;
        public RetryPolicy RetryPolicy { get; set; } = new RetryPolicy();
        public QualityThresholds QualityThresholds { get; set; } = new QualityThresholds();
    }

    public class LLMProviderConfig
    {
        public string ProviderId { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string ApiEndpoint { get; set; }
        public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
        public bool IsAvailable { get; set; } = true;
        public float Priority { get; set; } = 1.0f;
        public List<string> SupportedComplexities { get; set; } = new List<string> { "Simple", "Medium", "Complex", "VeryComplex" };
    }

    public class CachingSettings
    {
        public bool UseRedis { get; set; } = false;
        public string RedisConnectionString { get; set; }
        public TimeSpan DefaultTTL { get; set; } = TimeSpan.FromHours(1);
        public bool EnableSemanticCaching { get; set; } = true;
        public float SemanticSimilarityThreshold { get; set; } = 0.85f;
        public int MaxCacheSize { get; set; } = 1000;
    }

    public class SecuritySettings
    {
        public bool EnableSQLInjectionPrevention { get; set; } = true;
        public bool EnableAccessControl { get; set; } = true;
        public bool EnableAuditLogging { get; set; } = true;
        public bool EnableDataMasking { get; set; } = true;
        public List<string> RestrictedTables { get; set; } = new List<string>();
        public List<string> RestrictedColumns { get; set; } = new List<string>();
        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }

    public class PerformanceSettings
    {
        public int MaxConcurrentQueries { get; set; } = 100;
        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromMinutes(2);
        public int MaxResultRows { get; set; } = 10000;
        public bool EnableQueryOptimization { get; set; } = true;
        public bool EnableParallelProcessing { get; set; } = true;
        public int MaxParallelTasks { get; set; } = Environment.ProcessorCount;
    }

    public class GamblingDomainSettings
    {
        public Dictionary<string, string> StandardMetrics { get; set; } = new Dictionary<string, string>();
        public List<string> ComplianceRules { get; set; } = new List<string>();
        public Dictionary<string, string> DomainTerminology { get; set; } = new Dictionary<string, string>();
        public List<string> CalculationTemplates { get; set; } = new List<string>();
        public RegulatorySettings RegulatoryCompliance { get; set; } = new RegulatorySettings();
    }

    public class RegulatorySettings
    {
        public List<string> ApplicableRegulations { get; set; } = new List<string> { "GDPR", "Gaming Regulations" };
        public bool RequireDataMasking { get; set; } = true;
        public bool RequireAuditTrail { get; set; } = true;
        public TimeSpan DataRetentionPeriod { get; set; } = TimeSpan.FromDays(2555); // 7 years
    }

    public class MonitoringSettings
    {
        public bool EnableMetrics { get; set; } = true;
        public bool EnableTracing { get; set; } = true;
        public bool EnableHealthChecks { get; set; } = true;
        public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromMinutes(1);
        public string MetricsEndpoint { get; set; } = "/metrics";
    }

    public class RetryPolicy
    {
        public int MaxRetries { get; set; } = 3;
        public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
        public double BackoffMultiplier { get; set; } = 2.0;
    }

    public class QualityThresholds
    {
        public float MinSyntaxScore { get; set; } = 0.8f;
        public float MinSemanticScore { get; set; } = 0.7f;
        public float MinOverallScore { get; set; } = 0.75f;
    }

    public enum FallbackStrategy
    {
        Sequential,
        Parallel,
        BestEffort
    }

    public enum LoadBalancingStrategy
    {
        RoundRobin,
        WeightedRoundRobin,
        LeastConnections,
        Random
    }
}
