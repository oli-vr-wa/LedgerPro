import { useQuery } from '@tanstack/react-query';
import { bankTransactionMappingService } from '@/services/bankTransactionMappingService';
import { columns } from './columns';
import { DataTable } from '@/components/data-table/DataTable';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { BankTransactionMappingForm } from '@/components/forms/bank-transaction-mapping-form/BankTransactionMappingForm';
import { useState } from 'react';
import type { BankTransactionMapping } from '@/types/bank-transaction-mapping.types';

export function BankTransactionMappings() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [selectedMapping, setSelectedMapping] = useState<BankTransactionMapping | undefined>(undefined); // State to hold the mapping being edited

    const { data: mappings, isLoading } = useQuery({
        queryKey: ['bankTransactionMappings'],
        queryFn: () => bankTransactionMappingService.getAll().then(response => response.data)
    });

    const handleRowClick = (mapping: BankTransactionMapping) => {
        setSelectedMapping(mapping);
        setIsDialogOpen(true);
    };

    const handleOpenDialog = (isOpen: boolean) => {
        setSelectedMapping(undefined); // Clear selected mapping when opening the dialog
        setIsDialogOpen(isOpen); // Ensure dialog state is updated
    };

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">Bank Transaction Mappings</h1>
            <DataTable columns={columns} data={mappings ?? []} onRowClick={handleRowClick} />

            <LedgerDialog title={selectedMapping ? "Edit Bank Transaction Mapping" : "Add Bank Transaction Mapping"} isOpen={isDialogOpen} setIsOpen={handleOpenDialog}>
                <BankTransactionMappingForm closeDialog={() => handleOpenDialog(false)} mapping={selectedMapping} />
            </LedgerDialog>
        </div>        
    );
}
