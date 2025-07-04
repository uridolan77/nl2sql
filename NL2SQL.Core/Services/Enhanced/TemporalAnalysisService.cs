using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NL2SQL.Core.Models.Enhanced;
using NL2SQL.Core.Interfaces.Enhanced;
using Microsoft.Extensions.Logging;

namespace NL2SQL.Core.Services.Enhanced
{
    /// <summary>
    /// Temporal analysis service for date/time processing
    /// </summary>
    public class TemporalAnalysisService : ITemporalAnalysisService
    {
        private readonly ILogger<TemporalAnalysisService> _logger;

        // Temporal patterns for different expressions
        private readonly Dictionary<string, TemporalPattern> _temporalPatterns = new()
        {
            // Absolute dates
            [@"\b(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})\b"] = new TemporalPattern
            {
                Type = TemporalType.Absolute,
                Granularity = TemporalGranularity.Day,
                Parser = ParseAbsoluteDate
            },
            [@"\b(\d{4})[\/\-](\d{1,2})[\/\-](\d{1,2})\b"] = new TemporalPattern
            {
                Type = TemporalType.Absolute,
                Granularity = TemporalGranularity.Day,
                Parser = ParseAbsoluteDateISO
            },

            // Relative expressions
            [@"\btoday\b"] = new TemporalPattern
            {
                Type = TemporalType.Relative,
                Granularity = TemporalGranularity.Day,
                SQLExpression = "CAST(GETDATE() AS DATE)"
            },
            [@"\byesterday\b"] = new TemporalPattern
            {
                Type = TemporalType.Relative,
                Granularity = TemporalGranularity.Day,
                SQLExpression = "CAST(DATEADD(day, -1, GETDATE()) AS DATE)"
            },
            [@"\btomorrow\b"] = new TemporalPattern
            {
                Type = TemporalType.Relative,
                Granularity = TemporalGranularity.Day,
                SQLExpression = "CAST(DATEADD(day, 1, GETDATE()) AS DATE)"
            },

            // Week expressions
            [@"\bthis week\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Week,
                SQLExpression = "DATEPART(week, Date) = DATEPART(week, GETDATE()) AND YEAR(Date) = YEAR(GETDATE())"
            },
            [@"\blast week\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Week,
                SQLExpression = "Date >= DATEADD(week, -1, DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)) AND Date < DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)"
            },

            // Month expressions
            [@"\bthis month\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Month,
                SQLExpression = "MONTH(Date) = MONTH(GETDATE()) AND YEAR(Date) = YEAR(GETDATE())"
            },
            [@"\blast month\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Month,
                SQLExpression = "Date >= DATEADD(month, DATEDIFF(month, 0, GETDATE()) - 1, 0) AND Date < DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)"
            },

            // Year expressions
            [@"\bthis year\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Year,
                SQLExpression = "YEAR(Date) = YEAR(GETDATE())"
            },
            [@"\blast year\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Year,
                SQLExpression = "YEAR(Date) = YEAR(GETDATE()) - 1"
            },

            // Quarter expressions
            [@"\bthis quarter\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Quarter,
                SQLExpression = "DATEPART(quarter, Date) = DATEPART(quarter, GETDATE()) AND YEAR(Date) = YEAR(GETDATE())"
            },
            [@"\blast quarter\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Quarter,
                SQLExpression = "Date >= DATEADD(quarter, DATEDIFF(quarter, 0, GETDATE()) - 1, 0) AND Date < DATEADD(quarter, DATEDIFF(quarter, 0, GETDATE()), 0)"
            },

            // Numeric relative expressions
            [@"\blast (\d+) days?\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Day,
                Parser = ParseLastNDays
            },
            [@"\blast (\d+) weeks?\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Week,
                Parser = ParseLastNWeeks
            },
            [@"\blast (\d+) months?\b"] = new TemporalPattern
            {
                Type = TemporalType.Range,
                Granularity = TemporalGranularity.Month,
                Parser = ParseLastNMonths
            }
        };

        public TemporalAnalysisService(ILogger<TemporalAnalysisService> logger)
        {
            _logger = logger;
        }

