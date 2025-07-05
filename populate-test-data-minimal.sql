-- Test data - few good entries for rules/KPIs, all placeholders for template functionality
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- 1. Business Rules - Key rules from your existing template
INSERT INTO [dbo].[BusinessRules] ([RuleKey], [RuleName], [RuleContent], [RuleCategory], [IntentType], [Priority], [Condition], [Action])
VALUES
('DEPOSITS_COLUMN_RULE', 'Use Deposits Column for Deposits', 'CRITICAL: For deposits, ALWAYS use ''Deposits'' column, NEVER ''Amount''', 'FINANCIAL', 'QUERY_GENERATION', 1, 'deposit,deposits', 'Use Deposits column instead of Amount column'),
('YESTERDAY_DATE_LOGIC', 'Yesterday Date Logic', 'For yesterday queries: Date >= DATEADD(day, -1, GETUTCDATE()) AND Date < CAST(GETUTCDATE() AS DATE)', 'DATE_HANDLING', 'QUERY_GENERATION', 1, 'yesterday', 'Use proper date range for yesterday'),
('NOLOCK_MANDATORY', 'Mandatory NOLOCK Hints', 'Use WITH (NOLOCK) on ALL table references for performance', 'PERFORMANCE', 'QUERY_GENERATION', 1, NULL, 'Add WITH (NOLOCK) to all table references');

-- 2. KPI Definitions - Core gaming KPIs
INSERT INTO [dbo].[KPIDefinitions] ([KPIKey], [KPIName], [Definition], [CalculationFormula], [BusinessContext], [Category], [ImportanceScore])
VALUES
('GGR', 'Gross Gaming Revenue', 'Total bets minus total winnings', 'SUM(BetAmount) - SUM(WinAmount)', 'Primary revenue metric for gaming operations representing house edge', 'REVENUE', 10.0),
('ARPU', 'Average Revenue Per User', 'Total revenue divided by active players', 'SUM(Revenue) / COUNT(DISTINCT PlayerId)', 'Player value measurement', 'PLAYER_BEHAVIOR', 8.0);

-- 3. Compliance Rules - One real GDPR rule
INSERT INTO [dbo].[ComplianceRules] ([RuleKey], [RuleName], [RuleContent], [ComplianceType], [Jurisdiction], [EffectiveDate])
VALUES 
('GDPR_PII_RESTRICTION', 'GDPR PII Access Restriction', 'No access to personally identifiable information without explicit authorization and legitimate business purpose', 'GDPR', 'EU', '2018-05-25');

-- 4. Example Queries - One validated query from your existing template knowledge
INSERT INTO [dbo].[ExampleQueries] ([ExampleKey], [NaturalLanguageQuery], [SQLQuery], [IntentType], [Complexity], [SchemaElements], [BusinessConcepts], [IsValidated])
VALUES 
('DEPOSITS_YESTERDAY_BASIC', 'Show me total deposits for yesterday', 
'USE [DailyActionsDB]
DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE)
DECLARE @Yesterday DATE = DATEADD(DAY, -1, @Today)

SELECT TOP 100 
    ROUND(SUM(Deposits), 2) AS TotalDepositsAmount
FROM DailyActions da WITH (NOLOCK)
WHERE da.Date >= @Yesterday 
    AND da.Date < @Today', 
'QUERY_GENERATION', 'SIMPLE', '["DailyActions", "Deposits", "Date"]', '["deposits", "yesterday", "total"]', 1);

-- 5. Prompt Placeholders - ALL placeholders needed for template functionality
INSERT INTO [dbo].[PromptPlaceholders] ([PlaceholderKey], [PlaceholderName], [DataSource], [StaticContent], [CacheMinutes])
VALUES
('DATABASE_NAME', 'Target Database Name', 'STATIC', 'DailyActionsDB', 1440),
('COMPLIANCE_CONTEXT', 'Compliance Context', 'COMPUTED', NULL, 240),
('BUSINESS_DOMAIN_CONTEXT', 'Business Domain Context', 'COMPUTED', NULL, 120),
('GAMING_KPI_DEFINITIONS', 'Gaming KPI Definitions', 'COMPUTED', NULL, 120),
('SCHEMA_DEFINITION', 'Schema Definition', 'COMPUTED', NULL, 30),
('CONTEXT', 'Additional Context', 'COMPUTED', NULL, 60),
('DOMAIN_RULES', 'Domain-Specific Rules', 'COMPUTED', NULL, 60),
('REGULATORY_CONSTRAINTS', 'Regulatory Constraints', 'COMPUTED', NULL, 240),
('EXAMPLES', 'Example Queries', 'COMPUTED', NULL, 60),
('FINANCIAL_RULES', 'Financial Data Rules', 'COMPUTED', NULL, 120),
('DATE_HANDLING_RULES', 'Date Handling Rules', 'COMPUTED', NULL, 120),
('FRAUD_PREVENTION_RULES', 'Fraud Prevention Rules', 'COMPUTED', NULL, 120),
('OUTPUT_FORMATTING_RULES', 'Output Formatting Rules', 'COMPUTED', NULL, 120),
('FRAUD_FILTERS', 'Fraud Prevention Filters', 'STATIC', 'IsSuspicious = 0 AND IsTestAccount = 0', 1440),
('VALIDATION_CHECKLIST', 'Validation Checklist', 'STATIC', 'Before finalizing the query, verify:
- [ ] Uses correct column names from schema
- [ ] Includes mandatory safety declarations
- [ ] All tables have WITH (NOLOCK) hints
- [ ] Monetary values are properly rounded and aliased
- [ ] Date logic follows standardized patterns
- [ ] Includes fraud prevention filters where applicable
- [ ] Query structure follows the template format', 1440);

-- Verify the test data
SELECT 'BusinessRules' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[BusinessRules]
UNION ALL
SELECT 'KPIDefinitions', COUNT(*) FROM [dbo].[KPIDefinitions]
UNION ALL
SELECT 'ComplianceRules', COUNT(*) FROM [dbo].[ComplianceRules]
UNION ALL
SELECT 'ExampleQueries', COUNT(*) FROM [dbo].[ExampleQueries]
UNION ALL
SELECT 'PromptPlaceholders', COUNT(*) FROM [dbo].[PromptPlaceholders];
