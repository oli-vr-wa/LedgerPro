import { type ColumnDef } from '@tanstack/react-table';
import { type BankTransaction } from '@/types/bank-transaction.types';
import { formatCurrency, formatDate } from '@/components/data-table/utils/data-table-cell-format.utils';

export const columns: ColumnDef<BankTransaction>[] = [
    {
        accessorKey: "transactionDate",
        header: "Transaction Date",
        cell: ({ getValue }) => formatDate(getValue() as string)
    },
    {
        accessorKey: "description",
        header: "Description"
    },    
    {
        accessorKey: "transactionType",
        header: "Type"
    },
    {
        accessorKey: "amount",
        header: "Amount",
        cell: ({ getValue }) => formatCurrency(getValue() as number)
    },
    {
        accessorKey: "status",
        header: "Status"
    },
    {
        accessorKey: "generalLedgerClassification",
        header: "GL Classification"
    }
];