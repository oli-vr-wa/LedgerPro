export const GENERAL_LEDGER_ACCOUNT_TYPES = ['Asset', 'Liability', 'Equity', 'Revenue', 'Expense'] as const;

export type GeneralLedgerAccountType = typeof GENERAL_LEDGER_ACCOUNT_TYPES;

export interface GeneralLedgerAccount {
    id: number;
    name: string;
    description: string;
    accountType: GeneralLedgerAccountType;
}