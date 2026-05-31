import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { generalLedgerAccountService } from '../services/generalLedgerAccountService';
import { AddGeneralLedgerAccountForm } from '../components/AddGeneralLedgerAccountForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { columns } from './general-ledger-accounts/columns';
import { DataTable } from '@/components/DataTable';

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
            <DataTable columns={columns} data={accounts ?? []} />
                
            <LedgerDialog title="Add General Ledger Account" isOpen={isDialogOpen} setIsOpen={setIsDialogOpen}>
                <AddGeneralLedgerAccountForm closeDialog={() => setIsDialogOpen(false)} />
            </LedgerDialog>            
        </div>
    );

}