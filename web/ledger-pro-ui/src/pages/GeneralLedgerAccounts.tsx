import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { AddGeneralLedgerAccountForm } from '../components/AddGeneralLedgerAccountForm';

export function GeneralLedgerAccounts() {
    const [isModalOpen, setIsModalOpen] = useState(false);

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

                <button 
                    onClick={() => setIsModalOpen(true)}
                    className="fixed bottom-8 right-8 bg-blue-600 text-white p-4 rounded-full shadow-lg hover:bg-blue-700 transition hover:cursor-pointer"
                >
                    <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>                        
                </button>

                {isModalOpen && <AddGeneralLedgerAccountForm onClose={() => setIsModalOpen(false)} />}
            </div>
        </div>
    );

}