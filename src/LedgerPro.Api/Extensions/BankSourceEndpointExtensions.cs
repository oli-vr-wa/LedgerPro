using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Application.DTOs.BankSource;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Api.Extensions;

public static class BankSourceEndpointExtensions
{    
    public static IEndpointRouteBuilder MapBankSourcesEndpoints(this IEndpointRouteBuilder app)
    {
        // Use a group for bank sources
        var group = app.MapGroup("/api/v1").WithTags("Bank Sources");

        group.MapPost("/banksources/{id:guid}/import-statements", ImportBankStatementAsync).DisableAntiforgery();
        group.MapGet("/banksources/", GetBankSourcesAsync);
        group.MapPost("/banksource/", AddBankSourceAsync);
        group.MapPut("/banksource/{id:guid}", UpdateBankSourceAsync);
        group.MapDelete("/banksource/{id:guid}", DeleteBankSourceAsync);

        return app;
    }

    /// <summary>
    /// Handles the import of a bank statement file for a specific bank source. 
    /// This endpoint accepts a file upload and the unique identifier of the bank source, and it uses the IBankImportService 
    /// to process the file and import the transactions. 
    /// The method checks for the presence of the file, reads its content, and then calls the import service.
    /// </summary>
    /// <param name="id">Bank source identifier</param>
    /// <param name="file">Bank statement file</param>
    /// <param name="bankImportService">Bank import service</param>
    /// <param name="unitOfWork">Unit of work for committing changes</param>
    /// <returns>Result of the import operation</returns>
    internal static async Task<IResult> ImportBankStatementAsync(Guid id, IFormFile file, [FromServices]IBankImportService bankImportService, [FromServices]IUnitOfWork unitOfWork)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest(new ErrorResponse("No file uploaded."));
        
        using var stream = file.OpenReadStream();
        var request = new UploadBankStatementRequest(id, stream, file.FileName);

        var result = await bankImportService.ImportBankStatementAsync(request);
        // Only commit if the import was successful
        if (result.IsSuccess)
            await unitOfWork.CommitAsync();

        return result.IsSuccess
            ? Results.Ok(new ImportBankStatementResponse("Bank statement imported successfully.", result.Value))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    /// <summary>
    /// Retrieves all bank sources from the database using the IBankSourceRepository and returns them in the response.
    /// </summary>
    /// <param name="repo">The bank source repository</param>
    /// <returns>Result containing the list of bank sources</returns>
    internal static async Task<IResult> GetBankSourcesAsync([FromServices]IBankSourceRepository repo)
    {
        var sources = await repo.GetBankSourcesAsync();
        return Results.Ok(sources);
    }

    /// <summary>
    /// Adds a new bank source to the database.
    /// </summary>
    /// <param name="request">The request object containing the details of the bank source to be added.</param>
    /// <param name="service">The bank source service</param>
    /// <param name="unitOfWork">The unit of work for committing changes</param>
    /// <returns>Result indicating the outcome of the operation</returns>
    internal static async Task<IResult> AddBankSourceAsync(AddBankSourceRequest request, [FromServices]IBankSourceService service, [FromServices]IUnitOfWork unitOfWork)
    {
        if (request == null)        
            return Results.BadRequest(new ErrorResponse("Bank source data is required."));
        if (string.IsNullOrWhiteSpace(request.AccountName))
            return Results.BadRequest(new ErrorResponse("Account name is required."));
        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            return Results.BadRequest(new ErrorResponse("Account number is required."));
        if (string.IsNullOrWhiteSpace(request.BankName))
            return Results.BadRequest(new ErrorResponse("Bank name is required."));

        var bankSourceId = await service.AddBankSourceAsync(request);
        await unitOfWork.CommitAsync();
        return Results.Created($"/api/v1/banksources/{bankSourceId}", bankSourceId);
    }

    /// <summary>
    /// Updates an existing bank source with the specified ID using the provided update request data.
    /// </summary>
    /// <param name="id">The unique identifier of the bank source to update.</param>
    /// <param name="request">The update request containing the new bank source data.</param>
    /// <param name="repo">The bank source repository.</param>
    /// <param name="unitOfWork">The unit of work for committing changes.</param>
    /// <returns>A result indicating the outcome of the update operation.</returns>
    internal static async Task<IResult> UpdateBankSourceAsync(Guid id, UpdateBankSourceRequest request, [FromServices]IBankSourceRepository repo, [FromServices]IUnitOfWork unitOfWork)
    {
        if (request == null)        
            return Results.BadRequest(new ErrorResponse("Bank source data is required."));

        await repo.UpdateBankSourceAsync(id, request);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank source updated successfully."));
    }

    /// <summary>
    /// Deletes an existing bank source with the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the bank source to delete.</param>
    /// <param name="service">The bank source service.</param>
    /// <param name="unitOfWork">The unit of work for committing changes.</param>
    /// <returns>A result indicating the outcome of the delete operation.</returns>
    internal static async Task<IResult> DeleteBankSourceAsync(Guid id, [FromServices]IBankSourceService service, [FromServices]IUnitOfWork unitOfWork)
    {
        if (id == Guid.Empty)
            return Results.BadRequest(new ErrorResponse("Invalid bank source ID."));

        await service.DeleteBankSourceAsync(id);
        await unitOfWork.CommitAsync();

        return Results.Ok(new ActionResponse("Bank source deleted successfully."));
    }
}
