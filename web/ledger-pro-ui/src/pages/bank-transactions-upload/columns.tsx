import { type ColumnDef } from '@tanstack/react-table';
import { type StatementImportRow } from '@/types/statement-import-row.type';

export const columns: ColumnDef<StatementImportRow>[] = [
    {
        accessorKey: "fileName",
        header: "File Name"
    },
    {  
        accessorKey: "importDate", 
        header: "Import Date",
        cell: ({ getValue }) => {
            const dateStr = getValue() as string;
            const date = new Date(dateStr);
            return date.toLocaleString('en-AU');
        }
    },
    {
        accessorKey: "transactionCount",
        header: "Transaction Count"
    },
    {
        accessorKey: "firstTransactionDate",
        header: "First Transaction Date",
        cell: ({ getValue }) => {
            const dateStr = getValue() as string;
            const date = new Date(dateStr);
            return date.toLocaleDateString('en-AU');
        }
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
        cell: ({ getValue }) => {
            const dateStr = getValue() as string;
            const date = new Date(dateStr);
            return date.toLocaleDateString('en-AU');
        }
    }
];