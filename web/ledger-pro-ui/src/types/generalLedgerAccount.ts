export type GeneralLedgerAccountType = 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense';

export interface GeneralLedgerAccount {
    id: number;
    name: string;
    description: string;
    accountType: GeneralLedgerAccountType;
}