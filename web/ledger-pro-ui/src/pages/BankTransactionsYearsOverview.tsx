import { DataTable } from "@/components/DataTable";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransactionsYearRow } from "@/types/bank-transactions-year-row.types";
import { useEffect, useState } from "react";
import { columns } from "./bank-transactions-years-overview/columns";

export const BankTransactionsYearOverviewProps = {
    bankSourceId: '',
    onRowClick: (_bankTransactionsYearRow: BankTransactionsYearRow) => {}
};

export function BankTransactionsYearsOverview({ bankSourceId, onRowClick }: typeof BankTransactionsYearOverviewProps) {
    const [bankTransactionsYearsOverview, setBankTransactionsYearsOverview] = useState<BankTransactionsYearRow[]>([]);

    useEffect(() => {
        if (!bankSourceId) return;
        bankTransactionsService.getTransactionsFinancialYearsOverview(bankSourceId)
            .then(response => setBankTransactionsYearsOverview(response.data))
            .catch(error => console.error('Error fetching transactions financial years overview:', error));
    }, [bankSourceId]);

    return (
        <div className="pt-4">
            <DataTable columns={columns} data={bankTransactionsYearsOverview} onRowClick={onRowClick} />
        </div>
    );
}  
