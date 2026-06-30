import { formatCurrency } from "@/components/data-table/utils/data-table-cell-format.utils";
import type { GeneralLedgerItemTransaction } from "@/types/general-ledger-item-transaction.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<GeneralLedgerItemTransaction>[] = [
    {
         accessorKey: 'description',
         header: 'Description',
    },
    {
        accessorKey: 'reference',
        header: 'Reference',
    },
    {
        accessorKey: 'generalLedgerAccountName',
        header: 'GL Classification',
    },
    {
        accessorKey: 'amount',
        header: 'Amount',
        cell: ({ getValue }) => formatCurrency(getValue() as number) 
    }
];