import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { generalLedgerAccountService } from '../../services/generalLedgerAccountService';
import { GeneralLedgerAccountForm } from '../../components/GeneralLedgerAccountForm';
import { LedgerDialog } from '@/components/ui/LedgerDialog';
import { columns } from './columns';
import { DataTable } from '@/components/DataTable';
import type { GeneralLedgerAccount } from '@/types/general-ledger-account.types';

/**
 * GeneralLedgerAccounts component is responsible for displaying a list of general ledger accounts in a data table. 
 * It also provides functionality to add a new account or edit an existing one through a dialog form. 
 * The component uses react-query to fetch the accounts from the API and manage loading states, and it utilizes a reusable DataTable component 
 * for displaying the accounts in a tabular format. 
 * @returns JSX.Element - The rendered GeneralLedgerAccounts component.
 */
export function GeneralLedgerAccounts() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [selectedAccount, setSelectedAccount] = useState<GeneralLedgerAccount | undefined>(undefined); // State to hold the account being edited

    // Fetch general ledger accounts using react-query and store them in the 'accounts' variable. Also manage loading state with 'isLoading'.
    const { data: accounts, isLoading } = useQuery({
        queryKey: ['generalLedgerAccounts'],
        queryFn: () => generalLedgerAccountService.getAll().then(response => response.data)
    });

    // Handler for when a row in the data table is clicked. It sets the selected account and opens the dialog for editing.
    const handleRowClick = (account: GeneralLedgerAccount) => {
        setSelectedAccount(account);
        setIsDialogOpen(true);
    };

    // Handler for opening the dialog. It clears the selected account when opening the dialog for adding a new account, and ensures the dialog state is updated.
    const handleOpenDialog = (isOpen: boolean) => {
        setSelectedAccount(undefined); // Clear selected account when opening the dialog
        setIsDialogOpen(isOpen); // Ensure dialog state is updated
    };

    if (isLoading) return <div>Loading...</div>;

    return (
        <div>
            <h1 className="text-2xl font-bold mb-6">General Ledger Accounts</h1>
            <DataTable columns={columns} data={accounts ?? []} onRowClick={handleRowClick} />
                
            <LedgerDialog title="Add General Ledger Account" isOpen={isDialogOpen} setIsOpen={handleOpenDialog}>
                <GeneralLedgerAccountForm closeDialog={() => setIsDialogOpen(false)} account={selectedAccount} />
            </LedgerDialog>            
        </div>
    );

}