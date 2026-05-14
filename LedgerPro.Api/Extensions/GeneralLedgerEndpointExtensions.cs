using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;
using LedgerPro.Application.DTOs.Common;

namespace LedgerPro.Api.Extensions;

/// <summary>
/// Extension class for mapping the general ledger related endpoints to the API. 
/// This class defines the routes and handlers for operations such as retrieving general ledger items,
/// retrieving general ledger accounts, and adding new general ledger accounts.
/// </summary>
public static class GeneralLedgerEndpointExtensions
{
    /// <summary>
    /// Maps the endpoints related to the general ledger, including retrieving general ledger items, retrieving general ledger accounts, 
    /// and adding new general ledger accounts.
    /// Each endpoint is associated with a specific HTTP method and route, and it uses the IGeneralLedgerRepository to interact with 
    /// the database and perform the necessary operations. The endpoints are grouped under the "/api/v1/ledger" route and tagged as "General Ledger".
    /// </summary>
    /// <param name="app">The endpoint route builder used to map the endpoints.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapGeneralLedgerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/ledger").WithTags("General Ledger");

        group.MapGet("/items", GetGeneralLedgerItemsAsync);
        group.MapGet("/accounts", GetGeneralLedgerAccountsAsync);
        group.MapPost("/accounts", AddGeneralLedgerAccountAsync);

        return app;
    }

    /// <summary>
    /// Retrieves all GeneralLedgerItem entities from the database using the IGeneralLedgerRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access general ledger items.</param>
    /// <returns>A list of GeneralLedgerItem entities.</returns>
    internal static async Task<IResult> GetGeneralLedgerItemsAsync(IGeneralLedgerRepository repo)
    {
        var items = await repo.GetGeneralLedgerItemsAsync();
        return Results.Ok(items);
    }

    /// <summary>
    /// Adds a new GeneralLedgerAccount entity to the database using the IGeneralLedgerRepository.
    /// </summary>
    /// <param name="account">The GeneralLedgerAccount entity to add.</param>
    /// <param name="repo">The repository used to access general ledger accounts.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> AddGeneralLedgerAccountAsync(GeneralLedgerAccount account, IGeneralLedgerRepository repo)
    {
        bool isInUse = await repo.IsGeneralLedgerAccountIdInUseAsync(account.Id);
        if (isInUse)        
            return Results.BadRequest(new ErrorResponse($"General ledger account with ID {account.Id} is already in use and cannot be added."));        

        await repo.AddGeneralLedgerAccountAsync(account);
        return Results.Ok(new ActionResponse("General ledger account added successfully."));
    }

    /// <summary>
    /// Retrieves all GeneralLedgerAccount entities from the database using the IGeneralLedgerRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access general ledger accounts.</param>
    /// <returns>A list of GeneralLedgerAccount entities.</returns>
    internal static async Task<IResult> GetGeneralLedgerAccountsAsync(IGeneralLedgerRepository repo)
    {
        var accounts = await repo.GetGeneralLedgerAccountsAsync();
        return Results.Ok(accounts);
    }
}
