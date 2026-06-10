import { DataTable } from "@/components/DataTable";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransactionsYearRow } from "@/types/bank-transactions-year-row.types";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { columns } from "./bank-transactions-years-overview/columns";

export function BankTransactionsYearsOverview() {
    const [bankTransactionsYearsOverview, setBankTransactionsYearsOverview] = useState<BankTransactionsYearRow[]>([]);
    const { bankSourceId } = useParams();

    useEffect(() => {
        if (!bankSourceId) return;
        bankTransactionsService.getTransactionsFinancialYearsOverview(bankSourceId)
            .then(response => setBankTransactionsYearsOverview(response.data))
            .catch(error => console.error('Error fetching transactions financial years overview:', error));
    }, [bankSourceId]);

    return (
        <div className="pt-4">
            <DataTable columns={columns} data={bankTransactionsYearsOverview} />
        </div>
    );
}  
