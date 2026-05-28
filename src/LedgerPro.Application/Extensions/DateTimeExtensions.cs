
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

    /// <summary>
    /// Calculates the financial year ending date based on the provided year. The financial year is assumed to end on June 30th of the specified year.    
    /// </summary>
    /// <param name="year">The year for which to calculate the financial year ending.</param>
    /// <returns>The financial year ending date.</returns>
    public static DateTime GetFinancialYearEnding(this int year)
    {
        if (year < 1900 || year > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1900 and 2100.");
        }

        // Assuming financial year ends on 30th June
        return new DateTime(year, 6, 30);
    }

    /// <summary>
    /// Calculates the financial year starting date based on the provided date. The financial year is assumed to start on July 1st of the previous year.
    /// </summary>
    /// <param name="date">The date for which to calculate the financial year starting.</param>
    /// <returns>The financial year starting date.</returns>
    public static DateTime GetFinancialYearStart(this DateTime date)
    {
        // Assuming financial year starts on 1st July
        return new DateTime(date.Month >= 7 ? date.Year : date.Year - 1, 7, 1);
    }

    /// <summary>
    /// Calculates the financial year starting date based on the provided year. The financial year is assumed to start on July 1st of the previous year.
    /// </summary> 
    /// <param name="year">The year for which to calculate the financial year starting.</param>
    /// <returns>The financial year starting date.</returns>
    public static DateTime GetFinancialYearStart(this int year)
    {
        if (year < 1900 || year > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1900 and 2100.");
        }

        // Assuming financial year starts on 1st July
        return new DateTime(year - 1, 7, 1);
    }

    /// <summary>
    /// Calculates the financial year ending date based on the provided financial year ending. 
    /// </summary>
    /// <param name="date">The date for which to calculate the financial year ending.</param>
    /// <returns>The financial year ending date.</returns>
    public static DateTime GetFinancialYearEnd(this DateTime date)
    {
        // Assuming financial year ends on 30th June
        return new DateTime(date.Month >= 7 ? date.Year + 1 : date.Year, 6, 30);
    }

    /// <summary>
    /// Calculates the financial year ending date based on the provided financial year ending. 
    /// The financial year is assumed to end on June 30th of the specified year.
    /// </summary>
    /// <param name="year">The year for which to calculate the financial year ending.</param>
    /// <returns>The financial year ending date.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static DateTime GetFinancialYearEnd(this int year)
    {
        if (year < 1900 || year > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1900 and 2100.");
        }

        // Assuming financial year ends on 30th June
        return new DateTime(year, 6, 30);
    }
}
