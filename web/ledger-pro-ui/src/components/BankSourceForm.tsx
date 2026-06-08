import { useForm } from 'react-hook-form';
import { bankSourceService } from '../services/bankSourceService';
import { BANK_TYPES, type BankSource, type BankSourcePayload } from '../types/bank-source.types';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { LedgerSelect } from './ui/form-fields/LedgerSelect';
import { bankSourceSchema, type BankSourceFormData } from '../schemas/bank-source.schemas';
import type z from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from './ui/form-fields/LedgerForm';
import { LedgerInput } from './ui/form-fields/LedgerInput';
import { Button } from './ui/button';
import { showApiToast } from '@/lib/toast-utils';

interface BankSourceFormProps {
    bankSource?: BankSource; // Optional prop to determine if we're in add or edit mode
    closeDialog: () => void;
}

/**
 * Form component for adding a new Bank Source.
 * Utilizes react-hook-form for form state management and Zod for validation.
 * On successful submission, it creates a new bank source via the service and invalidates the relevant query to refresh the data.
 * @param closeDialog - Function to close the dialog containing this form after successful submission or cancellation.
 * @returns JSX.Element - The rendered form component. 
 */
export function BankSourceForm({ bankSource, closeDialog }: BankSourceFormProps) {         
    const isEditMode = !!bankSource; // Determine mode based on presence of bankSource prop
    const buttonText = isEditMode ? 'Save Changes' : 'Add Bank Source';
    const buttonTextLoading = isEditMode ? 'Saving...' : 'Adding...';

    const queryClient = useQueryClient();
    
    const defaultValues: z.infer<typeof bankSourceSchema> = isEditMode ? {
        bankName: bankSource?.bankName ?? '',
        accountName: bankSource?.accountName ?? '',
        accountNumber: bankSource?.accountNumber ?? '',
        bankType: bankSource?.bankType ?? ''
    } : {
        bankName: '',
        accountName: '',
        accountNumber: '',
        bankType: '' 
    };

    const form = useForm<BankSourceFormData>({
        resolver: zodResolver(bankSourceSchema),
        defaultValues
    });

    const { mutate: saveBankSource, isPending } = useMutation({
        mutationFn: (data: BankSourcePayload) =>{
            if (isEditMode && bankSource) 
                return bankSourceService.update(bankSource.id, data);
            else
                return bankSourceService.create(data);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['bankSources'] });
            showApiToast(isEditMode ? 'Bank source updated successfully' : 'Bank source created successfully');
            closeDialog();
        },
        onError: (error) => {
            showApiToast(isEditMode ? 'Error updating bank source' : 'Error creating bank source', '', error);
            console.error('Error adding bank source:', error);
        }
    });

    const onSubmit = async (data: BankSourceFormData) => {
        const payload: BankSourcePayload = {
            ...data,
            bankType: data.bankType as BankSource['bankType']
        };

        saveBankSource(payload);
    };

    const onDelete = () => {
        if (!isEditMode || !bankSource) return;
        bankSourceService.delete(bankSource.id)
            .then(() => {
                queryClient.invalidateQueries({ queryKey: ['bankSources'] });
                showApiToast('Bank source deleted successfully');
                closeDialog();
            })
            .catch(error => {
                showApiToast('Error deleting bank source', '', error);
            });
    };

    return (
        <LedgerForm onSubmit={form.handleSubmit(onSubmit)}>
            <LedgerFormBody>
                <LedgerInput 
                    label="Bank Name"
                    placeholder="Enter the bank name"
                    {...form.register('bankName')}
                    error={form.formState.errors.bankName?.message}
                />

                <LedgerInput
                    label="Account Name"
                    placeholder="Enter the account name"
                    {...form.register('accountName')}
                    error={form.formState.errors.accountName?.message}
                />

                <LedgerInput
                    label="Account Number"
                    placeholder="Enter the account number"
                    {...form.register('accountNumber')}
                    error={form.formState.errors.accountNumber?.message}
                />

                <LedgerSelect
                    label="Bank Type"
                    name="bankType"
                    control={form.control as any} 
                    options={BANK_TYPES}
                    placeholder="Select Bank Type"
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