        public async Task<TemporalContext> ExtractTemporalContextAsync(string query, Dictionary<string, TemporalExpression> patterns)
        {
            var context = new TemporalContext();
            var expressions = new List<TemporalExpression>();

            var queryLower = query.ToLowerInvariant();

            // Extract temporal expressions using patterns
            foreach (var pattern in _temporalPatterns)
            {
                var matches = Regex.Matches(queryLower, pattern.Key, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var expression = new TemporalExpression
                    {
                        Type = pattern.Value.Type,
                        OriginalText = match.Value,
                        Confidence = 0.9f
                    };

                    // Use custom parser if available
                    if (pattern.Value.Parser != null)
                    {
                        var parsed = pattern.Value.Parser(match);
                        expression.Value = parsed.Value;
                        expression.SQLExpression = parsed.SQLExpression;
                        expression.ResolvedDate = parsed.ResolvedDate;
                    }
                    else
                    {
                        expression.Value = match.Value;
                        expression.SQLExpression = pattern.Value.SQLExpression;
                    }

                    expressions.Add(expression);
                }
            }

            // Also check provided patterns
            if (patterns != null)
            {
                foreach (var pattern in patterns)
                {
                    if (queryLower.Contains(pattern.Key.ToLowerInvariant()))
                    {
                        expressions.Add(pattern.Value);
                    }
                }
            }

            context.Expressions = expressions;
            context.HasTemporalElements = expressions.Any();

            if (context.HasTemporalElements)
            {
                context.Granularity = DetermineGranularity(expressions);
                context.DateRange = CalculateDateRange(expressions);
                context.IsRelative = expressions.Any(e => e.Type == TemporalType.Relative);
            }

            return context;
        }

