using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Validation.BankTransaction;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Api.Extensions;

public static class ReportsEndpointExtensions
{
    public static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports").WithTags("Reports");

        group.MapGet("/accounts-summary", GetAccountsSummaryAsync);
        group.MapGet("/dashboard-summary", GetDashboardSummaryAsync);
        group.MapGet("/monthly-totals", GetMonthlyTotalsForDateRangeAsync);

        return app;
    }

    /// <summary>
    /// Retrieves a summary of accounts for a specified financial year by calling the IGeneralLedgerService to get the financial year accounts summary.
    /// </summary>
    /// <param name="service">The service used to retrieve the financial year accounts summary.</param>
    /// <param name="financialYearEnding">The ending year of the financial year to be reported.</param>
    /// <returns>A result containing the financial year accounts summary or a Bad Request response if the parameter is invalid.</returns>
    internal static async Task<IResult> GetAccountsSummaryAsync([FromServices] IGeneralLedgerService service, int? financialYearEnding)
    {
        // Validate the input parameter using the ValidGetFinancialYearValidator        
        var validator = new GetValidFinancialYearValidator();
        var validationResult = await validator.ValidateAsync(financialYearEnding);

        if (!validationResult.IsValid)        
            return Results.BadRequest(validationResult.Errors);
        
        // Call the service to get the financial year accounts summary based on the validated financial year ending parameter
        var fySummary = await service.GetFinancialYearAccountsSummaryAsync(financialYearEnding);

        if (fySummary == null)
            return Results.BadRequest("Unable to retrieve accounts summary for the specified financial year ending.");

        return Results.Ok(fySummary);
    } 

    /// <summary>
    /// Retrieves a summary of financial metrics for the dashboard, including total income, total expenses, assets, liabilities, 
    /// and the count of unreconciled transactions for a specified financial year by calling the IGeneralLedgerService to get the dashboard summary.
    /// The method validates the input parameter using the GetValidFinancialYearValidator and returns a Bad Request response if the parameter is invalid. 
    /// If the parameter is valid, it calls the service to get the dashboard summary and returns it in an Ok response.
    /// </summary>
    /// <param name="service">The service used to retrieve the dashboard summary.</param>
    /// <param name="financialYearEnding">The ending year of the financial year to be reported.</param>
    /// <returns>A result containing the dashboard summary or a Bad Request response if the parameter is invalid.</returns>
    internal static async Task<IResult> GetDashboardSummaryAsync([FromServices] IGeneralLedgerService service, int financialYearEnding)
    {
        // Validate the input parameter using the ValidGetFinancialYearValidator        
        var validator = new GetValidFinancialYearValidator();
        var validationResult = await validator.ValidateAsync(financialYearEnding);

        if (!validationResult.IsValid)        
            return Results.BadRequest(validationResult.Errors);
        
        // Call the service to get the dashboard summary based on the validated financial year ending parameter
        var summary = await service.GetDashboardSummaryAsync(financialYearEnding);

        return Results.Ok(summary);
    }   

    /// <summary>
    /// Retrieves monthly totals for a specified date range by calling the IGeneralLedgerService to get the monthly totals for the date range.
    /// </summary>
    /// <param name="service">The service used to retrieve the monthly totals.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A result containing the monthly totals for the specified date range or a Bad Request response if the parameters are invalid.</returns>
    internal static async Task<IResult> GetMonthlyTotalsForDateRangeAsync([FromServices] IGeneralLedgerService service, DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return Results.BadRequest("Start date must be less than or equal to end date.");
        
        // Call the service to get the monthly totals for the specified date range
        var totals = await service.GetMonthlyTotalsForDateRangeAsync(startDate, endDate);

        return Results.Ok(totals);
    }    
}
