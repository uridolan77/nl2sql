-- Populate initial data for Prompt Builder tables

-- 1. Business Rules
INSERT INTO [dbo].[BusinessRules] ([RuleKey], [RuleName], [RuleContent], [RuleCategory], [IntentType], [Priority], [Condition], [Action])
VALUES 
-- Financial Rules
('DEPOSITS_COLUMN_RULE', 'Use Deposits Column for Deposits', 'CRITICAL: For deposits, ALWAYS use ''Deposits'' column, NEVER ''Amount''', 'FINANCIAL', 'QUERY_GENERATION', 1, 'deposit,deposits', 'Use Deposits column'),
('MONETARY_PRECISION', 'Monetary Value Precision', 'All monetary values: ROUND(amount, 2) AS [AliasName]', 'FINANCIAL', 'QUERY_GENERATION', 2, NULL, 'Apply ROUND function'),
('MEANINGFUL_ALIASES', 'Meaningful Column Aliases', 'Use meaningful aliases: SUM(RealBetAmount) AS TotalWageringAmount', 'FINANCIAL', 'QUERY_GENERATION', 3, NULL, 'Create descriptive aliases'),

-- Date Handling Rules
('YESTERDAY_LOGIC', 'Yesterday Date Logic', 'For yesterday queries: Date >= @Yesterday AND Date < @Today', 'DATE_HANDLING', 'QUERY_GENERATION', 1, 'yesterday', 'Use date range logic'),
('TODAY_LOGIC', 'Today Date Logic', 'For today queries: Date = @Today', 'DATE_HANDLING', 'QUERY_GENERATION', 2, 'today', 'Use exact date match'),
('DATE_VARIABLES', 'Mandatory Date Variables', 'Always declare @Today and @Yesterday variables', 'DATE_HANDLING', 'QUERY_GENERATION', 1, NULL, 'Include date declarations'),

-- Fraud Prevention Rules
('SUSPICIOUS_FILTER', 'Filter Suspicious Records', 'Include WHERE IsSuspicious = 0 clause where applicable', 'FRAUD_PREVENTION', 'QUERY_GENERATION', 1, NULL, 'Add suspicious filter'),
('TEST_ACCOUNT_FILTER', 'Filter Test Accounts', 'Filter out test accounts: WHERE IsTestAccount = 0', 'FRAUD_PREVENTION', 'QUERY_GENERATION', 2, NULL, 'Add test account filter'),

-- Output Formatting Rules
('CALCULATED_PREFIX', 'Calculated Column Prefix', 'Prefix calculated columns with ''calc_'': calc_RetentionRate', 'OUTPUT_FORMATTING', 'QUERY_GENERATION', 1, NULL, 'Add calc_ prefix'),
('TOP_LIMIT', 'Query Result Limit', 'Use SELECT TOP 100 at the beginning (NEVER at the end)', 'OUTPUT_FORMATTING', 'QUERY_GENERATION', 1, NULL, 'Add TOP 100'),
('NOLOCK_HINTS', 'Table Locking Hints', 'Mandatory WITH (NOLOCK) on ALL table references', 'OUTPUT_FORMATTING', 'QUERY_GENERATION', 1, NULL, 'Add NOLOCK hints');

-- 2. KPI Definitions
INSERT INTO [dbo].[KPIDefinitions] ([KPIKey], [KPIName], [Definition], [CalculationFormula], [BusinessContext], [Category], [ImportanceScore])
VALUES 
('GGR', 'Gross Gaming Revenue', 'Total bets minus total winnings', 'SUM(BetAmount) - SUM(WinAmount)', 'Primary revenue metric for gaming operations', 'REVENUE', 10.0),
('NGR', 'Net Gaming Revenue', 'GGR minus bonuses and promotions', 'GGR - SUM(BonusAmount) - SUM(PromotionAmount)', 'Net revenue after promotional costs', 'REVENUE', 9.5),
('ARPU', 'Average Revenue Per User', 'Total revenue divided by active players', 'SUM(Revenue) / COUNT(DISTINCT PlayerId)', 'Player value measurement', 'PLAYER_BEHAVIOR', 8.0),
('LTV', 'Lifetime Value', 'Predicted total revenue from a player', 'Complex calculation based on historical patterns', 'Long-term player value prediction', 'PLAYER_BEHAVIOR', 7.5),
('CHURN_RATE', 'Churn Rate', 'Percentage of players who stop playing in a given period', '(Inactive Players / Total Players) * 100', 'Player retention measurement', 'PLAYER_BEHAVIOR', 8.5),
('WAGERING', 'Wagering Amount', 'Bets placed using real money (not bonus funds)', 'SUM(RealBetAmount)', 'Real money betting activity', 'OPERATIONAL', 9.0),
('BONUS_CONVERSION', 'Bonus Conversion', 'Funds converted from bonus to real balance', 'SUM(BonusToRealAmount)', 'Bonus effectiveness measurement', 'OPERATIONAL', 6.0);

