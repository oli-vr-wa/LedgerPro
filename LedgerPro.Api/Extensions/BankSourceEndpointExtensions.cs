using LedgerPro.Application.Interfaces;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;

namespace LedgerPro.Api.Extensions;

public static class BankSourceEndpointExtensions
{    
    public static IEndpointRouteBuilder MapBankSourcesEndpoints(this IEndpointRouteBuilder app)
    {
        // Use a group for bank sources
        var group = app.MapGroup("/api/v1/banksources").WithTags("Bank Sources");

        group.MapPost("/{id:guid}/import-statements", ImportBankStatementAsync).DisableAntiforgery();
        group.MapGet("/", GetBankSourcesAsync);
        group.MapPost("/", AddBankSourceAsync);

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
    internal static async Task<IResult> ImportBankStatementAsync(Guid id, IFormFile file, IBankImportService bankImportService, IUnitOfWork unitOfWork)
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
    internal static async Task<IResult> GetBankSourcesAsync(IBankSourceRepository repo)
    {
        var sources = await repo.GetBankSourcesAsync();
        return Results.Ok(sources);
    }

    /// <summary>
    /// Adds a new bank source to the database.
    /// </summary>
    /// <param name="bankSource">The bank source to add</param>
    /// <param name="repo">The bank source repository</param>
    /// <returns>Result indicating the outcome of the operation</returns>
    internal static async Task<IResult> AddBankSourceAsync(BankSource bankSource, IBankSourceRepository repo)
    {
        bool isInUse = await repo.IsBankSourceNameInUseAsync(bankSource.BankName);
        if (isInUse)        
            return Results.BadRequest(new ErrorResponse($"Bank source with name '{bankSource.BankName}' is already in use and cannot be added."));

        await repo.AddBankSourceAsync(bankSource);
        return Results.Created($"/api/v1/banksources/{bankSource.Id}", bankSource);
    }
}
