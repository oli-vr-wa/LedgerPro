import { formatCurrency } from "@/components/data-table/utils/data-table-cell-format.utils";
import type { GeneralLedgerAccountYearTotals } from "@/types/general-ledger-account-year-totals.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<GeneralLedgerAccountYearTotals>[] = [
    {
        accessorKey: "accountId",
        header: "Account #",
    },
    {
        accessorKey: "accountName",
        header: "Account Name",
    },
    {
        accessorKey: "accountType",
        header: "Account Type",
    },
    {
        accessorKey: "totalDebits",
        header: "Total Debits",
        cell: ({ getValue }) => formatCurrency(getValue() as number)
    },  
    {
        accessorKey: "totalCredits",
        header: "Total Credits",
        cell: ({ getValue }) => formatCurrency(getValue() as number)
    }
];