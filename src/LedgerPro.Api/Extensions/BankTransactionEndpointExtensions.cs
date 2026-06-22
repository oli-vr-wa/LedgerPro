using LedgerPro.Core.Entities;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Application.Validation.BankTransaction;
using LedgerPro.Application.DTOs.BankStatement;
using Microsoft.AspNetCore.Mvc;
namespace LedgerPro.Api.Extensions;

/// <summary>
/// Extension class for mapping bank transaction related endpoints to the API. 
/// This class defines the routes and handlers for operations such as retrieving bank transactions,
/// retrieving transaction mappings, and adding new transaction mappings.
/// </summary>
public static class BankTransactionEndpointExtensions
{
    /// <summary>
    /// Maps the endpoints related to bank transactions, including retrieving transactions for a specific bank source, 
    /// retrieving transaction mappings, and adding new transaction mappings.
    /// Each endpoint is associated with a specific HTTP method and route, and it uses the IBankTransactionRepository to interact with 
    /// the database and perform the necessary operations. The endpoints are grouped under the "/api/v1/banktransactions" route and tagged as 
    /// "Bank Transactions" for better organization in API documentation.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapBankTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/banktransactions").WithTags("Bank Transactions");

        group.MapGet("/mappings", GetBankTransactionMappingsAsync);
        group.MapPost("/mapping", AddBankTransactionMappingAsync);
        group.MapPut("/mapping/{id:guid}", UpdateBankTransactionMappingAsync);
        group.MapDelete("/mapping/{id:guid}", DeleteBankTransactionMappingAsync);
        group.MapGet("/{bankSourceId:guid}/transactions/{financialYearEnding:int}", GetBankTransactionsForFinancialYearAsync);
        group.MapGet("/{bankSourceId:guid}/transactions/financial-years-overview", GetBankTransactionsFinancialYearsRowsAsync);
        group.MapPost("/reconcile", ReconcileBankTransactionAsync);
        group.MapPost("/unreconcile", UnreconcileBankTransactionAsync);

