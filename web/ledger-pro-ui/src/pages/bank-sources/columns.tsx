import { type ColumnDef } from "@tanstack/react-table";
import { type BankSource } from "@/types/bank-source.types";
import { sortableHeader } from "@/components/ui/TableSortButton";

export const columns: ColumnDef<BankSource>[] = [
    {
        accessorKey: "bankName",
        header: ({ column }) => sortableHeader(column, "Bank Name")
    },
    {
        accessorKey: "accountName",
        header: ({ column }) => sortableHeader(column, "Account Name"),
    },
    {
        accessorKey: "accountNumber",
        header: "Account Number",   
    },
    {
        accessorKey: "bankType",
        header: "Bank Type",
    },
    {
        accessorKey: "generalLedgerAccountId",
        header: "GL Account ID",
    }
];