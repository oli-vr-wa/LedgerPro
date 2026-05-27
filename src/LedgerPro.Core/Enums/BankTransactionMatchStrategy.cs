namespace LedgerPro.Core.Enums;

public enum BankTransactionMatchStrategy
{
    Contains, // Match if the bank transaction description contains the search term
    StartsWith, // Match if the bank transaction description starts with the search term
    Exact, // Match if the bank transaction description exactly matches the search term
    Regex // Match if the bank transaction description matches the regular expression pattern defined in the search term
}
