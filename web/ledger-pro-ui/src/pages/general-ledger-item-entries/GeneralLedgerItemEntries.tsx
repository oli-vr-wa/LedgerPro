import { DataTable } from "@/components/data-table/DataTable";
import { columns } from "./columns";
import { generalLedgerService } from "@/services/generalLedgerService";
import { useQuery } from "@tanstack/react-query";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

export const GeneralLedgerItemEntriesProps = {
    financialYear: 0,
    accountId: 0,
};

export function GeneralLedgerItemEntries({ financialYear, accountId }: typeof GeneralLedgerItemEntriesProps) {
    const navigate = useNavigate();
    const location = useLocation();
    const { data: generalLedgerItemEntries = [], isLoading } = useQuery({
        queryKey: ['generalLedgerItemEntries', financialYear, accountId],
        queryFn: () => generalLedgerService.getGeneralLedgerItemsForFinancialYearAndAccount(financialYear, accountId)
            .then(response => response.data),
        enabled: !!financialYear && !!accountId
    });
    const accountName = location.state?.accountName || 'Unknown Account'; // Replace with actual account name retrieval logic if available

    return (
        <div>
            <Button variant="ghost" onClick={() => navigate(`/generalLedger/${financialYear}`)} className="mb-2">
                <ArrowLeft className="mr-2 h-4 w-4" /> Back to Accounts
            </Button>

            <h4 className="text-lg font-bold mb-6">General Ledger Item Entries for {accountName}</h4>

            <DataTable columns={columns} data={generalLedgerItemEntries} loading={isLoading} />
        </div>
    );
}