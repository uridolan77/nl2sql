using Microsoft.Extensions.DependencyInjection;
using NL2SQL.Core.Interfaces;
using NL2SQL.Core.Interfaces.Advanced;
using NL2SQL.Infrastructure.Repositories;
using NL2SQL.Infrastructure.Services;

namespace NL2SQL.Infrastructure.Configuration
{
    /// <summary>
    /// Service collection extensions for NL2SQL Infrastructure layer
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add NL2SQL Infrastructure services to the service collection
        /// </summary>
        public static IServiceCollection AddNL2SQLInfrastructure(this IServiceCollection services)
        {
            // Business metadata repository
            services.AddScoped<IBusinessMetadataRepository, BusinessMetadataRepository>();

            // Enhanced NLP services
            services.AddScoped<IAdvancedEntityExtractor, AdvancedEntityExtractor>();
            services.AddScoped<ISemanticSearchService, SemanticSearchService>();
            services.AddScoped<IVectorEmbeddingService, VectorEmbeddingService>();
            services.AddScoped<IGamblingDomainKnowledge, GamblingDomainKnowledge>();
            services.AddScoped<IAdvancedNLPPipeline, AdvancedNLPPipeline>();

            // Prompt Builder services
            services.AddScoped<IPromptBuilderService, PromptBuilderService>();
            services.AddScoped<IPlaceholderResolverService, PlaceholderResolverService>();
            services.AddScoped<IBusinessRuleService, BusinessRuleService>();

            return services;
        }
    }
}
