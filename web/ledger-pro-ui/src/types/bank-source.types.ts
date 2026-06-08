export const BANK_TYPES = ['Generic', 'NAB', 'ANZ', 'CBA'] as const;

export type BankType = typeof BANK_TYPES[number];

export interface BankSource {
    id: string;
    bankName: string;
    accountName: string;
    accountNumber: string;
    bankType: BankType;
    generalLedgerAccountId: number;
}

export type BankSourcePayload = Omit<BankSource, 'id' | 'generalLedgerAccountId'>;