import { useQuery } from '@tanstack/react-query';
import { bankTransactionMappingService } from '../services/bankTransactionMappingService';
import { columns } from './bank-transaction-mappings/columns';
import { DataTable } from '@/components/DataTable';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { AddBankTransactionMappingForm } from '@/components/AddBankTransactionMappingForm';
import { useState } from 'react';

export function BankTransactionMappings() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    const { data: mappings, isLoading } = useQuery({
        queryKey: ['bankTransactionMappings'],
        queryFn: () => bankTransactionMappingService.getAll().then(response => response.data)
    });

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">Bank Transaction Mappings</h1>
            <DataTable columns={columns} data={mappings ?? []} />

            <LedgerDialog title="Add Bank Transaction Mapping" isOpen={isDialogOpen} setIsOpen={setIsDialogOpen}>
                <AddBankTransactionMappingForm closeDialog={() => setIsDialogOpen(false)} />
            </LedgerDialog>
        </div>        
    );
}
