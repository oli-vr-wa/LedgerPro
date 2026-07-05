import { generalLedgerService } from "@/services/generalLedgerService";
import type { GeneralLedgerYearRow } from "@/types/general-ledger-year-row.types";
import { useQuery } from "@tanstack/react-query";
import { columns } from "./columns";
import { DataTable } from "@/components/data-table/DataTable";
import { useNavigate } from "react-router-dom";


export function GeneralLedgerYearsOverview() {
    const navigate = useNavigate();
    const { data: generalLedgerYearsOverviewState = [], isLoading } = useQuery({
        queryKey: ['generalLedgerYearsOverview'],
        queryFn: () => generalLedgerService.getGeneralLedgerFinancialYearsOverview()
            .then(response => response.data),
            enabled: true,
    });

    const handleRowClick = (row: GeneralLedgerYearRow) => {
        // Navigate to the GeneralLedgerAccountsYearTotals page for the selected financial year
        navigate(`/generalLedger/${row.yearEnding}`);
    }

    return (
        <div>
            <h4 className="text-lg font-bold mb-6">Financial Years</h4>

            <DataTable columns={columns} data={generalLedgerYearsOverviewState} loading={isLoading} onRowClick={handleRowClick} />
        </div>
    );
};
