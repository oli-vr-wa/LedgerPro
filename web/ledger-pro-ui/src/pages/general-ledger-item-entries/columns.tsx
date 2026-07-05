import { formatCurrency, formatDate } from "@/components/data-table/utils/data-table-cell-format.utils";
import type { GeneralLedgerItemEntryRow } from "@/types/general-ledger-item-entry-row.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<GeneralLedgerItemEntryRow>[] = [
    {
        accessorKey: "transactionDate",
        header: "Transaction Date",
        cell: ({ getValue }) => formatDate(getValue() as string)
    },  
    {
        accessorKey: "reference",
        header: "Reference",
    },
    {
        accessorKey: "description",
        header: "Description",
    },
    {
        accessorKey: "debit",
        header: "Debit",
        cell: ({ getValue }) => getValue() as number === 0 ? "-" : formatCurrency(getValue() as number)
    },
    {
        accessorKey: "credit",
        header: "Credit",
        cell: ({ getValue }) => getValue() as number === 0 ? "-" : formatCurrency(getValue() as number)
    },
    {
        accessorKey: "balance",
        header: "Balance",
        cell: ({ getValue }) => formatCurrency(getValue() as number)
    }
];