        public async Task<DateTimeRange> ResolveDateRangeAsync(string temporalExpression)
        {
            var expression = temporalExpression.ToLowerInvariant().Trim();
            var now = DateTime.Now;

            return expression switch
            {
                "today" => new DateTimeRange
                {
                    StartDate = now.Date,
                    EndDate = now.Date.AddDays(1).AddTicks(-1),
                    Description = "Today"
                },
                "yesterday" => new DateTimeRange
                {
                    StartDate = now.Date.AddDays(-1),
                    EndDate = now.Date.AddTicks(-1),
                    Description = "Yesterday"
                },
                "this week" => new DateTimeRange
                {
                    StartDate = now.Date.AddDays(-(int)now.DayOfWeek),
                    EndDate = now.Date.AddDays(7 - (int)now.DayOfWeek).AddTicks(-1),
                    Description = "This week"
                },
                "last week" => new DateTimeRange
                {
                    StartDate = now.Date.AddDays(-(int)now.DayOfWeek - 7),
                    EndDate = now.Date.AddDays(-(int)now.DayOfWeek).AddTicks(-1),
                    Description = "Last week"
                },
                "this month" => new DateTimeRange
                {
                    StartDate = new DateTime(now.Year, now.Month, 1),
                    EndDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1),
                    Description = "This month"
                },
                "last month" => new DateTimeRange
                {
                    StartDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1),
                    EndDate = new DateTime(now.Year, now.Month, 1).AddTicks(-1),
                    Description = "Last month"
                },
                "this year" => new DateTimeRange
                {
                    StartDate = new DateTime(now.Year, 1, 1),
                    EndDate = new DateTime(now.Year + 1, 1, 1).AddTicks(-1),
                    Description = "This year"
                },
                "last year" => new DateTimeRange
                {
                    StartDate = new DateTime(now.Year - 1, 1, 1),
                    EndDate = new DateTime(now.Year, 1, 1).AddTicks(-1),
                    Description = "Last year"
                },
                _ => new DateTimeRange
                {
                    IsOpenEnded = true,
                    Description = temporalExpression
                }
            };
        }

        public async Task<string> ConvertToSQLDateExpressionAsync(TemporalExpression expression)
        {
            if (!string.IsNullOrEmpty(expression.SQLExpression))
            {
                return expression.SQLExpression;
            }

            // Generate SQL expression based on type and value
            return expression.Type switch
            {
                TemporalType.Absolute when expression.ResolvedDate.HasValue =>
                    $"Date = '{expression.ResolvedDate.Value:yyyy-MM-dd}'",
                TemporalType.Relative => GenerateRelativeSQLExpression(expression.Value),
                TemporalType.Range => GenerateRangeSQLExpression(expression.Value),
                _ => "1=1" // Default to no filter
            };
        }

        public async Task<TemporalGranularity> DetectGranularityAsync(string query)
        {
            var queryLower = query.ToLowerInvariant();

            if (queryLower.Contains("daily") || queryLower.Contains("day") || queryLower.Contains("today") || queryLower.Contains("yesterday"))
                return TemporalGranularity.Day;
            
            if (queryLower.Contains("weekly") || queryLower.Contains("week"))
                return TemporalGranularity.Week;
            
            if (queryLower.Contains("monthly") || queryLower.Contains("month"))
                return TemporalGranularity.Month;
            
            if (queryLower.Contains("quarterly") || queryLower.Contains("quarter"))
                return TemporalGranularity.Quarter;
            
            if (queryLower.Contains("yearly") || queryLower.Contains("year") || queryLower.Contains("annual"))
                return TemporalGranularity.Year;
            
            if (queryLower.Contains("hourly") || queryLower.Contains("hour"))
                return TemporalGranularity.Hour;

            return TemporalGranularity.Day; // Default granularity
        }

        // Helper methods
        private TemporalGranularity DetermineGranularity(List<TemporalExpression> expressions)
        {
            if (!expressions.Any()) return TemporalGranularity.Day;

            // Return the finest granularity found
            var granularities = expressions.Select(e => GetGranularityFromExpression(e)).ToList();
            return granularities.Min();
        }

        private TemporalGranularity GetGranularityFromExpression(TemporalExpression expression)
        {
            var text = expression.OriginalText?.ToLowerInvariant() ?? "";
            
            if (text.Contains("hour")) return TemporalGranularity.Hour;
            if (text.Contains("day") || text.Contains("today") || text.Contains("yesterday")) return TemporalGranularity.Day;
            if (text.Contains("week")) return TemporalGranularity.Week;
            if (text.Contains("month")) return TemporalGranularity.Month;
            if (text.Contains("quarter")) return TemporalGranularity.Quarter;
            if (text.Contains("year")) return TemporalGranularity.Year;
            
            return TemporalGranularity.Day;
        }

        private DateTimeRange CalculateDateRange(List<TemporalExpression> expressions)
        {
            var ranges = expressions.Where(e => e.ResolvedDate.HasValue)
                                   .Select(e => e.ResolvedDate.Value)
                                   .ToList();

            if (!ranges.Any())
            {
                return new DateTimeRange { IsOpenEnded = true };
            }

            return new DateTimeRange
            {
                StartDate = ranges.Min(),
                EndDate = ranges.Max(),
                Description = "Calculated from temporal expressions"
            };
        }

        private string GenerateRelativeSQLExpression(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "today" => "CAST(Date AS DATE) = CAST(GETDATE() AS DATE)",
                "yesterday" => "CAST(Date AS DATE) = CAST(DATEADD(day, -1, GETDATE()) AS DATE)",
                "tomorrow" => "CAST(Date AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE)",
                _ => "1=1"
            };
        }

        private string GenerateRangeSQLExpression(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "this week" => "DATEPART(week, Date) = DATEPART(week, GETDATE()) AND YEAR(Date) = YEAR(GETDATE())",
                "last week" => "DATEPART(week, Date) = DATEPART(week, GETDATE()) - 1 AND YEAR(Date) = YEAR(GETDATE())",
                "this month" => "MONTH(Date) = MONTH(GETDATE()) AND YEAR(Date) = YEAR(GETDATE())",
                "last month" => "MONTH(Date) = MONTH(GETDATE()) - 1 AND YEAR(Date) = YEAR(GETDATE())",
                "this year" => "YEAR(Date) = YEAR(GETDATE())",
                "last year" => "YEAR(Date) = YEAR(GETDATE()) - 1",
                _ => "1=1"
            };
        }

        // Static parser methods
        private static TemporalExpression ParseAbsoluteDate(Match match)
        {
            if (DateTime.TryParse($"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value}", out var date))
            {
                return new TemporalExpression
                {
                    Value = date.ToString("yyyy-MM-dd"),
                    ResolvedDate = date,
                    SQLExpression = $"CAST(Date AS DATE) = '{date:yyyy-MM-dd}'"
                };
            }
            return new TemporalExpression { Value = match.Value };
        }

        private static TemporalExpression ParseAbsoluteDateISO(Match match)
        {
            if (DateTime.TryParse($"{match.Groups[1].Value}-{match.Groups[2].Value}-{match.Groups[3].Value}", out var date))
            {
                return new TemporalExpression
                {
                    Value = date.ToString("yyyy-MM-dd"),
                    ResolvedDate = date,
                    SQLExpression = $"CAST(Date AS DATE) = '{date:yyyy-MM-dd}'"
                };
            }
            return new TemporalExpression { Value = match.Value };
        }

        private static TemporalExpression ParseLastNDays(Match match)
        {
            if (int.TryParse(match.Groups[1].Value, out var days))
            {
                return new TemporalExpression
                {
                    Value = $"last {days} days",
                    SQLExpression = $"Date >= DATEADD(day, -{days}, GETDATE())"
                };
            }
            return new TemporalExpression { Value = match.Value };
        }

        private static TemporalExpression ParseLastNWeeks(Match match)
        {
            if (int.TryParse(match.Groups[1].Value, out var weeks))
            {
                return new TemporalExpression
                {
                    Value = $"last {weeks} weeks",
                    SQLExpression = $"Date >= DATEADD(week, -{weeks}, GETDATE())"
                };
            }
            return new TemporalExpression { Value = match.Value };
        }

        private static TemporalExpression ParseLastNMonths(Match match)
        {
            if (int.TryParse(match.Groups[1].Value, out var months))
            {
                return new TemporalExpression
                {
                    Value = $"last {months} months",
                    SQLExpression = $"Date >= DATEADD(month, -{months}, GETDATE())"
                };
            }
            return new TemporalExpression { Value = match.Value };
        }
    }

    // Helper classes
    public class TemporalPattern
    {
        public TemporalType Type { get; set; }
        public TemporalGranularity Granularity { get; set; }
        public string SQLExpression { get; set; }
        public Func<Match, TemporalExpression> Parser { get; set; }
    }
}
