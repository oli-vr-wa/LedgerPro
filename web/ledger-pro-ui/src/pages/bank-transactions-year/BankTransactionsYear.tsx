import { DataTable } from "@/components/DataTable";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransaction } from "@/types/bank-transaction.types";
import { useEffect, useState } from "react";
import { columns } from "./columns";

export const BankTransactionsYearProps = {
    bankSourceId: '',
    year: 0
};

export function BankTransactionsYear({ bankSourceId, year }: typeof BankTransactionsYearProps) {
    const [bankTransactionsState, setBankTransactionsState] = useState<BankTransaction[]>([]);

    useEffect(() => {
        if (!bankSourceId || !year) return;
        bankTransactionsService.getTransactionsByBankSourceIdAndYear(bankSourceId, year)
            .then(response => setBankTransactionsState(response.data))
            .catch(error => console.error('Error fetching transactions for year:', error));
    }, [bankSourceId, year]);

    return (
        <div className="pt-4">
            <h2 className="text-2xl font-bold mb-4">Bank Transactions for {year}</h2>
            <DataTable columns={columns} data={bankTransactionsState} />
        </div>
    );
}