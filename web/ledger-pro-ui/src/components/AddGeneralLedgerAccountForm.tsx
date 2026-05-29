import { useForm } from 'react-hook-form';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import type { GeneralLedgerAccount } from '../types/generalLedgerAccount';
import { useQueryClient } from '@tanstack/react-query';

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
                        <div>
                            <input {...register('id', { required: "Account Code is required" })} placeholder="Account Code" className="w-full p-2 border border-gray-400 rounded" />
                            {errors.id && <span className="text-red-500 text-sm">{errors.id.message}</span>}
                        </div>

                        <div>
                            <input {...register('name', { required: "Account Name is required" })} placeholder="Account Name" className="w-full p-2 border border-gray-400 rounded" />
                            {errors.name && <span className="text-red-500 text-sm">{errors.name.message}</span>}
                        </div>

                        <div>
                            <textarea {...register('description')} placeholder="Description (optional)" className="w-full p-2 border border-gray-400 rounded" />                            
                        </div>

                        <div>
                            <select {...register('accountType', { required: "Account Type is required" })} className="w-full p-2 border border-gray-400 rounded">
                                <option value="">Select Account Type</option>
                                <option value="Asset">Asset</option>
                                <option value="Liability">Liability</option>
                                <option value="Equity">Equity</option>
                                <option value="Revenue">Revenue</option>
                                <option value="Expense">Expense</option>
                            </select>
                            {errors.accountType && <span className="text-red-500 text-sm">{errors.accountType.message}</span>}
                        </div>
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