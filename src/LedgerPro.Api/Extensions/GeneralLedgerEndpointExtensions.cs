using LedgerPro.Core.Entities;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

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
        group.MapPost("/account", AddGeneralLedgerAccountAsync);
        group.MapPut("/account/{id:int}", UpdateGeneralLedgerAccountAsync);
        group.MapDelete("/account/{id:int}", DeleteGeneralLedgerAccountAsync);

        return app;
    }

    /// <summary>
    /// Retrieves all GeneralLedgerItem entities from the database using the IGeneralLedgerRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access general ledger items.</param>
    /// <returns>A list of GeneralLedgerItem entities.</returns>
    internal static async Task<IResult> GetGeneralLedgerItemsAsync([FromServices] IGeneralLedgerRepository repo)
    {
        var items = await repo.GetGeneralLedgerItemsAsync();
        return Results.Ok(items);
    }

    /// <summary>
    /// Adds a new GeneralLedgerAccount entity to the database using the IGeneralLedgerService.
    /// </summary>
    /// <param name="account">The GeneralLedgerAccount entity to add.</param>
    /// <param name="service">The service used to handle business logic for general ledger accounts.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> AddGeneralLedgerAccountAsync(GeneralLedgerAccount account, [FromServices] IGeneralLedgerService service, [FromServices] IUnitOfWork unitOfWork)
    {
        await service.AddGeneralLedgerAccountAsync(account);
        await unitOfWork.CommitAsync();

        return Results.Created($"/api/v1/ledger/account/{account.Id}", account);
    }

    /// <summary>
    /// Updates an existing GeneralLedgerAccount entity in the database using the IGeneralLedgerRepository.
    /// </summary>
    /// <param name="id">The ID of the general ledger account to update.</param>
    /// <param name="account">The updated GeneralLedgerAccount entity.</param>
    /// <param name="repo">The repository used to access general ledger accounts.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> UpdateGeneralLedgerAccountAsync(int id, GeneralLedgerAccount account, [FromServices] IGeneralLedgerRepository repo, [FromServices] IUnitOfWork unitOfWork)
    {
        await repo.UpdateGeneralLedgerAccountAsync(id, account);
        await unitOfWork.CommitAsync();

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes an existing GeneralLedgerAccount entity from the database using the IGeneralLedgerService.
    /// </summary>
    /// <param name="id">The ID of the general ledger account to delete.</param>
    /// <param name="service">The service used to handle business logic for general ledger accounts.</param>
    /// <param name="unitOfWork">The unit of work used to manage transactions.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    internal static async Task<IResult> DeleteGeneralLedgerAccountAsync(int id, [FromServices] IGeneralLedgerService service, [FromServices] IUnitOfWork unitOfWork)
    {
        await service.DeleteGeneralLedgerAccountAsync(id);
        await unitOfWork.CommitAsync();

        return Results.NoContent();
    }

    /// <summary>
    /// Retrieves all GeneralLedgerAccount entities from the database using the IGeneralLedgerRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The repository used to access general ledger accounts.</param>
    /// <returns>A list of GeneralLedgerAccount entities.</returns>
    internal static async Task<IResult> GetGeneralLedgerAccountsAsync([FromServices] IGeneralLedgerRepository repo)
    {
        var accounts = await repo.GetGeneralLedgerAccountsAsync();
        return Results.Ok(accounts);
    }
}
