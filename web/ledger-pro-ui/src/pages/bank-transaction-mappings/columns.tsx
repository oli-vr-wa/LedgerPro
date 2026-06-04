import { type ColumnDef } from '@tanstack/react-table';
import { type BankTransactionMapping } from '@/types/bank-transaction-mapping.types';
import { sortableHeader } from '@/components/ui/TableSortButton';

export const columns: ColumnDef<BankTransactionMapping>[] = [
    {
        accessorKey: 'searchTerm',
        header: ({ column }) => sortableHeader(column, 'Search Term')
    },
    {
        accessorKey: 'matchStrategy',
        header: ({ column }) => sortableHeader(column, 'Match Strategy')
    },
    {
        accessorKey: 'targetGeneralLedgerAccountId',
        header: ({ column }) => sortableHeader(column, 'General Ledger Account')
    },
    {
        accessorKey: 'descriptionTemplate',
        header: 'Description Template'
    },
    {
        accessorKey: 'referenceTemplate',
        header: 'Reference Template'
    },
    {
        accessorKey: 'priority',
        header: ({ column }) => sortableHeader(column, 'Priority')
    }
];