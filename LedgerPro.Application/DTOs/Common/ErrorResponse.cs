namespace LedgerPro.Application.DTOs.Common;

/// <summary>
/// DTO representing an error response, containing a message describing the error.
/// </summary>
/// <param name="Error">The message describing the error.</param>
/// <param name="Details">Optional details providing additional information about the error.</param>
public record ErrorResponse(
    string Error, 
    string? Details = null
);
