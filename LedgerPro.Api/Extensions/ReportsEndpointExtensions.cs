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

        return Results.Ok(fySummary);
    } 

    /// <summary>
    /// Retrieves a summary of financial metrics for the dashboard, including total income, total expenses, assets, liabilities, 
    /// and the count of unreconciled transactions for a specified financial year by calling the IGeneralLedgerService to get the dashboard summary.
    /// The method validates the input parameter using the GetBankTransactionsRequestValidator and returns a Bad Request response if the parameter is invalid. 
    /// If the parameter is valid, it calls the service to get the dashboard summary and returns it in an Ok response.
    /// </summary>
    /// <param name="service"></param>
    /// <param name="financialYearEnding"></param>
    /// <returns></returns>
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
}
