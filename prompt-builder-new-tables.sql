-- New tables needed for Prompt Builder Module

-- 1. Business Rules Repository
CREATE TABLE [dbo].[BusinessRules](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [RuleKey] [nvarchar](100) NOT NULL,
    [RuleName] [nvarchar](200) NOT NULL,
    [RuleContent] [nvarchar](max) NOT NULL,
    [RuleCategory] [nvarchar](50) NOT NULL, -- FINANCIAL, DATE_HANDLING, FRAUD_PREVENTION, etc.
    [IntentType] [nvarchar](50) NULL, -- QUERY_GENERATION, ANALYTICAL, etc.
    [Priority] [int] NOT NULL DEFAULT 1,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [Condition] [nvarchar](1000) NULL, -- When this rule applies
    [Action] [nvarchar](max) NULL, -- What the rule enforces
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](256) NULL,
    [UpdatedBy] [nvarchar](256) NULL,
    CONSTRAINT [PK_BusinessRules] PRIMARY KEY CLUSTERED ([Id])
);

-- 2. KPI Definitions Repository  
CREATE TABLE [dbo].[KPIDefinitions](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [KPIKey] [nvarchar](50) NOT NULL,
    [KPIName] [nvarchar](200) NOT NULL,
    [Definition] [nvarchar](max) NOT NULL,
    [CalculationFormula] [nvarchar](max) NULL,
    [BusinessContext] [nvarchar](1000) NULL,
    [Industry] [nvarchar](50) NOT NULL DEFAULT 'Gaming',
    [Category] [nvarchar](50) NOT NULL, -- REVENUE, PLAYER_BEHAVIOR, OPERATIONAL, etc.
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [ImportanceScore] [decimal](18,2) NOT NULL DEFAULT 1.0,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](256) NULL,
    [UpdatedBy] [nvarchar](256) NULL,
    CONSTRAINT [PK_KPIDefinitions] PRIMARY KEY CLUSTERED ([Id])
);

-- 3. Compliance Rules Repository
CREATE TABLE [dbo].[ComplianceRules](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [RuleKey] [nvarchar](100) NOT NULL,
    [RuleName] [nvarchar](200) NOT NULL,
    [RuleContent] [nvarchar](max) NOT NULL,
    [ComplianceType] [nvarchar](50) NOT NULL, -- GDPR, SOX, GAMING_REGULATION, etc.
    [Jurisdiction] [nvarchar](100) NULL, -- EU, US, UK, etc.
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [EffectiveDate] [datetime2](7) NOT NULL,
    [ExpiryDate] [datetime2](7) NULL,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](256) NULL,
    [UpdatedBy] [nvarchar](256) NULL,
    CONSTRAINT [PK_ComplianceRules] PRIMARY KEY CLUSTERED ([Id])
);

-- 4. Example Queries Repository (Enhanced)
CREATE TABLE [dbo].[ExampleQueries](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [ExampleKey] [nvarchar](100) NOT NULL,
    [NaturalLanguageQuery] [nvarchar](1000) NOT NULL,
    [SQLQuery] [nvarchar](max) NOT NULL,
    [IntentType] [nvarchar](50) NOT NULL,
    [Complexity] [nvarchar](20) NOT NULL, -- SIMPLE, MEDIUM, COMPLEX
    [SchemaElements] [nvarchar](max) NULL, -- JSON array of tables/columns used
    [BusinessConcepts] [nvarchar](max) NULL, -- JSON array of business concepts
    [IsValidated] [bit] NOT NULL DEFAULT 0,
    [ValidationDate] [datetime2](7) NULL,
    [SuccessRate] [decimal](5,2) NULL,
    [UsageCount] [int] NOT NULL DEFAULT 0,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](256) NULL,
    [UpdatedBy] [nvarchar](256) NULL,
    CONSTRAINT [PK_ExampleQueries] PRIMARY KEY CLUSTERED ([Id])
);

-- 5. Prompt Placeholder Mappings
CREATE TABLE [dbo].[PromptPlaceholders](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [PlaceholderKey] [nvarchar](100) NOT NULL,
    [PlaceholderName] [nvarchar](200) NOT NULL,
    [DataSource] [nvarchar](100) NOT NULL, -- TABLE, FUNCTION, STATIC, COMPUTED
    [SourceQuery] [nvarchar](max) NULL, -- SQL to retrieve content
    [StaticContent] [nvarchar](max) NULL, -- For static placeholders
    [CacheMinutes] [int] NOT NULL DEFAULT 60, -- How long to cache the content
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](256) NULL,
    [UpdatedBy] [nvarchar](256) NULL,
    CONSTRAINT [PK_PromptPlaceholders] PRIMARY KEY CLUSTERED ([Id])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_BusinessRules_Category_IntentType] ON [dbo].[BusinessRules] ([RuleCategory], [IntentType]) WHERE [IsActive] = 1;
CREATE NONCLUSTERED INDEX [IX_KPIDefinitions_Category] ON [dbo].[KPIDefinitions] ([Category]) WHERE [IsActive] = 1;
CREATE NONCLUSTERED INDEX [IX_ComplianceRules_Type] ON [dbo].[ComplianceRules] ([ComplianceType]) WHERE [IsActive] = 1;
CREATE NONCLUSTERED INDEX [IX_ExampleQueries_IntentType] ON [dbo].[ExampleQueries] ([IntentType]) WHERE [IsActive] = 1;
CREATE NONCLUSTERED INDEX [IX_PromptPlaceholders_Key] ON [dbo].[PromptPlaceholders] ([PlaceholderKey]) WHERE [IsActive] = 1;
