export type BankType = 'Generic' | 'NAB' | 'ANZ' | 'CBA';

export interface BankSource {
    id: string;
    bankName: string;
    accountName: string;
    accountNumber: string;
    bankType: BankType;
    generalLedgerAccountId: number;
}