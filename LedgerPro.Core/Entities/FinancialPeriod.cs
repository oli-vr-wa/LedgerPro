using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities
{
    /// <summary>
    /// Represents a financial period, such as a month, quarter, or year. Used for organizing financial data and reporting.
    /// </summary>
    public class FinancialPeriod
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PeriodClassification Classification { get; set; } // Month, Quarter, Year

        public bool IsClosed { get; set; }
        public DateTime? ClosedDate { get; set; }
    }
}