export const BANK_TRANSACTION_MAPPING_STRATEGIES = ['Contains', 'StartsWith', 'Exact', 'Regex'] as const;

export type BankTransactionMappingStrategy = (typeof BANK_TRANSACTION_MAPPING_STRATEGIES)[number];

export interface BankTransactionMapping {
    id: string;
    searchTerm: string;
    matchStrategy: BankTransactionMappingStrategy;
    targetGeneralLedgerAccountId: number;
    descriptionTemplate: string;
    referenceTemplate: string;
    priority: number;
}