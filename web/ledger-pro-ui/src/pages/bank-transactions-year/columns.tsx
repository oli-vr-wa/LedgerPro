import { type ColumnDef } from '@tanstack/react-table';
import { type BankTransaction } from '@/types/bank-transaction.types';

export const columns: ColumnDef<BankTransaction>[] = [
    {
        accessorKey: "transactionDate",
        header: "Transaction Date",
        cell: ({ getValue }) => {
            const dateStr = getValue() as string;
            const date = new Date(dateStr);
            return date.toLocaleDateString('en-AU');
        }
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
        cell: ({ getValue }) => {
            const amount = getValue() as number;
            return amount.toLocaleString('en-AU', { style: 'currency', currency: 'AUD' });
        }
    },
    {
        accessorKey: "status",
        header: "Status"
    }
];