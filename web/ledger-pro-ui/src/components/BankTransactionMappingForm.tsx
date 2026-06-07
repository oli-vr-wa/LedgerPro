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

interface BankTransactionMappingFormProps {
    mapping?: BankTransactionMapping; // Optional mapping prop to determine if we're in add or edit mode
    closeDialog: () => void;
}

/**
 * A form component for adding or editing bank transaction mappings. 
 * It uses react-hook-form for form state management and validation, and react-query for handling API mutations. 
 * The form fields are dynamically populated based on whether we're in add or edit mode, and it includes error handling and loading states for better user experience.
 * @param BankTransactionMappingFormProps - Props for the form, including an optional mapping for edit mode and a function to close the dialog.
 * @returns A React component that renders a form for creating or updating bank transaction mappings, with appropriate fields, validation, and API integration.
 */
export function BankTransactionMappingForm({ mapping, closeDialog }: BankTransactionMappingFormProps) {
    const isEditMode = !!mapping; // Determine mode based on presence of mapping prop
    const [accounts, setAccounts] = useState<GeneralLedgerAccount[]>([]);
    const [isLoadingAccounts, setIsLoadingAccounts] = useState(true);
    const buttonText = isEditMode ? 'Save Changes' : 'Add Mapping';
    const buttonTextLoading = isEditMode ? 'Saving...' : 'Adding...';

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

    // Set default form values based on whether we're in edit mode or add mode
    const defaultValues: z.infer<typeof bankTransactionMappingSchema> = isEditMode ? {
        searchTerm: mapping!.searchTerm,
        matchStrategy: mapping!.matchStrategy,
        targetGeneralLedgerAccountId: mapping!.targetGeneralLedgerAccountId.toString(),
        descriptionTemplate: mapping!.descriptionTemplate,
        referenceTemplate: mapping!.referenceTemplate,
        priority: mapping!.priority ?? 1 // Default to 1 if priority is undefined/null
    } : {
        searchTerm: '',
        matchStrategy: '',
        targetGeneralLedgerAccountId: '',
        descriptionTemplate: '',
        referenceTemplate: '',
        priority: 1
    };

    // Initialize the form with react-hook-form, using zod for validation and setting default values based on mode
    const form = useForm<BankTransactionMappingFormData>({
        resolver: zodResolver(bankTransactionMappingSchema),
        defaultValues
    });

    // Mutation for saving the mapping, which handles both create and update based on the mode. 
    // It also invalidates the relevant query to refresh the list after a successful mutation.
    const {mutate: saveMapping, isPending} = useMutation({
        mutationFn: (data: Omit<BankTransactionMapping, 'id'>) => {
            if (isEditMode) 
                return bankTransactionMappingService.update(mapping!.id, data);
            else 
                return bankTransactionMappingService.create(data);                        
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['bankTransactionMappings'] });
            closeDialog();
        },
        onError: (error) => {
            console.error('Error saving bank transaction mapping:', error);
        }
    });

    // Handle form submission by preparing the payload and calling the save mutation
    const onSubmit = (data: BankTransactionMappingFormData) => {
        const payload: Omit<BankTransactionMapping, 'id'> = {
            ...data,
            matchStrategy: data.matchStrategy as BankTransactionMapping['matchStrategy'], // Ensure correct type
            targetGeneralLedgerAccountId: parseInt(data.targetGeneralLedgerAccountId), // Convert to number before sending to API
            priority: Number(data.priority) // Ensure priority is a number
        };
        saveMapping(payload);
    }

    // Handle deletion of a bank transaction mapping, which is only available in edit mode. 
    // It calls the delete API and invalidates the query on success.
    const onDelete = () => {
        if (!isEditMode) return; // Should never happen since delete button is only shown in edit mode
        bankTransactionMappingService.delete(mapping!.id)
            .then(() => {
                queryClient.invalidateQueries({ queryKey: ['bankTransactionMappings'] });
                closeDialog();
            })
            .catch(error => {
                console.error('Error deleting bank transaction mapping:', error);
            });
    }

    if (isLoadingAccounts) return <div>Loading...</div>;

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
                {isEditMode && <Button type="button" variant="destructive" onClick={onDelete}>Delete</Button>}
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>                    
                <Button type="submit" variant="submit" disabled={isPending}>{isPending ? buttonTextLoading : buttonText}</Button>
            </LedgerFormFooter>
        </LedgerForm>
    );
}