export const TRANSACTION_STATUSES = ['Pending', 'Categorized', 'Reconciled'] as const;

export type TransactionStatus = typeof TRANSACTION_STATUSES[number];

export interface BankTransaction {
    id: string;
    transactionDate: string;
    description: string;
    amount: number;
    transactionType: string;
    status: TransactionStatus;
    bankSourceId: string;
    statementImportId: string;
}
