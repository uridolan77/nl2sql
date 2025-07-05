# NL2SQL Solution Structure

## 📁 Project Organization

The NL2SQL solution is organized into four main projects following clean architecture principles:

```
NL2SQL.sln
├── NL2SQL.Core/                    # Domain layer - Core business logic
├── NL2SQL.Infrastructure/          # Infrastructure layer - Data access & external services
├── NL2SQL.Console/                 # Presentation layer - Console application
└── NL2SQL.Enhanced.Test/           # Test application - Real database integration
```

## 🏗️ NL2SQL.Core

**Purpose**: Contains the core domain models, interfaces, and business logic.

### Key Components:
- **Data/Entities/**: Entity Framework models for business metadata
  - `BusinessTableInfo.cs` - Table metadata with 30+ business fields
  - `BusinessColumnInfo.cs` - Column metadata with 25+ business fields
- **Data/**: Entity Framework DbContext
  - `BusinessMetadataDbContext.cs` - Main database context
- **Interfaces/**: Service contracts and repository interfaces
  - `IBusinessMetadataRepository.cs` - Repository contract
  - `Advanced/` - Advanced service interfaces
- **Models/**: Domain models and DTOs
  - `BaseModels.cs` - Core domain models
- **Services/**: Core business logic services
  - `BaseServices.cs` - Core service implementations
- **Configuration/**: Dependency injection setup
  - `ServiceCollectionExtensions.cs` - DI registration

### Dependencies:
- Entity Framework Core
- Microsoft.Extensions.DependencyInjection
- System.Text.Json

## 🔧 NL2SQL.Infrastructure

**Purpose**: Implements data access patterns and external service integrations.

### Key Components:
- **Repositories/**: Entity Framework repository implementations
  - `BusinessMetadataRepository.cs` - Real database metadata access
- **Services/**: Advanced NLP and semantic search services
  - `AdvancedEntityExtractor.cs` - Named entity recognition
  - `SemanticSearchService.cs` - Vector-based similarity search
  - `VectorEmbeddingService.cs` - OpenAI embeddings integration
  - `GamblingDomainKnowledge.cs` - Domain-specific business rules
  - `AdvancedNLPPipeline.cs` - Multi-stage NLP processing
- **Configuration/**: Infrastructure service registration
  - `ServiceCollectionExtensions.cs` - Infrastructure DI setup

### Dependencies:
- NL2SQL.Core (project reference)
- Entity Framework Core
- OpenAI API client
- ML.NET for machine learning
- MathNet.Numerics for vector operations

## 🖥️ NL2SQL.Console

**Purpose**: Console application for testing and demonstration.

### Key Components:
- **Program.cs**: Main console application entry point
- **appsettings.json**: Configuration file (excluded from version control)

### Dependencies:
- NL2SQL.Core (project reference)
- NL2SQL.Infrastructure (project reference)
- Microsoft.Extensions.Hosting

## 🧪 NL2SQL.Enhanced.Test

**Purpose**: Enhanced test application that exclusively uses real database schema.

### Key Components:
- **Program.cs**: Test application with database connectivity validation
- **Features**:
  - Database connectivity testing
  - Real metadata loading validation
  - Graceful failure when schema is missing
  - **No mock data fallbacks** - fails if database unavailable

### Dependencies:
- NL2SQL.Core (project reference)
- NL2SQL.Infrastructure (project reference)
- Microsoft.Extensions.Hosting

## 🔄 Build and Dependency Flow

```
NL2SQL.Enhanced.Test ──┐
                       ├──► NL2SQL.Infrastructure ──► NL2SQL.Core
NL2SQL.Console ────────┘
```

### Build Order:
1. **NL2SQL.Core** - Built first (no dependencies)
2. **NL2SQL.Infrastructure** - Built second (depends on Core)
3. **NL2SQL.Console** - Built third (depends on Core + Infrastructure)
4. **NL2SQL.Enhanced.Test** - Built last (depends on Core + Infrastructure)

## 🚀 Building the Solution

### Build All Projects:
```bash
dotnet build NL2SQL.sln
```

### Build Individual Projects:
```bash
dotnet build NL2SQL.Core/NL2SQL.Core.csproj
dotnet build NL2SQL.Infrastructure/NL2SQL.Infrastructure.csproj
dotnet build NL2SQL.Console/NL2SQL.Console.csproj
dotnet build NL2SQL.Enhanced.Test/NL2SQL.Enhanced.Test.csproj
```

### Clean Solution:
```bash
dotnet clean NL2SQL.sln
```

### Run Applications:
```bash
# Console application
dotnet run --project NL2SQL.Console

# Enhanced test application
dotnet run --project NL2SQL.Enhanced.Test
```

## 📋 Solution Configuration

The solution file (`NL2SQL.sln`) includes:
- All four projects with proper GUIDs
- Debug and Release configurations for all projects
- Proper build dependencies and ordering

### Project GUIDs:
- **NL2SQL.Core**: `{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}`
- **NL2SQL.Console**: `{3D63F9D1-98F1-456F-ABDA-F83ED9FBEB6B}`
- **NL2SQL.Infrastructure**: `{B2C3D4E5-F6G7-8901-BCDE-F23456789012}`
- **NL2SQL.Enhanced.Test**: `{C3D4E5F6-G7H8-9012-CDEF-345678901234}`

## 🔒 Security Considerations

- **Configuration files** with sensitive data are excluded from version control
- **Template files** provided for safe configuration sharing
- **Database connections** require proper authentication
- **API keys** must be configured securely

## 📊 Architecture Benefits

1. **Separation of Concerns**: Clear boundaries between layers
2. **Testability**: Interfaces enable easy mocking and testing
3. **Maintainability**: Modular structure supports independent development
4. **Scalability**: Clean architecture supports future enhancements
5. **Real Data Only**: No mock data ensures production accuracy
