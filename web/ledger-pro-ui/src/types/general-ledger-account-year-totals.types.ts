import type { GeneralLedgerAccountType } from "./general-ledger-account.types";

export interface GeneralLedgerAccountYearTotals {
    accountId: number;
    accountName: string;
    accountType: GeneralLedgerAccountType;
    totalDebits: number;
    totalCredits: number;
}
