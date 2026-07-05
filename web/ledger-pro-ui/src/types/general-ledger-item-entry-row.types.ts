export interface GeneralLedgerItemEntryRow {
    id: string;
    transactionDate: string;
    reference: string;
    description: string;
    debit: number;
    credit: number;
    balance: number;
}