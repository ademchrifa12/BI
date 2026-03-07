using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.DataWarehouse;

/// <summary>
/// Date dimension table for the Data Warehouse
/// This entity provides comprehensive date attributes for time-based analysis
/// Pre-populated with dates for historical and future analysis
/// </summary>
[Table("DimDate", Schema = "DW")]
public class DimDate
{
    /// <summary>
    /// Surrogate key in YYYYMMDD format for efficient joins
    /// Example: 20260201 for February 1, 2026
    /// </summary>
    [Key]
    [Column("DateKey")]
    public int DateKey { get; set; }

    /// <summary>
    /// The actual date value
    /// </summary>
    [Column("Date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Day of the month (1-31)
    /// </summary>
    [Column("DayOfMonth")]
    public int DayOfMonth { get; set; }

    /// <summary>
    /// Day suffix (1st, 2nd, 3rd, etc.)
    /// </summary>
    [MaxLength(4)]
    [Column("DaySuffix")]
    public string DaySuffix { get; set; } = string.Empty;

    /// <summary>
    /// Day of the week (1-7, Sunday = 1)
    /// </summary>
    [Column("DayOfWeek")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Day name (Monday, Tuesday, etc.)
    /// </summary>
    [MaxLength(20)]
    [Column("DayName")]
    public string DayName { get; set; } = string.Empty;

    /// <summary>
    /// Short day name (Mon, Tue, etc.)
    /// </summary>
    [MaxLength(3)]
    [Column("DayNameShort")]
    public string DayNameShort { get; set; } = string.Empty;

    /// <summary>
    /// Day of the year (1-366)
    /// </summary>
    [Column("DayOfYear")]
    public int DayOfYear { get; set; }

    /// <summary>
    /// ISO week number of the year
    /// </summary>
    [Column("WeekOfYear")]
    public int WeekOfYear { get; set; }

    /// <summary>
    /// Week of the month
    /// </summary>
    [Column("WeekOfMonth")]
    public int WeekOfMonth { get; set; }

    /// <summary>
    /// Month number (1-12)
    /// </summary>
    [Column("Month")]
    public int Month { get; set; }

    /// <summary>
    /// Month name (January, February, etc.)
    /// </summary>
    [MaxLength(20)]
    [Column("MonthName")]
    public string MonthName { get; set; } = string.Empty;

    /// <summary>
    /// Short month name (Jan, Feb, etc.)
    /// </summary>
    [MaxLength(3)]
    [Column("MonthNameShort")]
    public string MonthNameShort { get; set; } = string.Empty;

    /// <summary>
    /// Quarter number (1-4)
    /// </summary>
    [Column("Quarter")]
    public int Quarter { get; set; }

    /// <summary>
    /// Quarter name (Q1, Q2, Q3, Q4)
    /// </summary>
    [MaxLength(2)]
    [Column("QuarterName")]
    public string QuarterName { get; set; } = string.Empty;

    /// <summary>
    /// Four-digit year
    /// </summary>
    [Column("Year")]
    public int Year { get; set; }

    /// <summary>
    /// Year-Month in YYYY-MM format
    /// </summary>
    [MaxLength(7)]
    [Column("YearMonth")]
    public string YearMonth { get; set; } = string.Empty;

    /// <summary>
    /// Year-Quarter (2026-Q1)
    /// </summary>
    [MaxLength(7)]
    [Column("YearQuarter")]
    public string YearQuarter { get; set; } = string.Empty;

    /// <summary>
    /// Flag indicating if weekend
    /// </summary>
    [Column("IsWeekend")]
    public bool IsWeekend { get; set; }

    /// <summary>
    /// Flag indicating if weekday
    /// </summary>
    [Column("IsWeekday")]
    public bool IsWeekday { get; set; }

    /// <summary>
    /// Flag indicating if holiday
    /// </summary>
    [Column("IsHoliday")]
    public bool IsHoliday { get; set; }

    /// <summary>
    /// Holiday name if applicable
    /// </summary>
    [MaxLength(100)]
    [Column("HolidayName")]
    public string? HolidayName { get; set; }

    /// <summary>
    /// Fiscal month (may differ from calendar)
    /// </summary>
    [Column("FiscalMonth")]
    public int FiscalMonth { get; set; }

    /// <summary>
    /// Fiscal quarter
    /// </summary>
    [Column("FiscalQuarter")]
    public int FiscalQuarter { get; set; }

    /// <summary>
    /// Fiscal year
    /// </summary>
    [Column("FiscalYear")]
    public int FiscalYear { get; set; }

    /// <summary>
    /// First day of the month
    /// </summary>
    [Column("FirstDayOfMonth")]
    public DateTime FirstDayOfMonth { get; set; }

    /// <summary>
    /// Last day of the month
    /// </summary>
    [Column("LastDayOfMonth")]
    public DateTime LastDayOfMonth { get; set; }

    /// <summary>
    /// First day of the quarter
    /// </summary>
    [Column("FirstDayOfQuarter")]
    public DateTime FirstDayOfQuarter { get; set; }

    /// <summary>
    /// Last day of the quarter
    /// </summary>
    [Column("LastDayOfQuarter")]
    public DateTime LastDayOfQuarter { get; set; }

    /// <summary>
    /// First day of the year
    /// </summary>
    [Column("FirstDayOfYear")]
    public DateTime FirstDayOfYear { get; set; }

    /// <summary>
    /// Last day of the year
    /// </summary>
    [Column("LastDayOfYear")]
    public DateTime LastDayOfYear { get; set; }

    // Navigation properties
    public virtual ICollection<FactVentes> Sales { get; set; } = new List<FactVentes>();
}
