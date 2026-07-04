import { DataTable } from "@/components/data-table/DataTable";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransaction } from "@/types/bank-transaction.types";
import { useState } from "react";
import { columns } from "./columns";
import { LedgerDialog } from "@/components/ui/LedgerDialog";
import { BankTransactionCategorizeForm } from "@/components/forms/bank-transaction-categorize-form/BankTransactionCategorizeForm";
import { useQuery } from "@tanstack/react-query";
import { BankTransactionCategorizedDetails } from "@/components/forms/bank-transaction-categorized-details/BankTransactionCategorizedDetails";

export const BankTransactionsYearProps = {
    bankSourceId: '',
    year: 0
};

export function BankTransactionsYear({ bankSourceId, year }: typeof BankTransactionsYearProps) {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [selectedTransaction, setSelectedTransaction] = useState<BankTransaction | undefined>(undefined);
    const { data: bankTransactionsState = [], isLoading } = useQuery({
        queryKey: ['bankTransactions', bankSourceId, year],
        queryFn: () => bankTransactionsService.getTransactionsByBankSourceIdAndYear(bankSourceId, year)
            .then(response => response.data),
        enabled: !!bankSourceId && !!year
    });

    const handleRowClick = (transaction: BankTransaction) => {
        setSelectedTransaction(transaction);
        setIsDialogOpen(true);
    };

    const handleOpenDialog = (isOpen: boolean) => {
        setIsDialogOpen(isOpen);
    };

    const setStatusRowClassName = (transaction: BankTransaction) => {
        switch (transaction.status) {
            case 'Pending':
                return 'bg-red-100';
            case 'Reconciled':
                return 'bg-green-100';
            default:
                return '';
        }
    };   

    return (
        <div className="pt-4">
            <h2 className="text-2xl font-bold mb-4">Bank Transactions for {year}</h2>
            <DataTable columns={columns} data={bankTransactionsState} loading={isLoading} getRowClassName={setStatusRowClassName} onRowClick={handleRowClick} />

            <LedgerDialog title="Transaction Details" isOpen={isDialogOpen} setIsOpen={handleOpenDialog} showTrigger={false} size="medium">
                {selectedTransaction?.status === 'Pending' ? (
                    <BankTransactionCategorizeForm transaction={selectedTransaction!} closeDialog={() => handleOpenDialog(false)} />
                ) : (
                    <BankTransactionCategorizedDetails transaction={selectedTransaction!} closeDialog={() => handleOpenDialog(false)} />
                )}
            </LedgerDialog>
        </div>
    );
}