        return app;
    }

    /// <summary>
    /// Retrieves all bank transaction mappings from the database using the IBankTransactionRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access bank transaction mappings.</param>
    /// <returns>A result containing the bank transaction mappings.</returns>
    internal static async Task<IResult> GetBankTransactionMappingsAsync([FromServices] IBankTransactionRepository repo)
    {
        var mappings = await repo.GetBankTransactionMappingsAsync();
        return Results.Ok(mappings);
    }

    /// <summary>
    /// Adds a new bank transaction mapping to the database using the IBankTransactionService and commits the transaction using the IUnitOfWork.
    /// After adding the new mapping, it also attempts to match any pending transactions that may now find a match with the new mapping and commits those changes as well.
    /// </summary>
    /// <param name="mapping">The bank transaction mapping to add.</param>
    /// <param name="service">The service used to manage bank transaction mappings.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> AddBankTransactionMappingAsync(BankTransactionMapping mapping, [FromServices] IBankTransactionService service, [FromServices] IUnitOfWork unitOfWork)
    {
        await service.AddBankTransactionMappingAsync(mapping);
        await unitOfWork.CommitAsync();

        // After adding a new mapping, attempt to match any pending transactions that may now find a match with the new mapping
        await service.MatchPendingTransactionsAsync();
        await unitOfWork.CommitAsync();

        return Results.Created($"/api/v1/banktransactions/mappings/{mapping.Id}", new ActionResponse("Bank transaction mapping added successfully."));
    }

    /// <summary>
    /// Updates an existing bank transaction mapping in the database using the IBankTransactionRepository and commits the transaction using the IUnitOfWork.
    /// </summary>
    /// <param name="id">The ID of the bank transaction mapping to update.</param>
    /// <param name="mapping">The updated bank transaction mapping.</param>
    /// <param name="repo">The repository used to access bank transaction mappings.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> UpdateBankTransactionMappingAsync(Guid id, BankTransactionMapping mapping, [FromServices] IBankTransactionRepository repo, [FromServices] IUnitOfWork unitOfWork)
    {
        if (id == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The mapping ID is required."));
        if (mapping == null)
            return Results.BadRequest(new ErrorResponse("The mapping data is required."));

        var updatedMapping = await repo.UpdateBankTransactionMappingAsync(id, mapping);
        await unitOfWork.CommitAsync();

        return Results.Ok(updatedMapping);
    }

    /// <summary>
    /// Deletes an existing bank transaction mapping from the database using the IBankTransactionRepository and commits the transaction using the IUnitOfWork.
    /// </summary>
    /// <param name="id">The ID of the bank transaction mapping to delete.</param>
    /// <param name="repo">The repository used to access bank transaction mappings.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> DeleteBankTransactionMappingAsync(Guid id, [FromServices] IBankTransactionRepository repo, [FromServices] IUnitOfWork unitOfWork)
    {
        if (id == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The mapping ID is required."));

        await repo.DeleteBankTransactionMappingAsync(id);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction mapping deleted successfully."));
    }

    /// <summary>
    /// Retrieves bank transactions for a specific bank source and financial year by calling the IBankTransactionRepository to get the transaction rows.
    /// This endpoint is used to get the transaction data in a format suitable for display in the UI, including details such as 
    /// transaction date, description, amount, type, status, and associated general ledger accounts. 
    /// The transactions are filtered based on the specified financial year ending to provide relevant data for reporting and analysis purposes.   
    /// </summary>
    /// <param name="bankSourceId">The ID of the bank source for which to retrieve transactions.</param>
    /// <param name="financialYearEnding">The ending year of the financial year to be reported.</param>
    /// <param name="repo">The repository used to access bank transactions.</param>
    /// <returns>A result containing the bank transactions for the specified financial year.</returns>
    internal static async Task<IResult> GetBankTransactionsForFinancialYearAsync(Guid bankSourceId, int? financialYearEnding, [FromServices] IBankTransactionRepository repo)
    {   
        // Validate the input parameters using the GetBankTransactionsRequestValidator
        var validator = new GetBankTransactionsRequestValidator();
        var validationResult = await validator.ValidateAsync(new GetBankTransactionsRequest(bankSourceId, financialYearEnding));

        if (!validationResult.IsValid)        
            return Results.BadRequest(new ErrorResponse("Invalid request parameters.", validationResult.Errors));        

        var transactions = await repo.GetBankTransactionRowsAsync(bankSourceId, financialYearEnding);
        return Results.Ok(transactions);
    }

    /// <summary>
    /// Retrieves summary rows for bank transactions in a financial year for a specific bank source by calling the IBankTransactionRepository to get the financial year rows.
     /// This endpoint provides an overview of the bank transactions for each financial year, including the year ending, the date of the last transaction, and the count of pending transactions. 
     /// This information can be useful for reporting and analysis purposes, allowing users to quickly assess the status of their bank transactions across different financial years.
    /// </summary>
    /// <param name="bankSourceId">The unique identifier of the bank source.</param>
    /// <param name="repo">The repository used to access bank transactions.</param>
    /// <returns>A result containing the summary rows for the specified bank source.</returns>
    internal static async Task<IResult> GetBankTransactionsFinancialYearsRowsAsync(Guid bankSourceId, [FromServices] IBankTransactionRepository repo)
    {
        var rows = await repo.GetBankTransactionsFinancialYearRowsAsync(bankSourceId);
        return Results.Ok(rows);
    }

    /// <summary>
    /// Reconciles a bank transaction by creating general ledger items based on the provided split general ledger items in the request. 
    /// The method first validates the request to ensure that it is not null, contains at least one split general ledger item, 
    /// and that the total amount of the split items matches the bank transaction amount.
    /// </summary>
    /// <param name="request">The request containing the bank transaction ID and split general ledger items.</param>
    /// <param name="service">The service used to manage bank transactions.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> ReconcileBankTransactionAsync(ReconcileBankTransactionRequest request, [FromServices] IBankTransactionService service, [FromServices] IUnitOfWork unitOfWork)
    {
        if (request == null)
            return Results.BadRequest(new ErrorResponse("The request cannot be null."));
        if (request.BankTransactionId == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The bank transaction ID is required."));
        if (request.SplitGeneralLedgerItems == null || request.SplitGeneralLedgerItems.Count == 0)
            return Results.BadRequest(new ErrorResponse("At least one split general ledger item is required for reconciliation."));         

        await service.ReconcileBankTransactionAsync(request);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction reconciled successfully."));
    }

    /// <summary>
    /// Confirms the reconciliation of a categorized bank transaction by calling the IBankTransactionService to perform the confirmation logic and 
    /// then commits the transaction using the IUnitOfWork.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction to confirm reconciliation for.</param>
    /// <param name="service">The service used to manage bank transactions.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> ConfirmReconcileCategorizedBankTransactionAsync(Guid bankTransactionId, [FromServices] IBankTransactionService service, [FromServices] IUnitOfWork unitOfWork)
    {
        if (bankTransactionId == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The bank transaction ID is required."));

        await service.ConfirmReconcileCategorizedBankTransactionAsync(bankTransactionId);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction reconciliation confirmed successfully."));
    }

    /// <summary>
    /// Unreconciles a bank transaction by calling the IBankTransactionService to perform the unreconciliation logic and then commits the transaction using the IUnitOfWork.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction to unreconcile.</param>
    /// <param name="service">The service used to manage bank transactions.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> UnreconcileBankTransactionAsync(Guid bankTransactionId, [FromServices] IBankTransactionService service, [FromServices] IUnitOfWork unitOfWork)
    {
        if (bankTransactionId == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The bank transaction ID is required."));

        await service.UnreconcileBankTransactionAsync(bankTransactionId);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction unreconciled successfully."));
    }

    internal static async Task<IResult> CategorizeBankTransactionAsync(BankTransactionCategorize categorizeDto, [FromServices] IGeneralLedgerService generalLedgerService, [FromServices] IBankTransactionRepository bankTransactionRepo, [FromServices] IUnitOfWork unitOfWork)
    {
        if (categorizeDto == null)
            return Results.BadRequest(new ErrorResponse("The request cannot be null."));
        if (categorizeDto.BankTransactionId == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("The bank transaction ID is required."));
        if (categorizeDto.CategorizeItems == null || categorizeDto.CategorizeItems.Count == 0)
            return Results.BadRequest(new ErrorResponse("At least one categorize item is required."));

        var result = await generalLedgerService.CreateGeneralLedgerItemsAsync(categorizeDto);
        if (!result.IsSuccess)
            return Results.BadRequest(new ErrorResponse(result.Error ?? "An error occurred while categorizing the bank transaction."));

        // If categorization was successful, update the bank transaction status to Categorized
        await bankTransactionRepo.UpdateBankTransactionStatusAsync(categorizeDto.TransactionId, Core.Enums.BankTransactionStatus.Categorized);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction categorized successfully."));
    }
}
