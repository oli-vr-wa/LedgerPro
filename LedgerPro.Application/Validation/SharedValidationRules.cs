using FluentValidation;

namespace LedgerPro.Application.Validation;

public static class SharedValidationRules
{
    /// <summary>
    /// Defines a custom validation rule for validating financial year inputs. 
    /// This rule checks that the input is either null or falls within a reasonable range (between 1900 and 2100).
    /// This validation rule can be applied to any integer property that represents a financial year, ensuring that the input 
    /// is valid and preventing invalid data from being processed in the application.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder used to define the validation rules.</param>
    /// <returns>The rule builder with the applied validation rules.</returns>
    public static IRuleBuilderOptions<T, int?> MustBeValidFinancialYear<T>(this IRuleBuilder<T, int?> ruleBuilder)
    {
        return ruleBuilder
            .Must(year => year == null || (year >= 1900 && year <= 2100))
            .WithMessage("Financial year must be between 1900 and 2100.");
    }
}
