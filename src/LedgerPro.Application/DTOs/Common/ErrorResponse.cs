using System.Text.Json.Serialization;

namespace LedgerPro.Application.DTOs.Common;

/// <summary>
/// DTO representing an error response, containing a message describing the error.
/// </summary>
/// <param name="Error">The message describing the error.</param>
/// <param name="Details">Optional details providing additional information about the error.</param>
public record ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Initializes a new instance of the ErrorResponse class with the specified error message and optional details.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="details">Optional details providing additional information about the error.</param>
    public ErrorResponse(string error, string? details = null)
    {
        Error = error;
        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the ErrorResponse class with the specified error message and validation failures.
    /// This constructor is used to create an error response that includes validation errors, where the validation failures are 
    /// grouped by property name and included in the ValidationErrors dictionary.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="validationFailures">The collection of validation failures.</param>
    public ErrorResponse(string error, IEnumerable<FluentValidation.Results.ValidationFailure> validationFailures)
    {
        Error = error;
        Details = "One or more validation errors occurred.";

        ValidationErrors = validationFailures
            .GroupBy(v => v.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(v => v.ErrorMessage).ToArray()
            );
    }
}
