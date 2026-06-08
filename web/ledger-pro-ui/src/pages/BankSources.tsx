import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { bankSourceService } from '../services/bankSourceService';
import { BankSourceForm } from '../components/BankSourceForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { DataTable } from '@/components/DataTable';
import { columns } from './bank-sources/columns';
import type { BankSource } from '@/types/bank-source.types';

export function BankSources() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [selectedBankSource, setSelectedBankSource] = useState<BankSource | undefined>(undefined); // State to hold the bank source being edited

    const { data: sources, isLoading} = useQuery({
        queryKey: ['bankSources'],
        queryFn: () => bankSourceService.getAll().then(response => response.data)
    })

    const handleRowClick = (bankSource: BankSource) => {
        setSelectedBankSource(bankSource);
        setIsDialogOpen(true);
    }

    const handleOpenDialog = (isOpen: boolean) => {
        setSelectedBankSource(undefined); // Clear selected bank source when opening the dialog
        setIsDialogOpen(isOpen); // Ensure dialog state is updated
    }

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">Bank Sources</h1>
            <DataTable columns={columns} data={sources ?? []} onRowClick={handleRowClick} />

            <LedgerDialog title="Add Bank Source" isOpen={isDialogOpen} setIsOpen={handleOpenDialog}>
                <BankSourceForm bankSource={selectedBankSource} closeDialog={() => setIsDialogOpen(false)} />
            </LedgerDialog>            
        </div>
    );
}