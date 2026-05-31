import { useForm } from 'react-hook-form';
import { bankSourceService } from '../services/bankSourceService';
import { BANK_TYPES, type BankSource, type CreateBankSourcePayload } from '../types/bank-source.types';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { LedgerSelect } from './ui/form-fields/LedgerSelect';
import { bankSourceSchema, type BankSourceFormData } from '../schemas/bank-source.schemas';
import type z from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from './ui/form-fields/LedgerForm';
import { LedgerInput } from './ui/form-fields/LedgerInput';
import { Button } from './ui/button';

interface AddBankSourceFormProps {
    closeDialog: () => void;
}

/**
 * Form component for adding a new Bank Source.
 * Utilizes react-hook-form for form state management and Zod for validation.
 * On successful submission, it creates a new bank source via the service and invalidates the relevant query to refresh the data.
 * @param closeDialog - Function to close the dialog containing this form after successful submission or cancellation.
 * @returns JSX.Element - The rendered form component. 
 */
export function AddBankSourceForm({ closeDialog }: AddBankSourceFormProps) {         
    const queryClient = useQueryClient();
    
    const defaultValues: z.infer<typeof bankSourceSchema> = {
        bankName: '',
        accountName: '',
        accountNumber: '',
        bankType: '' 
    };

    const form = useForm<BankSourceFormData>({
        resolver: zodResolver(bankSourceSchema),
        defaultValues
    });

    const { mutate: createBankSource, isPending } = useMutation({
        mutationFn: (data: CreateBankSourcePayload) => bankSourceService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['bankSources'] });
            closeDialog();
        },
        onError: (error) => {
            console.error('Error adding bank source:', error);
        }
    });

    const onSubmit = async (data: BankSourceFormData) => {
        const payload: CreateBankSourcePayload = {
            ...data,
            bankType: data.bankType as BankSource['bankType']
        };

        createBankSource(payload);
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
                <Button type="button" onClick={closeDialog}>Cancel</Button>
                <Button type="submit" disabled={isPending}>{isPending ? 'Adding...' : 'Add'}</Button>
            </LedgerFormFooter>
        </LedgerForm>
    );
}