import type { GeneralLedgerYearRow } from "@/types/general-ledger-year-row.types";
import type { ColumnDef } from "@tanstack/react-table";

export const columns: ColumnDef<GeneralLedgerYearRow>[] = [
    {
        accessorKey: "yearEnding",
        header: "Financial Year Ending",
    }
];    
