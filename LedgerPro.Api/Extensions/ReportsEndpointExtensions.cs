
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Api.Extensions;

public static class ReportsEndpointExtensions
{
    public static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports").WithTags("Reports");

        group.MapGet("/accounts-summary", GetAccountsSummaryAsync);

        return app;
    }

    /// <summary>
    /// Retrieves a summary of accounts for a specified financial year by calling the IGeneralLedgerService to get the financial year accounts summary.
    /// </summary>
    /// <param name="service">The service used to retrieve the financial year accounts summary.</param>
    /// <param name="financialYearEnding">The ending year of the financial year to be reported.</param>
    /// <returns>A result containing the financial year accounts summary or a Bad Request response if the parameter is invalid.</returns>
    internal static async Task<IResult> GetAccountsSummaryAsync(IGeneralLedgerService service, int? financialYearEnding)
    {
        if (financialYearEnding != null && (financialYearEnding < 1900 || financialYearEnding > 2100))
        {
            return Results.BadRequest("Financial year ending must be between 1900 and 2100.");
        }

        var fySummary = await service.GetFinancialYearAccountsSummaryAsync(financialYearEnding);

        return Results.Ok(fySummary);
    }    
}
