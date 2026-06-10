import type { BankTransactionsYearRow } from "@/types/bank-transactions-year-row.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<BankTransactionsYearRow>[] = [
    {
        accessorKey: "year",
        header: "Year",
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
    },
    {
        accessorKey: "pendingCount",
        header: "Pending Count",
    }
];