using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Validation.BankTransaction;

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
        // Validate the input parameter using the GetBankTransactionsRequestValidator
        var validationTarget = new GetBankTransactionsRequest(Guid.Empty, financialYearEnding);
        var validator = new GetBankTransactionsRequestValidator();
        var validationResult = await validator.ValidateAsync(validationTarget);

        if (!validationResult.IsValid)        
            return Results.BadRequest(validationResult.Errors);
        
        // Call the service to get the financial year accounts summary based on the validated financial year ending parameter
        var fySummary = await service.GetFinancialYearAccountsSummaryAsync(financialYearEnding);

        return Results.Ok(fySummary);
    }        
}
