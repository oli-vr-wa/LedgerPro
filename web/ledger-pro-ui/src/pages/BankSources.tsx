import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { bankSourceService } from '../services/bankSourceService';
import { AddBankSourceForm } from '../components/AddBankSourceForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { DataTable } from '@/components/DataTable';
import { columns } from './bank-sources/columns';

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
            <DataTable columns={columns} data={sources ?? []} />

            <LedgerDialog title="Add Bank Source" isOpen={isDialogOpen} setIsOpen={setIsDialogOpen}>
                <AddBankSourceForm closeDialog={() => setIsDialogOpen(false)} />
            </LedgerDialog>
            
        </div>
    );
}