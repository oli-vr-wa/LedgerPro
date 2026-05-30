import { useForm } from 'react-hook-form';
import { bankSourceService } from '../services/bankSourceService';
import { BANK_TYPES, type BankSource } from '../types/bankSource';
import { useQueryClient } from '@tanstack/react-query';
import { Input } from './ui/Input';
import { DropdownSelect } from './ui/DropdownSelect';
import { useState } from 'react';

type FormData = Omit<BankSource, 'id'>;

interface AddBankSourceFormProps {
    closeDialog: () => void;
}

export function AddBankSourceForm({ closeDialog }: AddBankSourceFormProps) { 
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { register, handleSubmit, formState: { errors } } = useForm<FormData>();
    const queryClient = useQueryClient();
    
    const onSubmit = async (data: FormData) => {
        setIsSubmitting(true);
        try {
            await bankSourceService.create(data);
            queryClient.invalidateQueries({ queryKey: ['bankSources'] });
        } catch (error) {
            console.error('Error adding bank source:', error);
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

                    <Input {...register('bankName', { required: "Bank Name is required" })} placeholder="Bank Name" error={errors.bankName} />
                    
                    <Input {...register('accountName', { required: "Account Name is required" })} placeholder="Account Name" error={errors.accountName} />

                    <Input {...register('accountNumber', { required: "Account Number is required" })} placeholder="Account Number" error={errors.accountNumber} />

                    <DropdownSelect {...register('bankType', { required: "Bank Type is required" })} options={BANK_TYPES} error={errors.bankType} />

                </div>

                <div className="mt-6 flex justify-end space-x-3">
                    <button type="button" className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 transition hover:cursor-pointer" onClick={closeDialog}>Cancel</button>
                    <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition hover:cursor-pointer" disabled={isSubmitting}>
                        {isSubmitting ? 'Adding...' : 'Add'}
                    </button>
                </div>
            </form>
    );
}