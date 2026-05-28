namespace LedgerPro.Application.DTOs.Common;

/// <summary>
/// DTO representing a generic action response, containing a message describing the result of the action.
/// </summary>
/// <param name="Message">The message describing the result of the action.</param>
public record ActionResponse(string Message);
