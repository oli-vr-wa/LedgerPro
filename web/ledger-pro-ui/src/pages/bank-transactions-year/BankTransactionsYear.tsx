import { DataTable } from "@/components/data-table/DataTable";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransaction } from "@/types/bank-transaction.types";
import { useEffect, useState } from "react";
import { columns } from "./columns";
import { LedgerDialog } from "@/components/ui/LedgerDialog";
import { BankTransactionCategorizeForm } from "@/components/BankTransactionCategorizeForm";

export const BankTransactionsYearProps = {
    bankSourceId: '',
    year: 0
};

export function BankTransactionsYear({ bankSourceId, year }: typeof BankTransactionsYearProps) {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [selectedTransaction, setSelectedTransaction] = useState<BankTransaction | undefined>(undefined);
    const [bankTransactionsState, setBankTransactionsState] = useState<BankTransaction[]>([]);

    useEffect(() => {
        if (!bankSourceId || !year) return;
        bankTransactionsService.getTransactionsByBankSourceIdAndYear(bankSourceId, year)
            .then(response => setBankTransactionsState(response.data))
            .catch(error => console.error('Error fetching transactions for year:', error));
    }, [bankSourceId, year]);

    const handleRowClick = (transaction: BankTransaction) => {
        setSelectedTransaction(transaction);
        setIsDialogOpen(true);
    };

    const handleOpenDialog = (isOpen: boolean) => {
        setIsDialogOpen(isOpen);
    };

    const setPendingRowClassName = (transaction: BankTransaction) => {
        return transaction.status === 'Pending' ? 'bg-red-100' : '';
    };   

    return (
        <div className="pt-4">
            <h2 className="text-2xl font-bold mb-4">Bank Transactions for {year}</h2>
            <DataTable columns={columns} data={bankTransactionsState} getRowClassName={setPendingRowClassName} onRowClick={handleRowClick} />

            <LedgerDialog title="Transaction Details" isOpen={isDialogOpen} setIsOpen={handleOpenDialog} showTrigger={false} size="medium">
                <BankTransactionCategorizeForm transaction={selectedTransaction!} closeDialog={() => handleOpenDialog(false)} />
            </LedgerDialog>
        </div>
    );
}