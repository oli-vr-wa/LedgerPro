import { DataTable } from "@/components/DataTable";
import { bankSourceService } from "@/services/bankSourceService";
import { useQuery } from "@tanstack/react-query";
import { columns } from "./bank-account-selection/columns";
import { useState } from "react";
import type { BankSourceTransactionsRow } from "@/types/bank-source-transactions-row.types";
import { useNavigate } from "react-router-dom";

export function BankAccountSelection() {
    const navigate = useNavigate();
    const [selectBankSource, setSelectBankSource] = useState<BankSourceTransactionsRow | null>(null);

    const {data: bankSourcesTransactionsRows, isLoading} = useQuery({
        queryKey: ['bankSourcesTransactionsRows'],
        queryFn: () => bankSourceService.getTransactionsOverview().then(response => response.data)
    });    

    const handleRowClick = (bankSource: BankSourceTransactionsRow) => {
        setSelectBankSource(bankSource);
        navigate(`/transactions/${bankSource.bankSourceId}`, { state: { displayName: `${bankSource.bankSourceName} - ${bankSource.bankAccountName}` } });
    }

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Select Bank Account</h2>
            <p className="mb-6 text-sm text-muted-foreground">Choose a bank account to view or manage transactions.</p>

            <DataTable columns={columns} data={bankSourcesTransactionsRows ?? []} onRowClick={handleRowClick} />
        </div>
    );
};