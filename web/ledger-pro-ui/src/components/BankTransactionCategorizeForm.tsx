import type { BankTransaction } from "@/types/bank-transaction.types";
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from "./ui/form-fields/LedgerForm";
import { bankTransactionClassificationSchema, type BankTransactionClassificationFormData } from "@/schemas/bank-transaction-classification.schema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod.js";
import type z from "zod";
import { LedgerTextDisplay } from "./ui/form-fields/LedgerTextDisplay";
import type { GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import { useEffect, useState } from "react";
import { generalLedgerAccountService } from "@/services/generalLedgerAccountService";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { LedgerSelect } from "./ui/form-fields/LedgerSelect";
import { Button } from "./ui/button";
import { showApiToast } from "@/lib/toast-utils";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransactionCategorize } from "@/types/bank-transaction-categorize.types";
import { formatCurrency, formatDate } from "./data-table/utils/data-table-cell-format.utils";

interface BankTransactionCategorizeFormProps {
    transaction: BankTransaction;
    closeDialog: () => void;
}

export function BankTransactionCategorizeForm({ transaction, closeDialog }: BankTransactionCategorizeFormProps) {
    const [accounts, setAccounts] = useState<GeneralLedgerAccount[]>([]);
    const [isLoadingAccounts, setIsLoadingAccounts] = useState(true);

    // React Query client for invalidating queries after mutations
    const queryClient = useQueryClient();

    // Fetch GL accounts on component mount to populate the select dropdown
        useEffect(() => {
            // Fetch GL accounts for the select dropdown, used in both add and edit modes
            generalLedgerAccountService.getAll()
                .then(response => {
                    setAccounts(response.data);
                    setIsLoadingAccounts(false);
                })
                .catch(() => {
                    setIsLoadingAccounts(false);
                });
        }, []);

    const defaultValues: z.infer<typeof bankTransactionClassificationSchema> = {
        generalLedgerClassification: transaction.generalLedgerClassification
    };

    const form = useForm<BankTransactionClassificationFormData>({ 
        resolver: zodResolver(bankTransactionClassificationSchema),
        defaultValues
    });

    const {mutate: categorizeTransaction, isPending} = useMutation({
        mutationFn: (data: BankTransactionCategorize) => {
            return bankTransactionsService.categorizeTransaction(transaction.id, data);
        },
        onSuccess: () => {
            showApiToast('Transaction categorized successfully');
            queryClient.invalidateQueries({ queryKey: ['bankTransactions']});
            closeDialog();
        },
        onError: () => {
            showApiToast('Failed to categorize transaction', 'error');
        }
    });

    const onSubmit = (data: BankTransactionClassificationFormData) => {
        const payload: BankTransactionCategorize = {
            transactionId: transaction.id,
            generalLedgerAccountId: Number(data.generalLedgerClassification)
        };
        categorizeTransaction(payload);
    };

    if (isLoadingAccounts) return <div>Loading...</div>;

    return (        
        <LedgerForm onSubmit={form.handleSubmit(onSubmit)}>
            <LedgerFormBody>

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
                    label="Status"
                    value={transaction.status} />

                <LedgerSelect
                    label="General Ledger Classification"
                    name="generalLedgerClassification"
                    readOnly={transaction.status === 'Reconciled'}
                    control={form.control as any}
                    options={accounts.map(account => ({ value: account.id.toString(), label: `${account.id} - ${account.name}` }))}
                    placeholder="Select a GL Account" />

            </LedgerFormBody>
            <LedgerFormFooter>
                {transaction.status === 'Reconciled' && <Button variant="destructive">Unreconcile</Button>}
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>
                {transaction.status !== 'Reconciled' && <Button type="submit" variant="submit" disabled={isPending}>{isPending ? 'Saving...' : 'Save'}</Button>}
            </LedgerFormFooter>
        </LedgerForm>
    );
};