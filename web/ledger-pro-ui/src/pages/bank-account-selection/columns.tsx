import { type ColumnDef } from '@tanstack/react-table';
import { type BankSourceTransactionsRow } from '@/types/bank-source-transactions-row.types';
import { formatDate, formatDateTime } from '@/components/data-table/utils/data-table-cell-format.utils';

export const columns: ColumnDef<BankSourceTransactionsRow>[] = [
    {
        accessorKey: "bankSourceName",
        header: "Bank Source Name"
    },
    {  
        accessorKey: "bankAccountName",
        header: "Bank Account Name"
    },
    {
        accessorKey: "lastImportDate",
        header: "Last Import Date",
        cell: ({ getValue }) => formatDateTime(getValue() as string)
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
        cell: ({ getValue }) => formatDate(getValue() as string)
    }
];