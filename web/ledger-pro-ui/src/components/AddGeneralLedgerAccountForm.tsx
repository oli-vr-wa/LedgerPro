import { useForm } from 'react-hook-form';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { GENERAL_LEDGER_ACCOUNT_TYPES, type GeneralLedgerAccount } from '../types/generalLedgerAccount';
import { useQueryClient } from '@tanstack/react-query';
import { Input } from './ui/Input';
import { TextArea } from './ui/TextArea';
import { DropdownSelect } from './ui/DropdownSelect';

type FormData = GeneralLedgerAccount;

export function AddGeneralLedgerAccountForm({ onClose }: { onClose: () => void }) {
    const { register, handleSubmit, formState: { errors } } = useForm<FormData>();
    const queryClient = useQueryClient();

    const onSubmit = async (data: FormData) => {
        try {
            await generalLedgerAccountService.create(data);
            queryClient.invalidateQueries({ queryKey: ['generalLedgerAccounts'] });
            onClose();
        } catch (error) {
            console.error('Error adding general ledger account:', error);
        }
    };

    const onError = (errors: any) => {
        console.log("Form Errors:", errors); // Check this in your browser console (F12)
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50">
            <div className="bg-white p-6 rounded-lg shadow-2xl w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Add General Ledger Account</h2>

                {/* Simple form inputs for now */}
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
                        <button type="button" className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300 transition hover:cursor-pointer" onClick={onClose}>Cancel</button>
                        <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition hover:cursor-pointer">Add</button>
                    </div>
                </form>
            </div>
        </div>
    );
}