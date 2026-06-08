export interface BankSourceTransactionsRow {
    bankSourceId: string;
    bankSourceName: string;
    bankAccountName: string;
    lastImportDate: string | null;
    lastTransactionDate: string | null;
}