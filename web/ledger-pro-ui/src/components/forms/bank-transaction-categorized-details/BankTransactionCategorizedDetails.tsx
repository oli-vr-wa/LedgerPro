import { DataTable } from "@/components/data-table/DataTable";
import { formatCurrency, formatDate } from "@/components/data-table/utils/data-table-cell-format.utils";
import { Button } from "@/components/ui/button";
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from "@/components/ui/form-fields/LedgerForm";
import { LedgerTextDisplay } from "@/components/ui/form-fields/LedgerTextDisplay";
import { showApiToast } from "@/lib/toast-utils";
import { generalLedgerItemService } from "@/services/generalLedgerItemService";
import type { BankTransaction } from "@/types/bank-transaction.types";
import type { GeneralLedgerItemTransaction } from "@/types/general-ledger-item-transaction.types";
import { useEffect, useState } from "react";
import { columns } from "./columns";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import { useQueryClient } from "@tanstack/react-query";

interface BankTransactionCategorizedDetailsProps {
    transaction: BankTransaction;
    closeDialog: () => void;
}

export function BankTransactionCategorizedDetails({ transaction, closeDialog }: BankTransactionCategorizedDetailsProps) {
    const [generalLedgerItems, setGeneralLedgerItems] = useState<GeneralLedgerItemTransaction[]>([]);

    // React Query client for invalidating queries after uncategorizing/reconciling/unreconciling
    const queryClient = useQueryClient();

    useEffect(() => {
        // Fetch GL items for the bank transaction when the component mounts
        generalLedgerItemService.getAllForBankTransaction(transaction.id)
            .then(response => {
                setGeneralLedgerItems(response.data);
            })
            .catch(() => {
                showApiToast("Failed to fetch general ledger items for the transaction.", "error");
            });
    }, [transaction.id]);
    
    const handleUncategorize = (e: React.MouseEvent) => {
        e.preventDefault();
        
        // Call the API to uncategorize the transaction
        bankTransactionsService.uncategorizeTransaction(transaction.id)
            .then(() => {
                showApiToast("Transaction uncategorized successfully.");
                queryClient.invalidateQueries({ queryKey: ['bankTransactions']});
                closeDialog();
            })
            .catch(() => {
                showApiToast("Failed to uncategorize the transaction.", "error");
            });
    };

    const handleReconcile = (e: React.MouseEvent) => {
        e.preventDefault();
        
        bankTransactionsService.reconcileTransaction(transaction.id)
            .then(() => {
                showApiToast("Transaction reconciled successfully.");
                queryClient.invalidateQueries({ queryKey: ['bankTransactions']});
                closeDialog();
            })
            .catch((error) => {
                //console.log(error.response.data);
                showApiToast("Failed to reconcile the transaction.", error.response.statusText, { error });
            });
    };

    const handleUnreconcile = (e: React.MouseEvent) => {
        e.preventDefault();

        bankTransactionsService.unreconcileTransaction(transaction.id)
            .then(() => {
                showApiToast("Transaction unreconciled successfully.");
                queryClient.invalidateQueries({ queryKey: ['bankTransactions']});
                closeDialog();
            })
            .catch((error) => {                
                showApiToast("Failed to unreconcile the transaction.", error, { error });
            });

    };

    return (
        <LedgerForm onSubmit={() => {}}>
            <LedgerFormBody>
                <div className="flex gap-4 flex-direction-row">
                    <LedgerTextDisplay
                        label="Transaction Date"
                        value={formatDate(transaction.transactionDate)} />
                    
                    <LedgerTextDisplay 
                        label="Description"
                        value={transaction.description} />

                    <LedgerTextDisplay 
                        label="Transaction Type"
                        value={transaction.transactionType} />

                    <LedgerTextDisplay 
                        label="Amount"
                        value={formatCurrency(transaction.amount)} />

                    <LedgerTextDisplay
                        label="GL Classification"
                        value={transaction.generalLedgerClassification} />                    

                </div>

                <DataTable columns={columns} data={generalLedgerItems} />

            </LedgerFormBody>
            <LedgerFormFooter>                
                {transaction.status === 'Reconciled' ? 
                    <Button variant="destructive" onClick={handleUnreconcile}>Unreconcile</Button> : 
                    (
                        <>
                            <Button variant="destructive" onClick={handleUncategorize}>Uncategorize</Button>
                            <Button variant="submit" onClick={handleReconcile}>Reconcile</Button>
                        </>                        
                    )}
            </LedgerFormFooter>
        </LedgerForm>
    );
}