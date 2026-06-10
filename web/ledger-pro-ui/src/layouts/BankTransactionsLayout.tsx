import { Outlet } from "react-router-dom";

export function BankTransactionsLayout() {
    return (
    <div>
        <h1 className="text-2xl font-bold mb-6">Bank Transactions</h1>
        <p className="mb-4 text-sm text-muted-foreground">Manage your bank transactions and records.</p>

        <main>
            <Outlet />
        </main>
    </div>
    );
}

