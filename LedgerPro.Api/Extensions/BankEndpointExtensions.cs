using LedgerPro.Application.Interfaces;
using LedgerPro.Application.DTOs;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Api.Extensions
{
    public static class BankEndpointExtensions
    {
        public static void MapBankEndpoints(this IEndpointRouteBuilder app)
        {
            // Use a group for bank sources
            var group = app.MapGroup("/api/banksources").WithTags("Bank Sources");

            group.MapPost("/{id:guid}/import", async (
                Guid id,
                IFormFile file,
                IBankImportService bankImportService) =>
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest("No file uploaded.");
                
                using var stream = file.OpenReadStream();
                var request = new UploadBankStatementRequest(id, stream, file.FileName);

                var result = await bankImportService.ImportBankStatementAsync(request);

                return result.IsSuccess
                    ? Results.Ok(new { Message = "Bank statement imported successfully.", count = result.Value })
                    : Results.BadRequest(new { error = result.Error });
            })
            .DisableAntiforgery();

            group.MapGet("/debug-sources", async (IBankRepository repo) => 
            {
                var sources = await repo.GetBankSourcesAsync(); // Or a GetSources method
                return Results.Ok(sources);
            });
        }
    }
}