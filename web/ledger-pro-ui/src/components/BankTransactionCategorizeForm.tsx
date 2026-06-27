import type { BankTransaction } from "@/types/bank-transaction.types";
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from "./ui/form-fields/LedgerForm";
import { createCategorizeSplitSchema, type BankTransactionCategorizeFormData } from "@/schemas/transaction-categorize-item.schemas";
import { useFieldArray, useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod.js";
import type z from "zod";
import { LedgerTextDisplay } from "./ui/form-fields/LedgerTextDisplay";
import type { GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import { useEffect, useMemo, useState } from "react";
import { generalLedgerAccountService } from "@/services/generalLedgerAccountService";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { LedgerSelect } from "./ui/form-fields/LedgerSelect";
import { Button } from "./ui/button";
import { showApiToast } from "@/lib/toast-utils";
import { bankTransactionsService } from "@/services/bankTransactionsService";
import type { BankTransactionCategorize } from "@/types/bank-transaction-categorize.types";
import { formatCurrency, formatDate } from "./data-table/utils/data-table-cell-format.utils";
import type { ColumnDef } from "@tanstack/react-table";
import { DataTable } from "./data-table/DataTable";
import { EditableCell } from "./data-table/EditableCell";
import { Trash2 } from "lucide-react";
import { Field } from "./ui/field";

interface BankTransactionCategorizeFormProps {
    transaction: BankTransaction;
    closeDialog: () => void;
}

const getItemsColumns = (accounts: GeneralLedgerAccount[]): ColumnDef<BankTransactionCategorizeFormData['items'][number]>[] => [
        {
            accessorKey: 'description',
            header: 'Description',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;                
                return (
                    <EditableCell
                        name={`items.${row.index}.description`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.description?.message as string}
                    />
                )
            }
        },
        {
            accessorKey: 'reference',
            header: 'Reference',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;
                return (
                    <EditableCell
                        name={`items.${row.index}.reference`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.reference?.message as string}
                    />
                )
            }
        },
        {
            accessorKey: 'generalLedgerAccountId',
            header: 'GL Classification',
            cell: ({ row, table }) => {
                const meta = table.options.meta as any;
                return (
                    <EditableCell
                        name={`items.${row.index}.generalLedgerAccountId`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.generalLedgerAccountId?.message as string}
                        type="select"
                        options={meta.accounts?.map((account: GeneralLedgerAccount) => ({ value: account.id.toString(), label: `${account.id} - ${account.name}` })) ?? []}
                    />
                )
            }
        },
        {
            accessorKey: 'amount',
            header: 'Amount',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;
                return (
                    <EditableCell
                        name={`items.${row.index}.amount`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.amount?.message as string}
                        type="number"
                    />
                )
            }
        },
        {
            id: 'actions',
            header: 'Actions',
            cell: ({ row, table }) => (
                <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => table.options.meta?.removeRow?.(row.index)}>
                        <Trash2 className="h-4 w-4 text-muted-foreground hover:text-destructive" />
                </Button>
            )
        }
    ];

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
        return getItemsColumns(accounts);
    }, [accounts]);

    const form = useForm<BankTransactionCategorizeFormData>({ 
        resolver: zodResolver(createCategorizeSplitSchema(transaction.amount)),
        defaultValues: defaultValues as BankTransactionCategorizeFormData
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

    // if (isLoadingAccounts) return <div>Loading...</div>;    

    const onRowEnter = (index: number) => {
        append({
            generalLedgerAccountId: '',
            description: '',
            reference: '',
            amount: 0
        });            
    };

    const { fields, append, remove } = useFieldArray({ control: form.control, name: "items" });

    
    const watchedItems = useWatch({ control: form.control, name: "items" });

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
                

                <DataTable columns={itemsColumns} data={fields} meta={
                    { 
                        control: form.control, 
                        errors: form.formState.errors, 
                        removeRow: (index: number) => {
                            if (fields.length > 1) {
                                remove(index);
                            }
                        },
                        onRowEnter: onRowEnter
                    }} />

                    <Field orientation="horizontal">
                        <Button type="button" variant="secondary" onClick={() => onRowEnter(form.getValues('items').length - 1)}>Add GL Item</Button>
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

            </LedgerFormBody>
            <LedgerFormFooter>
                {transaction.status === 'Reconciled' && <Button variant="destructive">Unreconcile</Button>}
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>
                {transaction.status !== 'Reconciled' && <Button type="submit" variant="submit" disabled={isPending}>{isPending ? 'Saving...' : 'Save'}</Button>}
            </LedgerFormFooter>
        </LedgerForm>
    );
};