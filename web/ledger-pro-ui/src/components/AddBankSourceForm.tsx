import { useForm } from 'react-hook-form';
import { bankSourceService } from '../services/bankSourceService';
import type { BankSource } from '../types/bankSource';
import { useQueryClient } from '@tanstack/react-query';

type FormData = Omit<BankSource, 'id'>;

export function AddBankSourceForm({ onClose }: { onClose: () => void }) { 
    const { register, handleSubmit, formState: { errors } } = useForm<FormData>();
    const queryClient = useQueryClient();
    
    const onSubmit = async (data: FormData) => {
        console.log("called"); // Check this in your browser console (F12)
        try {
            await bankSourceService.create(data);
            queryClient.invalidateQueries({ queryKey: ['bankSources'] });
            alert('Bank source added successfully!');
            onClose();
        } catch (error) {
            console.error('Error adding bank source:', error);
            alert('Failed to add bank source. Please try again.');
        }
    };

    const onError = (errors: any) => {
        console.log("Form Errors:", errors); // Check this in your browser console (F12)
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50">
            <div className="bg-white p-6 rounded-lg shadow-2xl w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Add Bank Source</h2>

                {/* Simple form inputs for now */}
                <form onSubmit={handleSubmit(onSubmit, onError)}>

                    <div className="space-y-4">                        
                        <div>
                            <input {...register('bankName', { required: "Bank Name is required" })} placeholder="Bank Name" className="w-full p-2 border border-gray-400 rounded" />
                            {errors.bankName && <span className="text-red-500 text-sm">{errors.bankName.message}</span>}
                        </div>

                        <div>
                            <input {...register('accountName', { required: "Account Name is required" })} placeholder="Account Name" className="w-full p-2 border border-gray-400 rounded" />
                            {errors.accountName && <span className="text-red-500 text-sm">{errors.accountName.message}</span>}
                        </div>

                        <div>
                            <input {...register('accountNumber', { required: "Account Number is required" })} placeholder="Account Number" className="w-full p-2 border border-gray-400 rounded" />
                            {errors.accountNumber && <span className="text-red-500 text-sm">{errors.accountNumber.message}</span>}
                        </div>

                        <div>
                            <select {...register('bankType', { required: "Bank Type is required" })} className="w-full p-2 border border-gray-400 rounded">
                                <option value="">Select Bank Type</option>
                                <option value="Generic">Generic</option>
                                <option value="NAB">NAB</option>
                                <option value="ANZ">ANZ</option>
                                <option value="CBA">CBA</option>
                            </select>
                            {errors.bankType && <span className="text-red-500 text-sm">{errors.bankType.message}</span>}
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