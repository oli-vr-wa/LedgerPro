import { useForm } from 'react-hook-form';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { GENERAL_LEDGER_ACCOUNT_TYPES, type GeneralLedgerAccount } from '../types/generalLedgerAccount';
import { useQueryClient } from '@tanstack/react-query';
import { Input } from './ui/Input';
import { TextArea } from './ui/TextArea';
import { DropdownSelect } from './ui/DropdownSelect';
import { useState } from 'react';
import { Button } from './ui/button';

type FormData = GeneralLedgerAccount;

interface AddGlAccountFormProps {
    closeDialog: () => void;    
}

export function AddGeneralLedgerAccountForm({ closeDialog }: AddGlAccountFormProps) {
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { register, handleSubmit, formState: { errors } } = useForm<FormData>();
    const queryClient = useQueryClient();

    const onSubmit = async (data: FormData) => {
        setIsSubmitting(true);
        try {
            await generalLedgerAccountService.create(data);
            queryClient.invalidateQueries({ queryKey: ['generalLedgerAccounts'] });
            closeDialog(); // Close the dialog after successful submission            
        } catch (error) {
            console.error('Error adding general ledger account:', error);
        } finally {
            setIsSubmitting(false);
        }
    };

    const onError = (errors: any) => {
        console.log("Form Errors:", errors); // Check this in your browser console (F12)
    };

    return (
        <form onSubmit={handleSubmit(onSubmit, onError)}>

            <div className="space-y-4">
                <Input {...register('id', { required: "Account Code is required" })} placeholder="Account Code" error={errors.id} />
                
                <Input {...register('name', { required: "Account Name is required" })} placeholder="Account Name" error={errors.name} />
                
                <TextArea {...register('description')} placeholder="Description (optional)" error={errors.description} />
                
                <DropdownSelect {...register('accountType', { required: "Account Type is required" })} 
                                options={GENERAL_LEDGER_ACCOUNT_TYPES} 
                                error={errors.accountType} />
                                            
            </div>

            <div className="mt-6 flex justify-end space-x-3">                    
                <Button type="button" onClick={closeDialog} className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300 transition hover:cursor-pointer">Cancel</Button>                    
                <Button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition hover:cursor-pointer" disabled={isSubmitting}>
                    {isSubmitting ? 'Adding...' : 'Add'}
                </Button>
            </div>
        </form>      
    );
}

