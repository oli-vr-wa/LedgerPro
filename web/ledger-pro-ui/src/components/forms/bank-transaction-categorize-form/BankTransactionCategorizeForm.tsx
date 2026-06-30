import type { BankTransaction } from "@/types/bank-transaction.types";
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from "../../ui/form-fields/LedgerForm";
import { createCategorizeSplitSchema, type BankTransactionCategorizeFormData } from "@/schemas/transaction-categorize-item.schemas";
import { useFieldArray, useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod.js";
import { LedgerTextDisplay } from "../../ui/form-fields/LedgerTextDisplay";
import type { GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import { useEffect, useMemo, useState } from "react";
import { generalLedgerAccountService } from "@/services/generalLedgerAccountService";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Button } from "../../ui/button";
import { showApiToast } from "@/lib/toast-utils";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransactionCategorize } from "@/types/bank-transaction-categorize.types";
import { formatCurrency, formatDate } from "../../data-table/utils/data-table-cell-format.utils";
import { DataTable } from "../../data-table/DataTable";
import { Field } from "../../ui/field";
import { getItemsColumns } from "./columns";

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

    const defaultValues: BankTransactionCategorizeFormData = {
        items: [{
            generalLedgerAccountId: '',
            description: '',
            reference: '',
            amount: transaction.amount
        }]
    };

    const itemsColumns = useMemo(() => {
        return getItemsColumns();
    }, []);

    const form = useForm<BankTransactionCategorizeFormData>({ 
        resolver: zodResolver(createCategorizeSplitSchema(transaction.amount)),
        defaultValues: defaultValues as BankTransactionCategorizeFormData,
        mode: "onChange",
        reValidateMode: "onChange"
    });

    const {mutate: categorizeTransaction, isPending} = useMutation({
        mutationFn: (data: BankTransactionCategorize) => {
            return bankTransactionsService.categorizeTransaction(data);
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

    const onSubmit = (data: BankTransactionCategorizeFormData) => {
        const payload: BankTransactionCategorize = {
            bankTransactionId: transaction.id,
            categorizeItems: data.items.map(item => ({
                generalLedgerAccountId: parseInt(item.generalLedgerAccountId, 10),
                Description: item.description,
                Reference: item.reference || '',
                Amount: item.amount
            }))
        };
        categorizeTransaction(payload);
    };   

    // Handler for adding a new row to the items array when the user presses Enter in the last row
    const onRowEnter = () => {
        append({
            generalLedgerAccountId: '',
            description: '',
            reference: '',
            amount: 0
        });            
    };

    const { fields, append, remove } = useFieldArray({ control: form.control, name: "items" });    
    const watchedItems = useWatch({ control: form.control, name: "items" });

    // Extract the error message for the items array. An Error can happen when the sum of the amounts does not equal the transaction amount.
    const itemsError = form.formState.errors.items as
        | { message?: string; root?: { message?: string } }
        | undefined;
    const splitAmountError = itemsError?.message ?? itemsError?.root?.message;

    // Calculate live math safely:
    const allocatedAmount = watchedItems?.reduce((sum, item) => sum + (Number(item.amount) || 0), 0) || 0;
    const remainingAmount = transaction.amount - allocatedAmount;

    return (        
        <LedgerForm onSubmit={form.handleSubmit(onSubmit)}>
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
                        
                </div>
                
                <DataTable
                    columns={itemsColumns}
                    data={fields}
                    getRowId={(row) => (row as unknown as { id: string }).id}
                    meta={
                    { 
                        control: form.control, 
                        errors: form.formState.errors, 
                        removeRowById: (rowId: string) => {
                            if (fields.length > 1) {
                                const index = fields.findIndex((field) => field.id === rowId);
                                if (index !== -1) {
                                    remove(index);
                                }
                            }
                        },
                        accounts: accounts
                    }}
                />

                <Field orientation="horizontal">
                    <Button type="button" variant="secondary" disabled={isLoadingAccounts} onClick={() => onRowEnter()}>Add GL Item</Button>
                </Field>    

                <Field orientation="horizontal">

                    <LedgerTextDisplay
                        label="Target Amount"
                        value={formatCurrency(transaction.amount)} />

                    <LedgerTextDisplay
                        label="Allocated Amount"
                        value={formatCurrency(allocatedAmount)} />
                    
                    <LedgerTextDisplay
                        label="Remaining Amount"
                        value={formatCurrency(remainingAmount)} />

                </Field>    

                {splitAmountError && (
                    <p className="text-sm text-destructive">{splitAmountError}</p>
                )}                            

            </LedgerFormBody>
            <LedgerFormFooter>                
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>
                {transaction.status !== 'Reconciled' && <Button type="submit" variant="submit" disabled={isPending}>{isPending ? 'Saving...' : 'Save'}</Button>}
            </LedgerFormFooter>
        </LedgerForm>
    );
};