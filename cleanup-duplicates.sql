-- Clean up duplicate data from the new tables
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Remove duplicates from BusinessRules - keep the ones with lower IDs
WITH BusinessRulesCTE AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY RuleKey ORDER BY Id) as rn
    FROM BusinessRules
)
DELETE FROM BusinessRulesCTE WHERE rn > 1;

-- Remove duplicates from KPIDefinitions
WITH KPIDefinitionsCTE AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY KPIKey ORDER BY Id) as rn
    FROM KPIDefinitions
)
DELETE FROM KPIDefinitionsCTE WHERE rn > 1;

-- Remove duplicates from ComplianceRules
WITH ComplianceRulesCTE AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY RuleKey ORDER BY Id) as rn
    FROM ComplianceRules
)
DELETE FROM ComplianceRulesCTE WHERE rn > 1;

-- Remove duplicates from ExampleQueries
WITH ExampleQueriesCTE AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY ExampleKey ORDER BY Id) as rn
    FROM ExampleQueries
)
DELETE FROM ExampleQueriesCTE WHERE rn > 1;

-- Remove duplicates from PromptPlaceholders
WITH PromptPlaceholdersCTE AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY PlaceholderKey ORDER BY Id) as rn
    FROM PromptPlaceholders
)
DELETE FROM PromptPlaceholdersCTE WHERE rn > 1;

-- Verify cleanup
SELECT 'BusinessRules' AS TableName, COUNT(*) AS RecordCount FROM BusinessRules
UNION ALL
SELECT 'KPIDefinitions', COUNT(*) FROM KPIDefinitions
UNION ALL
SELECT 'ComplianceRules', COUNT(*) FROM ComplianceRules
UNION ALL
SELECT 'ExampleQueries', COUNT(*) FROM ExampleQueries
UNION ALL
SELECT 'PromptPlaceholders', COUNT(*) FROM PromptPlaceholders;
