import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { bankSourceService } from '../services/bankSourceService';
import { AddBankSourceForm } from '../components/AddBankSourceForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';

export function BankSources() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    const { data: sources, isLoading} = useQuery({
        queryKey: ['bankSources'],
        queryFn: () => bankSourceService.getAll().then(response => response.data)
    })

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">Bank Sources</h1>
            <div className="bg-white shadow rounded-lg overflow-hidden">
                <table className="min-w-full">
                    <thead className="bg-gray-200">
                        <tr>
                            <th className="text-left px-6 py-3">Bank Name</th>
                            <th className="text-left px-6 py-3">Account Name</th>
                            <th className="text-left px-6 py-3">Account Number</th>
                            <th className="text-left px-6 py-3">Bank Type</th>
                            <th className="text-left px-6 py-3">GL Account ID</th>
                        </tr>
                    </thead>
                    <tbody>
                        {(sources ?? []).map(source => (
                            <tr key={source.id} className="border-t">
                                <td className="px-6 py-4">{source.bankName}</td>
                                <td className="px-6 py-4">{source.accountName}</td>
                                <td className="px-6 py-4">{source.accountNumber}</td>
                                <td className="px-6 py-4">{source.bankType}</td>
                                <td className="px-6 py-4">{source.generalLedgerAccountId}</td>
                            </tr>
                        ))}                            
                    </tbody>
                </table>

                <LedgerDialog title="Add Bank Source" isOpen={isDialogOpen} setIsOpen={setIsDialogOpen}>
                    <AddBankSourceForm closeDialog={() => setIsDialogOpen(false)} />
                </LedgerDialog>
            </div>
        </div>
    );
}