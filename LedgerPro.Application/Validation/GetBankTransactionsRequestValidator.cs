using FluentValidation;

namespace LedgerPro.Application.Validation;

/// <summary>
/// Request model for retrieving bank transactions, which includes the BankSourceId to specify the source of the transactions and 
/// an optional FinancialYearEnding parameter to filter transactions by financial year.
/// </summary>
/// <param name="BankSourceId">The ID of the bank source for which to retrieve transactions.</param>
/// <param name="FinancialYearEnding">The ending year of the financial year to be reported.</param>
public record GetBankTransactionsRequest(Guid BankSourceId, int? FinancialYearEnding);

/// <summary>
/// Validator for the GetBankTransactionsRequest, which ensures that the BankSourceId is not empty and that 
/// the FinancialYearEnding is either null or a valid financial year (between 1900 and 2100).
/// </summary>
public class GetBankTransactionsRequestValidator : AbstractValidator<GetBankTransactionsRequest>
{
    public GetBankTransactionsRequestValidator()
    {
        RuleFor(x => x.BankSourceId).NotEmpty().WithMessage("Bank source ID is required.");
        RuleFor(x => x.FinancialYearEnding).MustBeValidFinancialYear();
    }
}
