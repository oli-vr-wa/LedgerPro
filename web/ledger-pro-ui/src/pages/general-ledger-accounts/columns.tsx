import { type ColumnDef } from "@tanstack/react-table";
import { type GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import { sortableHeader } from "@/components/ui/TableSortButton";

export const columns: ColumnDef<GeneralLedgerAccount>[] = [
    {
        accessorKey: "id",
        header: ({ column }) => sortableHeader(column, "ID")
    },
    {
        accessorKey: "name",
        header: ({ column }) => sortableHeader(column, "Name")
    },
    {
        accessorKey: "description",
        header: "Description",
    },
    {
        accessorKey: "accountType",
        header: ({ column }) => sortableHeader(column, "Account Type")
    }
];