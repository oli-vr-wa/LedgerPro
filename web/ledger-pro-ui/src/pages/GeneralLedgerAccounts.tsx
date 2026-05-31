import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { AddGeneralLedgerAccountForm } from '../components/AddGeneralLedgerAccountForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';

export function GeneralLedgerAccounts() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    const { data: accounts, isLoading } = useQuery({
        queryKey: ['generalLedgerAccounts'],
        queryFn: () => generalLedgerAccountService.getAll().then(response => response.data)
    });

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">General Ledger Accounts</h1>
            <div className="bg-white shadow rounded-lg overflow-hidden">
                <table className="min-w-full"> 
                    <thead className="bg-gray-200">
                        <tr>
                            <th className="text-left px-6 py-3">Account Code</th>
                            <th className="text-left px-6 py-3">Account Name</th>
                            <th className="text-left px-6 py-3">Account Type</th>
                        </tr>
                    </thead>
                    <tbody>
                        {accounts?.map(account => (
                            <tr key={account.id} className="border-t">
                                <td className="px-6 py-4">{account.id}</td>
                                <td className="px-6 py-4">{account.name}</td>
                                <td className="px-6 py-4">{account.accountType}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                
                <LedgerDialog title="Add General Ledger Account" isOpen={isDialogOpen} setIsOpen={setIsDialogOpen}>
                    <AddGeneralLedgerAccountForm closeDialog={() => setIsDialogOpen(false)} />
                </LedgerDialog>
            </div>
        </div>
    );

}