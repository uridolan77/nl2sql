using System;
using System.Collections.Generic;

namespace NL2SQL.Core.Models.Enhanced
{
    /// <summary>
    /// Enhanced query analysis result with gambling domain expertise
    /// </summary>
    public class EnhancedQueryAnalysis
    {
        public string OriginalQuery { get; set; }
        public string NormalizedQuery { get; set; }
        public QueryContext QueryContext { get; set; }
        public DateTime AnalysisTimestamp { get; set; }
        
        // Core analysis results
        public QueryIntent Intent { get; set; }
        public List<EntityMention> Entities { get; set; } = new List<EntityMention>();
        public TemporalContext TemporalContext { get; set; } = new TemporalContext();
        public List<BusinessConcept> BusinessConcepts { get; set; } = new List<BusinessConcept>();
        
        // Gambling-specific analysis
        public List<GamblingMetric> GamblingMetrics { get; set; } = new List<GamblingMetric>();
        public AggregationRequirements AggregationRequirements { get; set; } = new AggregationRequirements();
        public FilterRequirements FilterRequirements { get; set; } = new FilterRequirements();
        public SortingRequirements SortingRequirements { get; set; } = new SortingRequirements();
        
        // Analysis metadata
        public QueryComplexity Complexity { get; set; }
        public float Confidence { get; set; }
        public List<QueryAmbiguity> Ambiguities { get; set; } = new List<QueryAmbiguity>();
        public List<string> RecommendedTables { get; set; } = new List<string>();
        public List<JoinRequirement> RequiredJoins { get; set; } = new List<JoinRequirement>();
        
        // Performance hints
        public List<string> OptimizationHints { get; set; } = new List<string>();
        public Dictionary<string, object> AdditionalMetadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Entity mention in natural language query
    /// </summary>
    public class EntityMention
    {
        public string Text { get; set; }
        public string EntityType { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public float Confidence { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public List<string> Synonyms { get; set; } = new List<string>();
        public string NormalizedValue { get; set; }
    }

    /// <summary>
    /// Temporal context extracted from query
    /// </summary>
    public class TemporalContext
    {
        public bool HasTemporalElements { get; set; }
        public List<TemporalExpression> Expressions { get; set; } = new List<TemporalExpression>();
        public DateTimeRange DateRange { get; set; }
        public TemporalGranularity Granularity { get; set; }
        public bool IsRelative { get; set; }
        public string TimeZone { get; set; } = "UTC";
    }

    /// <summary>
    /// Temporal expression definition
    /// </summary>
    public class TemporalExpression
    {
        public TemporalType Type { get; set; }
        public string Value { get; set; }
        public string OriginalText { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string SQLExpression { get; set; }
        public float Confidence { get; set; }
    }

    /// <summary>
    /// Date range specification
    /// </summary>
    public class DateTimeRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOpenEnded { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Gambling-specific metric definition
    /// </summary>
    public class GamblingMetric
    {
        public string MetricId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CalculationFormula { get; set; }
        public List<string> RequiredTables { get; set; } = new List<string>();
        public List<string> RequiredColumns { get; set; } = new List<string>();
        public string Domain { get; set; }
        public MetricType Type { get; set; }
        public string Unit { get; set; }
        public List<string> BusinessRules { get; set; } = new List<string>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Aggregation requirements for query
    /// </summary>
    public class AggregationRequirements
    {
        public List<string> Functions { get; set; } = new List<string>();
        public List<string> GroupByColumns { get; set; } = new List<string>();
        public List<string> HavingConditions { get; set; } = new List<string>();
        public bool RequiresDistinct { get; set; }
        public Dictionary<string, string> ColumnAliases { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Filter requirements for query
    /// </summary>
    public class FilterRequirements
    {
        public List<string> Conditions { get; set; } = new List<string>();
        public List<FilterCondition> StructuredConditions { get; set; } = new List<FilterCondition>();
        public LogicalOperator DefaultOperator { get; set; } = LogicalOperator.And;
    }

    /// <summary>
    /// Structured filter condition
    /// </summary>
    public class FilterCondition
    {
        public string Column { get; set; }
        public FilterOperator Operator { get; set; }
        public object Value { get; set; }
        public string DataType { get; set; }
        public bool IsParameterized { get; set; }
        public string ParameterName { get; set; }
    }

    /// <summary>
    /// Sorting requirements for query
    /// </summary>
    public class SortingRequirements
    {
        public List<SortColumn> Columns { get; set; } = new List<SortColumn>();
        public string Direction { get; set; } = "ASC";
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    /// <summary>
    /// Sort column specification
    /// </summary>
    public class SortColumn
    {
        public string ColumnName { get; set; }
        public string Direction { get; set; } = "ASC";
        public int Priority { get; set; }
        public bool IsCalculated { get; set; }
        public string Expression { get; set; }
    }

    /// <summary>
    /// Query ambiguity detection
    /// </summary>
    public class QueryAmbiguity
    {
        public AmbiguityType Type { get; set; }
        public string Description { get; set; }
        public List<string> Suggestions { get; set; } = new List<string>();
        public float Severity { get; set; }
        public string Context { get; set; }
    }

    /// <summary>
    /// Join requirement for multi-table queries
    /// </summary>
    public class JoinRequirement
    {
        public string LeftTable { get; set; }
        public string RightTable { get; set; }
        public JoinType Type { get; set; }
        public List<JoinCondition> Conditions { get; set; } = new List<JoinCondition>();
        public string BusinessReason { get; set; }
        public float Confidence { get; set; }
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// Join condition specification
    /// </summary>
    public class JoinCondition
    {
        public string LeftColumn { get; set; }
        public string RightColumn { get; set; }
        public string Operator { get; set; } = "=";
        public string Description { get; set; }
    }

    // Enums for query analysis
    public enum TemporalType
    {
        Absolute,
        Relative,
        Range,
        Recurring
    }

    public enum TemporalGranularity
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        Year
    }

    public enum MetricType
    {
        Financial,
        Operational,
        Engagement,
        Performance,
        Compliance,
        Risk
    }

    public enum LogicalOperator
    {
        And,
        Or,
        Not
    }

    public enum FilterOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Like,
        NotLike,
        In,
        NotIn,
        Between,
        IsNull,
        IsNotNull
    }

    public enum AmbiguityType
    {
        TermAmbiguity,
        StructuralAmbiguity,
        TemporalAmbiguity,
        EntityAmbiguity,
        MetricAmbiguity
    }

    public enum JoinType
    {
        Inner,
        Left,
        Right,
        Full,
        Cross
    }
}
