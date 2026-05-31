import { type ColumnDef } from "@tanstack/react-table";
import { type BankSource } from "@/types/bank-source.types";

export const columns: ColumnDef<BankSource>[] = [
    {
        accessorKey: "bankName",
        header: "Bank Name",
    },
    {
        accessorKey: "accountName",
        header: "Account Name",
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