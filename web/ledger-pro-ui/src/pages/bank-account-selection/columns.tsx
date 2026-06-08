import { type ColumnDef } from '@tanstack/react-table';
import { type BankSourceTransactionsRow } from '@/types/bank-source-transactions-row.types';

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
    },
    {
        accessorKey: "lastTransactionDate",
        header: "Last Transaction Date",
    }
];