import { type ColumnDef } from "@tanstack/react-table";
import { type GeneralLedgerAccount } from "@/types/general-ledger-account.types";

export const columns: ColumnDef<GeneralLedgerAccount>[] = [
    {
        accessorKey: "id",
        header: "Account Code",
    },
    {
        accessorKey: "name",
        header: "Account Name",
    },
    {
        accessorKey: "description",
        header: "Description",
    },
    {
        accessorKey: "accountType",
        header: "Account Type",
    }
];