# NL2SQL - Comprehensive Natural Language to SQL Library

A robust, intelligent, and production-ready Natural Language to SQL (NL2SQL) C# library specifically designed for the gambling/gaming industry with advanced semantic analysis and domain expertise.

## 🎯 Features

### Core Capabilities
- **Advanced Query Analysis**: Named entity recognition, temporal processing, business concept mapping
- **Gambling Domain Expertise**: Built-in knowledge of GGR, NGR, RTP, LTV, ARPU, and other gambling metrics
- **Intelligent Schema Building**: Semantic similarity scoring, business rule integration, join path optimization
- **Dynamic Prompt Engineering**: Intent-based templates, business context injection, quality validation
- **Multi-LLM Support**: OpenAI, Azure OpenAI, Anthropic Claude, local models with fallback mechanisms

### Business Intelligence Integration
- **Revenue Metrics**: Automated GGR/NGR calculations
- **Player Analytics**: LTV, ARPU, churn prediction
- **Compliance**: Regulatory reporting and responsible gaming
- **Real-time Operations**: Live betting, game performance, financial transactions

### Production-Ready Features
- **Security**: SQL injection prevention, access control, audit trails
- **Performance**: Intelligent caching, query optimization, parallel processing
- **Monitoring**: Health checks, metrics collection, distributed tracing
- **Scalability**: Microservices architecture, container support

## 🏗️ Solution Structure

### Projects Overview

```
NL2SQL.sln
├── NL2SQL.Core/                    # Core domain models, interfaces, and services
│   ├── Data/                       # Entity Framework models and DbContext
│   ├── Interfaces/                 # Service contracts and repository interfaces
│   ├── Models/                     # Domain models and DTOs
│   ├── Services/                   # Core business logic services
│   └── Configuration/              # Dependency injection setup
├── NL2SQL.Infrastructure/          # Data access and external service implementations
│   ├── Repositories/               # Entity Framework repositories
│   ├── Services/                   # Advanced NLP and semantic search services
│   └── Configuration/              # Infrastructure service registration
├── NL2SQL.Console/                 # Console application for testing
└── NL2SQL.Enhanced.Test/           # Enhanced test application (real database only)
```

### Key Components

- **🏗️ Entity Framework DbContext** - `BusinessMetadataDbContext`
- **📊 Entity Models** - `BusinessTableInfo`, `BusinessColumnInfo` with 50+ metadata fields
- **🔄 Repository Pattern** - `IBusinessMetadataRepository`, `BusinessMetadataRepository`
- **🎯 Semantic Search** - Vector embeddings for intelligent table/column matching
- **🤖 Advanced NLP Pipeline** - Multi-stage natural language processing
- **⚙️ Dependency Injection** - Full DI container integration

### Architecture Flow

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   NL Query      │───▶│  Query Analyzer │───▶│ Context Builder │
│   Processing    │    │   & Intent      │    │   & Schema      │
└─────────────────┘    │   Detection     │    │   Selection     │
                       └─────────────────┘    └─────────────────┘
                                                        │
┌─────────────────┐    ┌─────────────────┐             ▼
│   SQL Result    │◀───│  LLM Service    │◀───┌─────────────────┐
│   Validation    │    │   & Prompt      │    │ Prompt Builder  │
└─────────────────┘    │   Generation    │    │ & Enhancement   │
                       └─────────────────┘    └─────────────────┘
```

### Database-First Approach

**🚨 IMPORTANT**: This system operates exclusively with real database schema. There are **no mock data fallbacks**. The system will fail gracefully if the required business metadata tables are not available.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 or later
- SQL Server with your gambling business metadata
- Optional: OpenAI API key for LLM integration

### Building the Solution

1. **Clone the repository**:
   ```bash
   git clone https://github.com/uridolan77/nl2sql.git
   cd nl2sql
   ```

2. **Build the entire solution**:
   ```bash
   dotnet build NL2SQL.sln
   ```

3. **Run tests**:
   ```bash
   dotnet run --project NL2SQL.Enhanced.Test
   ```

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd nl2sql
   ```

2. **Update connection string**
   Edit `NL2SQL.Console/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BIReportingCopilot_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

3. **Build and run**
   ```bash
   dotnet build
   cd NL2SQL.Console
   dotnet run
   ```

### Basic Usage

```csharp
// Configure services
services.AddNL2SQL(configuration);

// Use the enhanced service
var nl2SqlService = serviceProvider.GetRequiredService<IEnhancedNL2SqlService>();

var context = new QueryContext
{
    UserId = "analyst",
    UserRoles = new List<string> { "Business Analyst" },
    PrimaryDomain = new BusinessDomain { DomainName = "Financial Operations" }
};

var result = await nl2SqlService.GenerateSqlAsync(
    "Show me the total GGR for last month", 
    context
);

