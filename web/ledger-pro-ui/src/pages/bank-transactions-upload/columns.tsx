import { type ColumnDef } from '@tanstack/react-table';
import { type StatementImportRow } from '@/types/statement-import-row.type';
import { formatDate, formatDateTime } from '@/components/data-table/utils/data-table-cell-format.utils';

export const columns: ColumnDef<StatementImportRow>[] = [
    {
        accessorKey: "fileName",
        header: "File Name"
    },
    {  
        accessorKey: "importDate", 
        header: "Import Date",
        cell: ({ getValue }) => formatDateTime(getValue() as string)
    },
    {
        accessorKey: "transactionCount",
        header: "Transaction Count"
    },
    {
        accessorKey: "firstTransactionDate",
        header: "First Transaction Date",
        cell: ({ getValue }) => formatDate(getValue() as string)
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
        cell: ({ getValue }) => formatDate(getValue() as string)
    }
];