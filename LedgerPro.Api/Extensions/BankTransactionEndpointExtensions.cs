using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;
using LedgerPro.Application.DTOs.Common;
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

        group.MapGet("/", GetBankTransactionsAsync);
        group.MapGet("/mappings", GetBankTransactionMappingsAsync);
        group.MapPost("/mappings", AddBankTransactionMappingAsync);

        return app;
    }

    /// <summary>
    /// Retrieves all bank transactions for a specific bank source from the database using the IBankTransactionRepository and returns them in the response.
    /// </summary>
    /// <param name="bankSourceId">The ID of the bank source for which to retrieve transactions.</param>
    /// <param name="repo">The repository used to access bank transactions.</param>
    /// <returns>A result containing the bank transactions.</returns>
    private static async Task<IResult> GetBankTransactionsAsync(Guid bankSourceId, IBankTransactionRepository repo)
    {
        var transactions = await repo.GetBankTransactionsAsync(bankSourceId);
        return Results.Ok(transactions);
    }

    /// <summary>
    /// Retrieves all bank transaction mappings from the database using the IBankTransactionRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access bank transaction mappings.</param>
    /// <returns>A result containing the bank transaction mappings.</returns>
    private static async Task<IResult> GetBankTransactionMappingsAsync(IBankTransactionRepository repo)
    {
        var mappings = await repo.GetBankTransactionMappingsAsync();
        return Results.Ok(mappings);
    }

    /// <summary>
    /// Adds a new bank transaction mapping to the database using the IBankTransactionService and commits the transaction using the IUnitOfWork.
    /// </summary>
    /// <param name="mapping">The bank transaction mapping to add.</param>
    /// <param name="service">The service used to manage bank transaction mappings.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    private static async Task<IResult> AddBankTransactionMappingAsync(BankTransactionMapping mapping, IBankTransactionService service, IUnitOfWork unitOfWork)
    {
        await service.AddBankTransactionMappingAsync(mapping);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank transaction mapping added successfully."));
    }
}
