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

interface BankTransactionCategorizedDetailsProps {
    transaction: BankTransaction;
}

export function BankTransactionCategorizedDetails({ transaction }: BankTransactionCategorizedDetailsProps) {
    const [generalLedgerItems, setGeneralLedgerItems] = useState<GeneralLedgerItemTransaction[]>([]);

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
                    <Button variant="destructive">Unreconcile</Button> : 
                    (
                        <>
                            <Button variant="destructive">Uncategorize</Button>
                            <Button variant="submit">Reconcile</Button>
                        </>                        
                    )}
            </LedgerFormFooter>
        </LedgerForm>
    );
}