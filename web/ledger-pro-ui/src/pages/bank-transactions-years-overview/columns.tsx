import { formatDate } from "@/components/data-table/utils/data-table-cell-format.utils";
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
        cell: ({ getValue }) => formatDate(getValue() as string)
    },
    {
        accessorKey: "pendingCount",
        header: "Pending Count",
    }
];