-- 3. Compliance Rules
INSERT INTO [dbo].[ComplianceRules] ([RuleKey], [RuleName], [RuleContent], [ComplianceType], [Jurisdiction], [EffectiveDate])
VALUES 
('GDPR_DATA_ACCESS', 'GDPR Data Access Restrictions', 'No access to PII tables unless explicitly authorized', 'GDPR', 'EU', '2018-05-25'),
('SOX_AUDIT_TRAIL', 'SOX Audit Trail Requirements', 'All queries must maintain audit trail compliance', 'SOX', 'US', '2002-07-30'),
('GAMING_FINANCIAL_ACCURACY', 'Gaming Financial Accuracy', 'Financial data requires absolute precision for regulatory reporting', 'GAMING_REGULATION', 'GLOBAL', '2020-01-01'),
('PII_PROTECTION', 'Personal Information Protection', 'Player data access follows strict role-based permissions', 'PRIVACY', 'GLOBAL', '2020-01-01');

-- 4. Example Queries
INSERT INTO [dbo].[ExampleQueries] ([ExampleKey], [NaturalLanguageQuery], [SQLQuery], [IntentType], [Complexity], [SchemaElements], [BusinessConcepts], [IsValidated])
VALUES 
('DEPOSITS_YESTERDAY', 'Show me total deposits for yesterday', 
'USE [DailyActionsDB]
DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE)
DECLARE @Yesterday DATE = DATEADD(DAY, -1, @Today)

SELECT TOP 100 
    ROUND(SUM(Deposits), 2) AS TotalDepositsAmount,
    COUNT(*) AS DepositCount
FROM PlayerActions pa WITH (NOLOCK)
WHERE pa.Date >= @Yesterday 
    AND pa.Date < @Today
    AND pa.IsSuspicious = 0
    AND pa.IsTestAccount = 0
ORDER BY TotalDepositsAmount DESC', 
'QUERY_GENERATION', 'SIMPLE', '["PlayerActions", "Deposits", "Date"]', '["deposits", "yesterday", "total"]', 1),

('PLAYER_ACTIVITY_TODAY', 'How many players were active today?', 
'USE [DailyActionsDB]
DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE)

SELECT TOP 100 
    COUNT(DISTINCT PlayerId) AS ActivePlayersToday
FROM PlayerActions pa WITH (NOLOCK)
WHERE pa.Date = @Today
    AND pa.IsSuspicious = 0
    AND pa.IsTestAccount = 0', 
'QUERY_GENERATION', 'SIMPLE', '["PlayerActions", "PlayerId", "Date"]', '["players", "active", "today"]', 1);

-- 5. Prompt Placeholders
INSERT INTO [dbo].[PromptPlaceholders] ([PlaceholderKey], [PlaceholderName], [DataSource], [StaticContent], [CacheMinutes])
VALUES 
('BUSINESS_DOMAIN_CONTEXT', 'Business Domain Context', 'COMPUTED', NULL, 60),
('GAMING_KPI_DEFINITIONS', 'Gaming KPI Definitions', 'COMPUTED', NULL, 120),
('COMPLIANCE_CONTEXT', 'Compliance Context', 'COMPUTED', NULL, 240),
('REGULATORY_CONSTRAINTS', 'Regulatory Constraints', 'COMPUTED', NULL, 240),
('SCHEMA_DEFINITION', 'Schema Definition', 'COMPUTED', NULL, 30),
('EXAMPLES', 'Example Queries', 'COMPUTED', NULL, 60),
('DATABASE_NAME', 'Database Name', 'STATIC', 'DailyActionsDB', 1440),
('FRAUD_FILTERS', 'Fraud Prevention Filters', 'STATIC', 'IsSuspicious = 0 AND IsTestAccount = 0', 1440);

-- Verify the data
SELECT 'BusinessRules' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[BusinessRules]
UNION ALL
SELECT 'KPIDefinitions', COUNT(*) FROM [dbo].[KPIDefinitions]
UNION ALL
SELECT 'ComplianceRules', COUNT(*) FROM [dbo].[ComplianceRules]
UNION ALL
SELECT 'ExampleQueries', COUNT(*) FROM [dbo].[ExampleQueries]
UNION ALL
SELECT 'PromptPlaceholders', COUNT(*) FROM [dbo].[PromptPlaceholders];
