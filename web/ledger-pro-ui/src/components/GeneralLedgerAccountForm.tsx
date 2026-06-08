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
import { showApiToast } from '@/lib/toast-utils';

interface GlAccountFormProps {
    account?: GeneralLedgerAccount; // Optional account prop to determine if we're in add or edit mode
    closeDialog: () => void;    
}

/**
 * Form component for adding or editing a General Ledger Account. 
 * Utilizes react-hook-form for form state management and Zod for validation. 
 * On successful submission, it creates or updates a GL account via the service and invalidates the relevant query to refresh the data.
 * @param closeDialog - Function to close the dialog containing this form after successful submission or cancellation.
 * @returns JSX.Element - The rendered form component. 
 */
export function GeneralLedgerAccountForm({ account, closeDialog }: GlAccountFormProps) { 
    const isEditMode = !!account; // Determine mode based on presence of account prop
    const buttonText = isEditMode ? 'Save Changes' : 'Add Account';
    const buttonTextLoading = isEditMode ? 'Saving...' : 'Adding...';

    const queryClient = useQueryClient();

    // Define default form values based on the Zod schema
    const defaultValues: z.infer<typeof glAccountSchema> = isEditMode ? {
        id: account?.id.toString() ?? '',
        name: account?.name ?? '',
        description: account?.description ?? '',
        accountType: account?.accountType ?? ''
    } : {
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
    const { mutate: createOrUpdateGLAccount, isPending } = useMutation({
        mutationFn: (data: GeneralLedgerAccount) => {
            if (isEditMode) {
                return generalLedgerAccountService.update(data.id, data);
            } else {
                return generalLedgerAccountService.create(data);
            }
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['generalLedgerAccounts'] });
            showApiToast(isEditMode ? 'GL account updated successfully' : 'GL account created successfully');
            closeDialog();
        },
        onError: (error) => {
            showApiToast(isEditMode ? 'Error updating GL account' : 'Error creating GL account', '', error);
            console.error('Error adding general ledger account:', error);
        }
    });

    // Form submission handler that prepares the payload and calls the mutation function
    const onSubmit = async (data: glAccountFormData) => {
        const payload: GeneralLedgerAccount = {
            ...data,
            id: parseInt(data.id), // Convert id to number before sending to API
            accountType: data.accountType as GeneralLedgerAccount['accountType']
        };
        createOrUpdateGLAccount(payload);
    };

    // Handler for deleting a GL account, only available in edit mode. It calls the delete service and invalidates the query on success.
    const onDelete = () => {
        if (!isEditMode || !account) return; // Should never happen, but just in case
        generalLedgerAccountService.delete(account.id)
            .then(() => {
                queryClient.invalidateQueries({ queryKey: ['generalLedgerAccounts'] });
                showApiToast('GL account deleted successfully');
                closeDialog();
            })
            .catch(error => {
                showApiToast('Error deleting GL account', '', error);
            });
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
                {isEditMode && <Button type="button" variant="destructive" onClick={onDelete}>Delete</Button>}
                <Button type="button" variant="cancel" onClick={closeDialog}>Cancel</Button>                    
                <Button type="submit" variant="submit" disabled={isPending}>{isPending ? buttonTextLoading : buttonText}</Button>
            </LedgerFormFooter>       
        </LedgerForm>      
    );
}

