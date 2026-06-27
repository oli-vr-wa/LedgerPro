import type { BankTransactionCategorizeItem } from "./bank-transaction-categorize-item.types";

export interface BankTransactionCategorize {
    bankTransactionId: string;
    categorizeItems: BankTransactionCategorizeItem[];
}