if (result.Success)
{
    Console.WriteLine($"Generated SQL: {result.GeneratedSql}");
    Console.WriteLine($"Confidence: {result.Analysis.Confidence:F2}");
}
```

## 📊 Business Metadata Structure

The library leverages your existing business metadata tables:

### Core Tables
- **BusinessTableInfo**: Table-level context and relationships
- **BusinessColumnInfo**: Detailed column semantics and rules  
- **BusinessGlossary**: Term disambiguation and calculations
- **BusinessDomain**: Domain-specific context

### Sample Queries Supported

```csharp
// Financial metrics
"What was our GGR last month?"
"Calculate NGR by brand for this quarter"
"Show daily revenue trends for the past 30 days"

// Player analytics  
"What's the average player lifetime value?"
"How many new players registered yesterday?"
"Show top 10 players by total deposits"

// Game performance
"Which games have the highest RTP?"
"Show game revenue by category this week"
"What are the most popular slot games?"

// Operational queries
"How many withdrawals are pending approval?"
"Show deposit success rates by payment method"
"What's the average session duration by game type?"
```

## ⚙️ Configuration

### LLM Providers
```json
{
  "NL2SQL": {
    "LLMProviders": {
      "Providers": [
        {
          "ProviderId": "openai",
          "Name": "OpenAI",
          "Model": "gpt-4",
          "Configuration": {
            "ApiKey": "your-api-key",
            "MaxTokens": 2000,
            "Temperature": 0.1
          }
        }
      ]
    }
  }
}
```

### Security Settings
```json
{
  "Security": {
    "EnableSQLInjectionPrevention": true,
    "EnableAccessControl": true,
    "EnableAuditLogging": true,
    "RestrictedColumns": ["PlayerID", "Email", "PersonalData"]
  }
}
```

### Performance Tuning
```json
{
  "Performance": {
    "MaxConcurrentQueries": 100,
    "QueryTimeout": "00:02:00",
    "EnableQueryOptimization": true,
    "EnableParallelProcessing": true
  }
}
```

## 🎲 Gambling Domain Features

### Built-in Metrics
- **GGR (Gross Gaming Revenue)**: Total bets minus total wins
- **NGR (Net Gaming Revenue)**: GGR minus bonuses and costs
- **RTP (Return to Player)**: Percentage returned to players
- **LTV (Lifetime Value)**: Predicted player revenue
- **ARPU (Average Revenue Per User)**: Revenue per active player

### Compliance & Regulations
- GDPR compliance with data masking
- Gaming regulation adherence
- Audit trail generation
- Responsible gaming metrics

### Business Rules
- Automatic application of gambling-specific business rules
- Regulatory compliance validation
- Data quality checks
- Financial calculation accuracy

## 🔧 Development

### Project Structure
```
NL2SQL/
├── NL2SQL.Core/                 # Core library
│   ├── Models/Enhanced/         # Enhanced data models
│   ├── Services/Enhanced/       # Advanced services
│   ├── Interfaces/Enhanced/     # Service interfaces
│   ├── Repositories/Enhanced/   # Data access layer
│   └── Configuration/           # DI setup
├── NL2SQL.Console/             # Test console app
└── docs/                       # Documentation
```

### Key Components
- **AdvancedQueryAnalyzer**: NLP and intent detection
- **GamblingDomainService**: Industry-specific knowledge
- **EnhancedMetadataRepository**: Business metadata access
- **SemanticAnalysisService**: Text understanding
- **TemporalAnalysisService**: Date/time processing

### Testing
```bash
# Run the console application
cd NL2SQL.Console
dotnet run

# The app will test:
# - Metadata loading from your database
# - Gambling domain service functionality  
# - Query analysis capabilities
# - Sample business query processing
```

## 📈 Performance Metrics

### Target Performance
- **Query Processing Time**: < 2 seconds for 95% of queries
- **SQL Accuracy**: > 95% for domain-specific queries
- **Cache Hit Rate**: > 80% for similar queries
- **Concurrent Users**: Support for 1000+ users
- **Availability**: 99.9% uptime

### Monitoring
- Application metrics (response times, error rates)
- Business metrics (query accuracy, user satisfaction)
- Infrastructure metrics (CPU, memory, network)
- Custom dashboards and alerting

## 🛡️ Security

### Data Protection
- SQL injection prevention
- Access control validation
- Sensitive data masking
- Comprehensive audit logging

### Compliance
- GDPR compliance
- Gaming regulation adherence
- Financial reporting standards
- Data retention policies

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the GitHub repository
- Check the documentation in the `/docs` folder
- Review the sample queries in the console application

## 🎯 Roadmap

### Phase 1 (Current)
- ✅ Enhanced query analysis engine
- ✅ Gambling domain specialization
- ✅ Basic LLM integration
- ✅ Metadata repository

### Phase 2 (Next)
- [ ] Vector embeddings for semantic similarity
- [ ] Advanced caching with Redis
- [ ] Real-time analytics dashboard
- [ ] Machine learning model training

### Phase 3 (Future)
- [ ] Multi-language support
- [ ] Advanced visualization
- [ ] Predictive analytics
- [ ] Voice query support

---

**Built with ❤️ for the gambling industry**
