using FluentValidation;

namespace LedgerPro.Application.Validation.BankTransaction;

/// <summary>
/// Custom validator to ensure that the FinancialYearEnding parameter is either null or falls within a reasonable range (e.g., between 1900 and 2100).
/// </summary>
public class GetValidFinancialYearValidator : AbstractValidator<int?>
{
    public GetValidFinancialYearValidator()
    {
        RuleFor(x => x).MustBeValidFinancialYear();
    }
}
