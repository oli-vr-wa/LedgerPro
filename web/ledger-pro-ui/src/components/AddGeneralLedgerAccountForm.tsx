import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQueryClient, useMutation } from '@tanstack/react-query';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { LedgerForm, LedgerFormBody, LedgerFormFooter } from './ui/form-fields/LedgerForm';
import { LedgerInput } from './ui/form-fields/LedgerInput';
import { LedgerTextarea } from './ui/form-fields/LedgerTextarea';
import { LedgerSelect } from './ui/form-fields/LedgerSelect';
import { Button } from './ui/button';
import { GENERAL_LEDGER_ACCOUNT_TYPES, type GeneralLedgerAccount } from '../types/general-ledger-account.types';
import { glAccountSchema, type glAccountFormData } from '../schemas/general-ledger-account.schemas';
import { z } from 'zod';

interface AddGlAccountFormProps {
    closeDialog: () => void;    
}

/**
 * Form component for adding a new General Ledger Account. 
 * Utilizes react-hook-form for form state management and Zod for validation. 
 * On successful submission, it creates a new GL account via the service and invalidates the relevant query to refresh the data.
 * @param closeDialog - Function to close the dialog containing this form after successful submission or cancellation.
 * @returns JSX.Element - The rendered form component. 
 */
export function AddGeneralLedgerAccountForm({ closeDialog }: AddGlAccountFormProps) { 
    const queryClient = useQueryClient();

    // Define default form values based on the Zod schema
    const defaultValues: z.infer<typeof glAccountSchema> = {
        id: '',
        name: '',
        description: '',
        accountType: ''
    };

    // Initialize react-hook-form with Zod validation
    const form = useForm<glAccountFormData>({
        resolver: zodResolver(glAccountSchema),
        defaultValues
    });

    // TanStack Mutation for creating a new GL account
    const { mutate: createGLAccount, isPending } = useMutation({
        mutationFn: (data: GeneralLedgerAccount) => generalLedgerAccountService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['generalLedgerAccounts'] });
            closeDialog();
        },
        onError: (error) => {
            console.error('Error adding general ledger account:', error);
        }
    });

    const onSubmit = async (data: glAccountFormData) => {
        const payload: GeneralLedgerAccount = {
            ...data,
            id: parseInt(data.id), // Convert id to number before sending to API
            accountType: data.accountType as GeneralLedgerAccount['accountType']
        };
        createGLAccount(payload);
    };

    return (
        <LedgerForm onSubmit={form.handleSubmit(onSubmit)}>
            <LedgerFormBody> 

                <LedgerInput 
                    label="Account Code" 
                    placeholder="Enter the GL account code (e.g., 1000)" 
                    {...form.register('id')} 
                    error={form.formState.errors.id?.message} />

                <LedgerInput 
                    label="Account Name" 
                    placeholder="Enter the GL account name (e.g., Cash)" 
                    {...form.register('name')} 
                    error={form.formState.errors.name?.message} />

                <LedgerTextarea
                    label="Description"
                    placeholder="Add a description of the GL account (optional)"
                    {...form.register('description')}
                    error={form.formState.errors.description?.message}
                />

                <LedgerSelect 
                    label="Account Type" 
                    name="accountType"
                    control={form.control as any}                    
                    options={GENERAL_LEDGER_ACCOUNT_TYPES} 
                    placeholder="Select Account Type" 
                />
                  
            </LedgerFormBody>
            <LedgerFormFooter>
                <Button type="button" onClick={closeDialog}>Cancel</Button>                    
                <Button type="submit" disabled={isPending}>{isPending ? 'Adding...' : 'Add'}</Button>
            </LedgerFormFooter>       
        </LedgerForm>      
    );
}

