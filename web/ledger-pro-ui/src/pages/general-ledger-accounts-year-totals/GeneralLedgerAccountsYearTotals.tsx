import { DataTable } from "@/components/data-table/DataTable";
import { generalLedgerService } from "@/services/generalLedgerService";
import { useQuery } from "@tanstack/react-query";
import { columns } from "./columns";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";

export const GeneralLedgerAccountsYearTotalsProps = {
    financialYear: 0,
};

export function GeneralLedgerAccountsYearTotals({ financialYear }: typeof GeneralLedgerAccountsYearTotalsProps) {
    const navigate = useNavigate();
    const { data: generalLedgerAccounts = [], isLoading } = useQuery({
        queryKey: ['generalLedgerAccounts', financialYear],
        queryFn: () => generalLedgerService.getGeneralLedgerAccountsForFinancialYear(financialYear)
            .then(response => response.data),
        enabled: !!financialYear
    });

    const handleRowClick = (row: any) => {
        // Navigate to the GeneralLedgerItemEntries page for the selected account and financial year
        navigate(`/generalLedger/${financialYear}/account/${row.accountId}`, { state: { accountName: row.accountName } });
    }

    return (
        <div>
            <Button variant="ghost" onClick={() => navigate("/generalLedger")} className="mb-2">
                <ArrowLeft className="mr-2 h-4 w-4" /> Back to Overview
            </Button>

            <h4 className="text-lg font-bold mb-6">General Ledger Accounts for {financialYear}</h4>

            <DataTable columns={columns} data={generalLedgerAccounts} loading={isLoading} onRowClick={handleRowClick} />
        </div>
    );
}