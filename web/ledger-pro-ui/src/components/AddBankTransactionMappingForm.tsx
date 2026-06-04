import { bankTransactionMappingSchema, type BankTransactionMappingFormData } from "@/schemas/bank-transaction-mapping.schemas";
import { bankTransactionMappingService } from "@/services/bankTransactionMappingService";
import { generalLedgerAccountService } from "@/services/generalLedgerAccountService";
import { BANK_TRANSACTION_MAPPING_STRATEGIES, type BankTransactionMapping } from "@/types/bank-transaction-mapping.types";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod.js";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import z from "zod";
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from "./ui/form-fields/LedgerForm";
import { LedgerInput } from "./ui/form-fields/LedgerInput";
import { LedgerSelect } from "./ui/form-fields/LedgerSelect";
import { Button } from "./ui/button";
import type { GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import { useEffect, useState } from "react";


interface AddBankTransactionMappingFormProps {
    closeDialog: () => void;
}

export function AddBankTransactionMappingForm({ closeDialog }: AddBankTransactionMappingFormProps) {
    const [accounts, setAccounts] = useState<GeneralLedgerAccount[]>([]);
    const [isLoadingAccounts, setIsLoadingAccounts] = useState(true);

    const queryClient = useQueryClient();

    useEffect(() => {
        // Use the general ledger account service to fetch accounts for the select dropdown
        generalLedgerAccountService.getAll()
            .then(response => {
                setAccounts(response.data);
                setIsLoadingAccounts(false);
            })
            .catch(error => {
                console.error('Error fetching general ledger accounts:', error);
                setIsLoadingAccounts(false);
            });
    }, []); // Fetch GL accounts on mount

    const defaultValues: z.infer<typeof bankTransactionMappingSchema> = {
        searchTerm: '',
        matchStrategy: '',
        targetGeneralLedgerAccountId: '',
        descriptionTemplate: '',
        referenceTemplate: '',
        priority: 1
    };

    const form = useForm<BankTransactionMappingFormData>({
        resolver: zodResolver(bankTransactionMappingSchema),
        defaultValues
    });

    const {mutate: createMapping, isPending} = useMutation({
        mutationFn: (data: Omit<BankTransactionMapping, 'id'>) => bankTransactionMappingService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['bankTransactionMappings'] });
            closeDialog();
        },
        onError: (error) => {
            console.error('Error creating bank transaction mapping:', error);
        }
    });

    const onSubmit = (data: BankTransactionMappingFormData) => {
        const payload: Omit<BankTransactionMapping, 'id'> = {
            ...data,           
            matchStrategy: data.matchStrategy as BankTransactionMapping['matchStrategy'], // Ensure correct type 
            targetGeneralLedgerAccountId: parseInt(data.targetGeneralLedgerAccountId), // Convert to number before sending to API
            priority: Number(data.priority) // Ensure priority is a number
        };
        createMapping(payload);
    };

    if (isLoadingAccounts) {
        return <div>Loading...</div>;
    }

    return (
        <LedgerForm onSubmit={form.handleSubmit(onSubmit)}>
            <LedgerFormBody>

                <LedgerInput
                    label="Search Term"
                    placeholder="Term to match in bank transactions"
                    {...form.register('searchTerm')}
                    error={form.formState.errors.searchTerm?.message}
                />  

                <LedgerSelect
                    label="Match Strategy"
                    name="matchStrategy"
                    control={form.control as any} 
                    options={BANK_TRANSACTION_MAPPING_STRATEGIES}
                    placeholder="Select a match strategy"
                />

                <LedgerSelect
                    label="General Ledger Account"
                    name="targetGeneralLedgerAccountId"
                    control={form.control as any}
                    options={accounts.map(account => ({ value: account.id.toString(), label: `${account.id} - ${account.name}` }))}
                    placeholder="Select a GL account"
                />

                <LedgerInput
                    label="Description Template"
                    placeholder="Template for the description"
                    {...form.register('descriptionTemplate')}
                    error={form.formState.errors.descriptionTemplate?.message}
                />

                <LedgerInput
                    label="Reference Template"  
                    placeholder="Template for the reference"
                    {...form.register('referenceTemplate')}
                    error={form.formState.errors.referenceTemplate?.message}
                />

                <LedgerInput
                    label="Priority"
                    type="number"
                    placeholder="Priority for matching (lower number = higher priority)"
                    {...form.register('priority')}
                    error={form.formState.errors.priority?.message}
                />

            </LedgerFormBody>
            <LedgerFormFooter>
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>                    
                <Button type="submit" variant="submit" disabled={isPending}>{isPending ? 'Adding...' : 'Add'}</Button>
            </LedgerFormFooter>
        </LedgerForm>
    );
}
