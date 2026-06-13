import type { BankTransactionsYearRow } from "@/types/bank-transactions-year-row.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<BankTransactionsYearRow>[] = [
    {
        accessorKey: "yearEnding",
        header: "Financial Year Ending",
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
        cell: ({ getValue }) => {
            const dateStr = getValue() as string;
            const date = new Date(dateStr);
            return date.toLocaleDateString('en-AU');
        }
    },
    {
        accessorKey: "pendingCount",
        header: "Pending Count",
    }
];