
namespace LedgerPro.Application.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Calculates the financial year ending date based on the provided date. The financial year is assumed to end on June 30th.
    /// </summary>
    /// <param name="date">The date for which to calculate the financial year ending.</param>
    /// <returns>The financial year ending date.</returns>
    public static DateTime GetFinancialYearEnding(this DateTime date)
    {
        // Assuming financial year ends on 30th June
        return new DateTime(date.Month >= 7 ? date.Year + 1 : date.Year, 6, 30);
    }
}
